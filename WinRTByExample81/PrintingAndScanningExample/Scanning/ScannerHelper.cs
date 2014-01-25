using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using PrintingAndScanningExample.Annotations;

namespace PrintingAndScanningExample
{
    public class ScannerHelper
    {
        public async Task<IEnumerable<ScannerModel>> GetScannersAsync()
        {
            // String to be used to enumerate scanners;
            var deviceSelector = ImageScanner.GetDeviceSelector();

            var scanners = await DeviceInformation.FindAllAsync(deviceSelector);
            var result = scanners
                .Select(x => new ScannerModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDefault = x.IsDefault,
                    IsEnabled = x.IsEnabled,
                    //Thumbnail = await x.GetThumbnailAsync()
                })
                .ToList();
            return result;
        }

        public async Task<ScanSourceDetails> GetSupportedScannerSources(String scannerDeviceId)
        {
            var scanner = await ImageScanner.FromIdAsync(scannerDeviceId);

            var supportedSources = new List<ScanSourceDetailsItem>();
            if (scanner.IsScanSourceSupported(ImageScannerScanSource.Flatbed))
            {

                supportedSources.Add(new ScanSourceDetailsItem
                                     {
                                         SourceType = ImageScannerScanSource.Flatbed,
                                         SupportsPreview = scanner.IsPreviewSupported(ImageScannerScanSource.Flatbed)
                                     });
            }
            if (scanner.IsScanSourceSupported(ImageScannerScanSource.Feeder))
            {

                supportedSources.Add(new ScanSourceDetailsItem
                {
                    SourceType = ImageScannerScanSource.Feeder,
                    SupportsPreview = scanner.IsPreviewSupported(ImageScannerScanSource.Feeder)
                });
            }
            if (scanner.IsScanSourceSupported(ImageScannerScanSource.AutoConfigured))
            {

                supportedSources.Add(new ScanSourceDetailsItem
                {
                    SourceType = ImageScannerScanSource.AutoConfigured,
                    SupportsPreview = scanner.IsPreviewSupported(ImageScannerScanSource.AutoConfigured)
                });
            } 

            var results = new ScanSourceDetails
                          {
                              SupportedScanSources = supportedSources,
                              DefaultScanSource = scanner.DefaultScanSource
                          };
            return results;
        }

        public async Task<BitmapImage> ScanPicturePreviewAsync(String scannerDeviceId, ImageScannerScanSource scanSource)
        {
            var scanner = await ImageScanner.FromIdAsync(scannerDeviceId);
            if (scanner.IsPreviewSupported(scanSource))
            {
                var stream = new InMemoryRandomAccessStream();
                var result = await scanner.ScanPreviewToStreamAsync(scanSource, stream);
                if (result.Succeeded)
                {
                    var thumbnail = new BitmapImage();
                    thumbnail.SetSource(stream);
                    return thumbnail;
                }
            }
            return null;
        }

        //public async Task<IEnumerable<StorageFile>> ScanPictureAsync(String scannerDeviceId, Double horizontalScanPercentage, Double verticalScanPercentage, Progress<UInt32> progressHandler, CancellationToken cancellationToken)
        public async Task<IEnumerable<StorageFile>> ScanPictureAsync(String scannerDeviceId, ImageScannerScanSource scanSource, Double horizontalScanPercentage, Double verticalScanPercentage, Progress<UInt32> progressHandler)
        {
            var scanner = await ImageScanner.FromIdAsync(scannerDeviceId);
            if (scanner.IsScanSourceSupported(scanSource))
            {
                var picker = new FolderPicker {SuggestedStartLocation = PickerLocationId.PicturesLibrary };
                picker.FileTypeFilter.Add("*");
                var destinationFolder = await picker.PickSingleFolderAsync();
                if (destinationFolder != null)
                {
                    ConfigureScanner(scanner, scanSource, horizontalScanPercentage, verticalScanPercentage);

                    var scanResult = await scanner
                        .ScanFilesToFolderAsync(scanSource, destinationFolder)
                        .AsTask(progressHandler);
                        //.AsTask(cancellationToken, progressHandler);

                    var results = new List<StorageFile>();
                    // Caution - do NOT try to enumerate this list (foreach) - you'll get a nasty COM exception.
                    // Instead, just walking the array and building a parallel list works fine.
                    for (var i = 0; i < scanResult.ScannedFiles.Count; i++)
                    {
                        results.Add(scanResult.ScannedFiles[i]);
                    }
                    return results;
                }
            }
            return null;
        }

