using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using PrintingAndScanningExample.Annotations;

namespace PrintingAndScanningExample
{
    public class PrintTemplateViewModel
    {
        private readonly List<TemplatePictureModel> _pictures = new List<TemplatePictureModel>();
        private readonly String _pageTitle;

        public PrintTemplateViewModel(Int32 pageNumber, Int32 picturesPerPage, [NotNull] IEnumerable<PictureModel> pictureList, Boolean useThumbnails, String pageTitle)
        {
            if (pictureList == null) throw new ArgumentNullException("pictureList");

            _pictures.AddRange(
                pictureList.Skip((pageNumber - 1)*picturesPerPage)
                    .Take(picturesPerPage)
                    .Select(x => new TemplatePictureModel(x, useThumbnails)));

            _pageTitle = pageTitle;
        }

        public IList<TemplatePictureModel> Pictures { get { return _pictures; } }

        public String PageTitle { get { return _pageTitle; } }
    }

    public class TemplatePictureModel
    {
        public TemplatePictureModel([NotNull] PictureModel pictureModel, Boolean useThumbnail)
        {
            if (pictureModel == null) throw new ArgumentNullException("pictureModel");

            PictureModel = pictureModel;
            UseThumbnail = useThumbnail;
        }

        public PictureModel PictureModel { get; private set; }

        public Boolean UseThumbnail { get; private set; }
    }

    public abstract class DesignPrintTemplateViewModelBase : PrintTemplateViewModel
    {
        protected static readonly List<PictureModel> SamplePictures = new List<PictureModel>
            {
                new PictureModel
                {
                    Caption = "Sample One",
                    Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                    Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"))
                },
                new PictureModel
                {
                    Caption = "Sample Two",
                    Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                    Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"))
                },
                new PictureModel
                {
                    Caption = "Sample Three",
                    Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                    Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"))
                },
                new PictureModel
                {
                    Caption = "Sample Four",
                    Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                    Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"))
                },
            };

        protected DesignPrintTemplateViewModelBase(Int32 pageNumber, Int32 picturesPerPage, IEnumerable<PictureModel> pictureList, Boolean useThumbnails, String pageTitle)
            : base(pageNumber, picturesPerPage, pictureList, useThumbnails, pageTitle)
        {
        }
    }
}