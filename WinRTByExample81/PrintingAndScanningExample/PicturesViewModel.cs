using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        private readonly ObservableCollection<PictureModel> _pictures = new ObservableCollection<PictureModel>();
        private PictureModel _currentPicture;

        private RelayCommand _addPictureCommand;
        private RelayCommand _printPreviewCommand;
        private RelayCommand _deleteCurrentPictureCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesViewModel"/> class.
        /// </summary>
        /// <param name="printHelper">The print helper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// printHelper
        /// or
        /// scannerHelper
        /// </exception>
        public PicturesViewModel([NotNull] PrintHelper printHelper)
        {
            if (printHelper == null) throw new ArgumentNullException("printHelper");

            _printHelper = printHelper;
            _pictures.CollectionChanged += (sender, args) => PrintPreviewCommand.RaiseCanExecuteChanged();
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

        public RelayCommand AddPicturesCommand
        {
            get { return _addPictureCommand ?? (_addPictureCommand = new RelayCommand(AddPictures)); }
        }

        public RelayCommand PrintPreviewCommand
        {
            get { return _printPreviewCommand ?? (_printPreviewCommand = new RelayCommand(ShowPrintPreview, CanShowPrintPreview)); }
        }

        public RelayCommand DeleteCurrentPictureCommand
        {
            get
            {
                return _deleteCurrentPictureCommand ??
                       (_deleteCurrentPictureCommand = new RelayCommand(DeleteCurrentPicture, CanDeleteCurrentPicture));
            }
        }

        public async Task AddPicturesFromFiles([NotNull] IEnumerable<StorageFile> files)
        {
            if (files == null) throw new ArgumentNullException("files");

            PictureModel pictureModel = null;
            foreach (var file in files)
            {
                var bitmap = new BitmapImage();
                var bitmapFileStream = await file.OpenAsync(FileAccessMode.Read);
                bitmap.SetSource(bitmapFileStream);

                var thumbnail = new BitmapImage();
                var thumbnailStream = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                thumbnail.SetSource(thumbnailStream);
                pictureModel = new PictureModel
                {
                    Caption = file.DisplayName,
                    Picture = bitmap,
                    Thumbnail = thumbnail
                };
                _pictures.Add(pictureModel);
            }

            // Set the current picture once all of the pictures have been added
            if (pictureModel != null) CurrentPicture = pictureModel;
        }

        private async void AddPictures()
        {
            var picker = new FileOpenPicker
                         {
                             FileTypeFilter = { ".jpg", ".jpeg", ".bmp", ".png" },
                             SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                             ViewMode = PickerViewMode.Thumbnail
                         };
            var files = await picker.PickMultipleFilesAsync();
            await AddPicturesFromFiles(files);
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

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion

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