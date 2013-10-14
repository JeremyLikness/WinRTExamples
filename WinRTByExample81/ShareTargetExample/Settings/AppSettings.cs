using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace ShareTargetExample
{
    public class AppSettings
    {
        public const String CustomPersonSchemaName = "http://schema.org/Person";

        private static readonly ReadOnlyCollection<ShareFormat> OriginalCollection = 
            new ReadOnlyCollection<ShareFormat>(new[]
                                                {
                                                    new ShareFormat {DataFormat = CustomPersonSchemaName, DisplayName = "Custom Person"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.Html,DisplayName = "HTML"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.Bitmap,DisplayName = "Bitmap"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.WebLink,DisplayName = "Web Link"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.ApplicationLink,DisplayName = "App Link"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.StorageItems,DisplayName = "Storage Items"},
                                                    //new ShareFormat {DataFormat = StandardDataFormats.Rtf,DisplayName = "RTF"},
                                                    new ShareFormat {DataFormat = StandardDataFormats.Text, DisplayName = "Text"}
                                                });

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        public AppSettings()
        {
            OrderedFormats = GetShareFormatSettings().ToList();
            if (OrderedFormats.Count() < OriginalCollection.Count())
            {
                OrderedFormats = OriginalCollection.ToList();
                SaveShareFormatSettings(OrderedFormats);
            }
        }

        public Boolean AcceptAllSetting
        {
            get
            {
                var result = false;
                Object acceptAllObject;
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AcceptAll", out acceptAllObject))
                {
                    result = (Boolean) acceptAllObject;
                }
                return result;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["AcceptAll"] = value;
            }
        }

        public IEnumerable<ShareFormat> OrderedFormats
        {
            get;
            private set;
        }

        public void UpdateFormatOrder(IEnumerable<ShareFormat> newOrder)
        {
            if (newOrder == null) throw new ArgumentNullException("newOrder");

            var newOrderList = newOrder.ToList();
            SaveShareFormatSettings(newOrderList);
            OrderedFormats = newOrderList;
        }

        private IEnumerable<ShareFormat> GetShareFormatSettings()
        {
            var results = new List<ShareFormat>();
            Object serializedValues;
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("OrderedShareFormats", out serializedValues))
            {
                var commaDelString = serializedValues.ToString();
                var formats = commaDelString.Split(',');
                results.AddRange(formats.Select(format => OriginalCollection.First(x => x.DataFormat == format)));
            }
            return results;
        }
        private void SaveShareFormatSettings(IEnumerable<ShareFormat> values)
        {
            var commaDelString = String.Join(",", values.Select(x => x.DataFormat));
            ApplicationData.Current.LocalSettings.Values["OrderedShareFormats"] = commaDelString;
        }
    }
}