        private static void ConfigureScanner([NotNull] ImageScanner scanner, ImageScannerScanSource scanSource, Double horizontalScanPercentage, Double verticalScanPercentage)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");

            IImageScannerSourceConfiguration sourceConfiguration = null;
            IImageScannerFormatConfiguration formatConfiguration = null;

            if (scanSource == ImageScannerScanSource.Flatbed)
            {
                sourceConfiguration = scanner.FlatbedConfiguration;
                formatConfiguration = scanner.FlatbedConfiguration;
            }
            else if (scanSource == ImageScannerScanSource.Feeder)
            {
                sourceConfiguration = scanner.FeederConfiguration;
                formatConfiguration = scanner.FeederConfiguration;
                //Additional feeder-specific settings:
                    //scanner.FeederConfiguration.AutoDetectPageSize
                    //scanner.FeederConfiguration.CanAutoDetectPageSize
                    //scanner.FeederConfiguration.CanScanAhead
                    //scanner.FeederConfiguration.ScanAhead
                    //scanner.FeederConfiguration.CanScanDuplex
                    //scanner.FeederConfiguration.Duplex
                    //scanner.FeederConfiguration.PageOrientation
                    //scanner.FeederConfiguration.PageSize
                    //scanner.FeederConfiguration.PageSizeDimensions
            }
            else if (scanSource == ImageScannerScanSource.AutoConfigured)
            {
                formatConfiguration = scanner.AutoConfiguration;
            }

            // Potentially update the scanner configuration
            if (sourceConfiguration != null)
            {
                var maxScanArea = sourceConfiguration.MaxScanArea; // Size, with Width, Height in Inches    // MinScanArea
                sourceConfiguration.SelectedScanRegion = new Rect(
                    0,
                    0,
                    maxScanArea.Width*horizontalScanPercentage,
                    maxScanArea.Height*verticalScanPercentage); // In inches
                // Additional Configuration settings
                    // sourceConfiguration.AutoCroppingMode
                    // sourceConfiguration.ColorMode    // DefaultColorMode
                    // sourceConfiguration.Brightness   // DefaultBrightness    //MaxBrightness     // MinBrightness
                    // sourceConfiguration.Contrast     // DefaultContrast      // MaxContrast      // MinContrast
                    // sourceConfiguration.DesiredResolution = resolution;      // MaxResolution    // MinResolution
                    // var actualResolution = sourceConfiguration.ActualResolution;
            }

            // Potentially update the format that the end product is saved to
            if (formatConfiguration != null)
            {
                Debug.WriteLine("Default format is {0}", formatConfiguration.DefaultFormat);

                if (formatConfiguration.IsFormatSupported(ImageScannerFormat.Png))
                    formatConfiguration.Format = ImageScannerFormat.Png;
                else if (formatConfiguration.IsFormatSupported(ImageScannerFormat.Jpeg))
                    formatConfiguration.Format = ImageScannerFormat.Jpeg;
                else if (formatConfiguration.IsFormatSupported(ImageScannerFormat.Pdf))
                    formatConfiguration.Format = ImageScannerFormat.Pdf;
                //else if (formatConfiguration.IsFormatSupported(ImageScannerFormat.Tiff))
                //    formatConfiguration.Format = ImageScannerFormat.Tiff;
                //else if (formatConfiguration.IsFormatSupported(ImageScannerFormat.Xps))
                //    formatConfiguration.Format = ImageScannerFormat.Xps;
                //else if (formatConfiguration.IsFormatSupported(ImageScannerFormat.OpenXps))
                //    formatConfiguration.Format = ImageScannerFormat.OpenXps;
                // NOTE: If your desired format isn't natively supported, it may be possible to generate
                // the desired format post-process using image conversion, etc. libraries.

                Debug.WriteLine("Configured format is {0}", formatConfiguration.Format);
            }
        }
    }

    public class ScanSourceDetails
    {
        public IEnumerable<ScanSourceDetailsItem> SupportedScanSources { get; set; }
        public ImageScannerScanSource DefaultScanSource { get; set; }
    }

    public class ScanSourceDetailsItem
    {
        public ImageScannerScanSource SourceType { get; set; }
        public Boolean SupportsPreview { get; set; }
    }
}