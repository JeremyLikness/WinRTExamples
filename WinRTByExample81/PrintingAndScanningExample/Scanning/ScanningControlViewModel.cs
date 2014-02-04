using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using PrintingAndScanningExample.Annotations;
using PrintingAndScanningExample.Common;

namespace PrintingAndScanningExample
{
    public class ScanningControlViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private readonly ScannerHelper _scannerHelper;

        private readonly ObservableCollection<ScannerModel> _scanners = new ObservableCollection<ScannerModel>();
        private readonly ObservableCollection<ScanSourceDetailsItem> _scanSources = new ObservableCollection<ScanSourceDetailsItem>();
        private RelayCommand _scanPreviewCommand;
        private RelayCommand _scanPictureCommand;
        private Double _horizontalScanPercentage = 1.0;
        private Double _verticalScanPercentage = 1.0;
        private BitmapImage _previewImage;
        private ScannerModel _selectedScanner;
        private ScanSourceDetailsItem _selectedScanSource;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanningControlViewModel"/> class.
        /// </summary>
        /// <param name="scannerHelper">The scanner helper.</param>
        /// <exception cref="System.ArgumentNullException">scannerHelper</exception>
        public ScanningControlViewModel([NotNull] ScannerHelper scannerHelper)
        {
            if (scannerHelper == null) throw new ArgumentNullException("scannerHelper");

            _scannerHelper = new ScannerHelper();
        }

        public async void GetScanners()
        {
            _scanners.Clear();
            var scanners = await _scannerHelper.GetScannersAsync();
            foreach (var scanner in scanners)
            {
                _scanners.Add(scanner);
            }

            // Choose the first scanner marked as system default, or just the first one period
            var defaultScanner = _scanners.FirstOrDefault(x => x.IsDefault) ??
                                 _scanners.FirstOrDefault();
            SelectedScanner = defaultScanner;
        }

        #endregion

        public IEnumerable<ScannerModel> Scanners { get { return _scanners; } }

        public ScannerModel SelectedScanner
        {
            get { return _selectedScanner; }
            set
            {
                if (Equals(value, _selectedScanner)) return;
                _selectedScanner = value;
                OnPropertyChanged();
                _scanSources.Clear();
                ResetAvailableScanSources();
                ScanPreviewCommand.RaiseCanExecuteChanged();
                ScanPictureCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<ScanSourceDetailsItem> ScanSources { get { return _scanSources; } }

        public ScanSourceDetailsItem SelectedScanSource
        {
            get { return _selectedScanSource; }
            set
            {
                if (Equals(value, _selectedScanSource)) return;
                _selectedScanSource = value;
                OnPropertyChanged();
                ScanPreviewCommand.RaiseCanExecuteChanged();
                ScanPictureCommand.RaiseCanExecuteChanged();
            }
        }

        public BitmapImage PreviewImage
        {
            get { return _previewImage; }
            set
            {
                if (Equals(value, _previewImage)) return;
                _previewImage = value;
                OnPropertyChanged();
                OnPropertyChanged("IsPreviewLoaded");
                ScanPictureCommand.RaiseCanExecuteChanged();
            }
        }

        public Boolean IsPreviewLoaded
        {
            get { return PreviewImage != null; }
        }

        public Double HorizontalScanPercentage
        {
            get { return _horizontalScanPercentage; }
            set
            {
                if (value.Equals(_horizontalScanPercentage)) return;
                _horizontalScanPercentage = value;
                OnPropertyChanged();
            }
        }

        public Double VerticalScanPercentage
        {
            get { return _verticalScanPercentage; }
            set
            {
                if (value.Equals(_verticalScanPercentage)) return;
                _verticalScanPercentage = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<ScanProgressEventArgs> ScanProgressChanged;

        protected virtual void OnScanProgressChanged(UInt32 progress)
        {
            EventHandler<ScanProgressEventArgs> handler = ScanProgressChanged;
            if (handler != null) handler(this, new ScanProgressEventArgs {ScanProgress = progress});
        }

        public event EventHandler<ScanCompletedResultEventArgs> ScanCompleted;

        protected virtual void OnScanCompleted(IEnumerable<StorageFile> scannedFiles)
        {
            EventHandler<ScanCompletedResultEventArgs> handler = ScanCompleted;
            if (handler != null) handler(this, new ScanCompletedResultEventArgs { ScannedFiles = scannedFiles });
        }

        public RelayCommand ScanPreviewCommand
        {
            get { return _scanPreviewCommand ?? (_scanPreviewCommand = new RelayCommand(ScanPreview, CanScanPreview)); }
        }

        public RelayCommand ScanPictureCommand
        {
            get { return _scanPictureCommand ?? (_scanPictureCommand = new RelayCommand(ScanPicture, CanScanPicture)); }
        }

        private async void ScanPreview()
        {
            PreviewImage = await _scannerHelper.ScanPicturePreviewAsync(SelectedScanner.Id, SelectedScanSource.SourceType);
        }

        private Boolean CanScanPreview()
        {
            return SelectedScanner != null && SelectedScanner.IsEnabled && 
                SelectedScanSource != null && SelectedScanSource.SupportsPreview;
        }

        private async void ResetAvailableScanSources()
        {
            SelectedScanSource = null;
            _scanSources.Clear();

            if (SelectedScanner != null)
            {
                var scanSourceDetails = await _scannerHelper.GetSupportedScanSources(SelectedScanner.Id);
                foreach (var supportedSource in scanSourceDetails.SupportedScanSources)
                {
                    _scanSources.Add(supportedSource);
                }
                SelectedScanSource =
                    _scanSources.FirstOrDefault(x => x.SourceType == scanSourceDetails.DefaultScanSource);
            }
        }

        private async void ScanPicture()
        {
            //var cancellationTokenSource = new CancellationTokenSource();
            //var scannedPictures = await _scannerHelper.ScanPictureAsync(SelectedScanner.Id, HorizontalScanPercentage, VerticalScanPercentage, progress, cancellationTokenSource.Token);

            var picker = new FolderPicker {SuggestedStartLocation = PickerLocationId.PicturesLibrary};
            picker.FileTypeFilter.Add("*");
            var destinationFolder = await picker.PickSingleFolderAsync();
            if (destinationFolder != null)
            {
                var scannedPictures =
                    await _scannerHelper.ScanPicturesAsync(SelectedScanner.Id, SelectedScanSource.SourceType, destinationFolder, HorizontalScanPercentage, VerticalScanPercentage);
                OnScanCompleted(scannedPictures);
            }
        }

        private Boolean CanScanPicture()
        {
            if (SelectedScanSource != null)
            {
                return SelectedScanSource.SupportsPreview
                    ? SelectedScanner != null && SelectedScanner.IsEnabled && IsPreviewLoaded
                    : SelectedScanner != null && SelectedScanner.IsEnabled;
            }
            return false;
        }

        private void ScanProgressHandler(UInt32 progress)
        {
            _syncContext.Post(x => OnScanProgressChanged(progress), null);
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }

    public class ScanProgressEventArgs : EventArgs
    {
        public UInt32 ScanProgress { get; set; }
    }

    public class ScanCompletedResultEventArgs : EventArgs
    {
        public IEnumerable<StorageFile> ScannedFiles { get; set; }
    }
}