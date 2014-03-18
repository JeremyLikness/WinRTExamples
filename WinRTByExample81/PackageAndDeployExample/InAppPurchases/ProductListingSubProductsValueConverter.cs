using System;
using System.Linq;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml.Data;

namespace PackageAndDeployExample
{
    public class ProductListingSubProductsValueConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var listingInformation = (ListingInformation) value;
            var subproductListingsList = listingInformation.ProductListings.Values.ToList();
            return subproductListingsList;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}