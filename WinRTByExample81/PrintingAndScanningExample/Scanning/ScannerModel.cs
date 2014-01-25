using System;

namespace PrintingAndScanningExample
{
    public class ScannerModel
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public Boolean IsDefault { get; set; }
        public Boolean IsEnabled { get; set; }
        //Thumbnail = await x.GetThumbnailAsync()
    }
}