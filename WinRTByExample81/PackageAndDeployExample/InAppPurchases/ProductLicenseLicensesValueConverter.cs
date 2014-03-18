using System;
using System.Linq;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml.Data;

namespace PackageAndDeployExample
{
    public class ProductLicenseLicensesValueConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var licenseInformation = (LicenseInformation)value;
            var productLicensesList = licenseInformation.ProductLicenses.Values.ToList();
            return productLicensesList;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}