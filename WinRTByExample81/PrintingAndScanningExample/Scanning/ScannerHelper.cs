using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace PrintingAndScanningExample
{
    public class ScannerHelper
    {
        public async Task<IEnumerable<ScannerModel>> GetScannersAsync()
        {
            // String to be used to enumerate scanners
            var deviceSelector = ImageScanner.GetDeviceSelector();

            var scanners = await DeviceInformation.FindAllAsync(deviceSelector);
            var result = scanners
                .Select(x => new ScannerModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDefault = x.IsDefault,
                    IsEnabled = x.IsEnabled,
                });
            return result;

            // Alternatively, a watcher can be used, which raises events as items are located/added/removed/updated:
            //var watcher = DeviceInformation.CreateWatcher(deviceSelector);
            //watcher.EnumerationCompleted += (sender, args) => Debug.WriteLine("Scanners enumerated");
            //watcher.Added += (sender, args) => Debug.WriteLine("Scanner added - {0}", args.Name);
            //watcher.Removed += (sender, args) => Debug.WriteLine("Scanner removed - {0}", args.Id);
            //watcher.Updated += (sender, args) => Debug.WriteLine("Scanner updated - {0}", args.Id);
            //watcher.Start();
        }

        public async Task<ScanSourceDetails> GetSupportedScanSources
            (String deviceId)
        {
            // Check whether each of the known scan sources is available
            // for the scanner with the provided ID
            var scanSources = new[]
                              {
                                  ImageScannerScanSource.Flatbed,
                                  ImageScannerScanSource.Feeder,
                                  ImageScannerScanSource.AutoConfigured
                              };

            var scanner = await ImageScanner.FromIdAsync(deviceId);

            var supportedSources = scanSources
                .Where(x => scanner.IsScanSourceSupported(x))
                .Select(x => new ScanSourceDetailsItem
                             {
                                 SourceType = x,
                                 SupportsPreview = scanner.IsPreviewSupported(x)
                             })
                .ToList();

            var results = new ScanSourceDetails
                          {
                              SupportedScanSources = supportedSources,
                              DefaultScanSource = scanner.DefaultScanSource
                          };
            return results;
        }

        public async Task<BitmapImage> ScanPicturePreviewAsync
            (String scannerDeviceId, ImageScannerScanSource scanSource)
        {
            var scanner = await ImageScanner.FromIdAsync(scannerDeviceId);
            if (scanner.IsPreviewSupported(scanSource))
            {
                var stream = new InMemoryRandomAccessStream();
                var result = 
                    await scanner.ScanPreviewToStreamAsync(scanSource, stream);
                if (result.Succeeded)
                {
                    var thumbnail = new BitmapImage();
                    thumbnail.SetSource(stream);
                    return thumbnail;
                }
            }
            return null;
        }

        public async Task<IEnumerable<StorageFile>> ScanPicturesAsync(
            String scannerDeviceId, ImageScannerScanSource source, 
            StorageFolder destinationFolder,
            Double hScanPercent, Double vScanPercent)
        {
            var scanner = await ImageScanner.FromIdAsync(scannerDeviceId);
            if (scanner.IsScanSourceSupported(source))
            {
                ConfigureScanner(scanner, source, hScanPercent, vScanPercent);

                var scanResult = await scanner
                    .ScanFilesToFolderAsync(source, destinationFolder);
                
                var results = new List<StorageFile>();
                // Caution - enumerating this list (foreach) will result in a 
                // COM exception.  Instead, just walk the collection by index 
                // and build a new list.
                for (var i = 0; i < scanResult.ScannedFiles.Count; i++)
                {
                    results.Add(scanResult.ScannedFiles[i]);
                }
                return results;
            }
            return null;
        }

        private static void ConfigureScanner(
            ImageScanner scanner, ImageScannerScanSource source,
            Double hScanPercent, Double vScanPercent)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");

            IImageScannerSourceConfiguration sourceConfig = null;
            IImageScannerFormatConfiguration formatConfig = null;

            switch (source)
            {
                case ImageScannerScanSource.Flatbed:
                    sourceConfig = scanner.FlatbedConfiguration;
                    formatConfig = scanner.FlatbedConfiguration;
                    break;
                case ImageScannerScanSource.Feeder:
                    sourceConfig = scanner.FeederConfiguration;
                    formatConfig = scanner.FeederConfiguration;
                    //Additional feeder-specific settings:
                    //scanner.FeederConfiguration.CanScanDuplex
                    //scanner.FeederConfiguration.Duplex
                    //scanner.FeederConfiguration.CanScanAhead
                    //scanner.FeederConfiguration.ScanAhead
                    //scanner.FeederConfiguration.AutoDetectPageSize
                    //scanner.FeederConfiguration.CanAutoDetectPageSize
                    //scanner.FeederConfiguration.PageSize
                    //scanner.FeederConfiguration.PageSizeDimensions
                    //scanner.FeederConfiguration.PageOrientation
                    break;
                case ImageScannerScanSource.AutoConfigured:
                    formatConfig = scanner.AutoConfiguration;
                    break;
            }

            // Potentially update the scanner configuration
            if (sourceConfig != null)
            {
                var maxScanArea = sourceConfig.MaxScanArea; // Size, with Width, Height in Inches    // MinScanArea
                sourceConfig.SelectedScanRegion = new Rect(
                    0,
                    0,
                    maxScanArea.Width * hScanPercent,
                    maxScanArea.Height * vScanPercent); // In inches
                // Additional Configuration settings
                    // sourceConfig.AutoCroppingMode
                    // sourceConfig.ColorMode ==     // DefaultColorMode
                    // sourceConfig.Brightness   // DefaultBrightness    //MaxBrightness     // MinBrightness
                    // sourceConfig.Contrast     // DefaultContrast      // MaxContrast      // MinContrast
                    // sourceConfig.DesiredResolution = resolution;      // MaxResolution    // MinResolution
                    // var actualResolution = sourceConfig.ActualResolution;
            }

            // Potentially update the format that the end product is saved to
            if (formatConfig != null)
            {
                Debug.WriteLine("Default format is {0}", formatConfig.DefaultFormat);

                // NOTE: If your desired format isn't natively supported, it may 
                // be possible to generate the desired format post-process 
                // using image conversion, etc. libraries.
                var desiredFormats = new[]
                {
                    ImageScannerFormat.Png,
                    ImageScannerFormat.Jpeg,
                    ImageScannerFormat.DeviceIndependentBitmap,
                    //ImageScannerFormat.Tiff,
                    //ImageScannerFormat.Xps,
                    //ImageScannerFormat.OpenXps,
                    //ImageScannerFormat.Pdf
                };

                foreach (var format in desiredFormats)
                {
                    if (formatConfig.IsFormatSupported(format))
                    {
                        formatConfig.Format = format;
                        break;
                    }
                }

                Debug.WriteLine("Configured format is {0}", formatConfig.Format);
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