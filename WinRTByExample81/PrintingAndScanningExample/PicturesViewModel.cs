using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using PrintingAndScanningExample.Annotations;
using PrintingAndScanningExample.Common;

namespace PrintingAndScanningExample
{
    public class PicturesViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly PrintHelper _printHelper;
        private readonly ScannerHelper _scannerHelper;

        private readonly ObservableCollection<PictureModel> _pictures = new ObservableCollection<PictureModel>();
        private PictureModel _currentPicture;

        private RelayCommand _addPictureCommand;
        private RelayCommand _scanPictureCommand;
        private RelayCommand _printPreviewCommand;
        private RelayCommand _deleteCurrentPictureCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesViewModel"/> class.
        /// </summary>
        /// <param name="printHelper">The print helper.</param>
        /// <param name="scannerHelper">The scanner helper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// printHelper
        /// or
        /// scannerHelper
        /// </exception>
        public PicturesViewModel([NotNull] PrintHelper printHelper, [NotNull] ScannerHelper scannerHelper)
        {
            if (printHelper == null) throw new ArgumentNullException("printHelper");
            if (scannerHelper == null) throw new ArgumentNullException("scannerHelper");

            _printHelper = printHelper;
            _scannerHelper = scannerHelper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesViewModel"/> class.
        /// </summary>
        protected PicturesViewModel()
        {
            // Provided for design-time support
        } 

        #endregion

        public IList<PictureModel> Pictures
        {
            get { return _pictures; }
        }

        public PictureModel CurrentPicture
        {
            get { return _currentPicture; }
            set
            {
                if (Equals(value, _currentPicture)) return;
                _currentPicture = value;
                OnPropertyChanged();
                _deleteCurrentPictureCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddPictureCommand
        {
            get { return _addPictureCommand ?? (_addPictureCommand = new RelayCommand(AddPicture)); }
        }

        public ICommand ScanPictureCommand
        {
            get { return _scanPictureCommand ?? (_scanPictureCommand = new RelayCommand(ScanPicture)); }
        }

        public ICommand PrintPreviewCommand
        {
            get { return _printPreviewCommand ?? (_printPreviewCommand = new RelayCommand(ShowPrintPreview, CanShowPrintPreview)); }
        }

        public ICommand DeleteCurrentPictureCommand
        {
            get
            {
                return _deleteCurrentPictureCommand ??
                       (_deleteCurrentPictureCommand = new RelayCommand(DeleteCurrentPicture, CanDeleteCurrentPicture));
            }
        }

        private async void AddPicture()
        {
            var picker = new FileOpenPicker
                         {
                             FileTypeFilter = { ".jpg", ".jpeg", ".bmp", ".png" },
                             SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                             ViewMode = PickerViewMode.Thumbnail
                         };
            var files = await picker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                var bitmap = new BitmapImage();
                var bitmapFileStream = await file.OpenAsync(FileAccessMode.Read);
                bitmap.SetSource(bitmapFileStream);

                var thumbnail = new BitmapImage();
                var thumbnailStream = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                thumbnail.SetSource(thumbnailStream);
                var pictureModel = new PictureModel
                                   {
                                       Caption = file.DisplayName,
                                       Picture = bitmap,
                                       Thumbnail = thumbnail
                                   };
                _pictures.Add(pictureModel);
                CurrentPicture = pictureModel;
            }
            _printPreviewCommand.RaiseCanExecuteChanged();
        }

        private void ScanPicture()
        {
            _scannerHelper.ScanPicture();
        }

        private async void ShowPrintPreview()
        {
            await _printHelper.ShowPrintPreview();
        }

        private Boolean CanShowPrintPreview()
        {
            return _pictures.Any();
        }

        private void DeleteCurrentPicture()
        {
            _pictures.Remove(CurrentPicture);
        }

        private Boolean CanDeleteCurrentPicture()
        {
            return _currentPicture != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SamplePicturesViewModel : PicturesViewModel
    {
        public SamplePicturesViewModel()
        {
            var picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png"));
            var thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));

            Pictures.Add(new PictureModel
            {
                Caption = "Picture One",
                Picture = picture,
                Thumbnail = thumbnail
            });
            //Pictures.Add(new PictureModel
            //{
            //    Caption = "Picture Two",
            //    Picture = picture,
            //    Thumbnail = thumbnail
            //});
        }
    }
}