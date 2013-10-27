using System;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Data;

namespace IntegrationExample.Data
{
    public class ContactPhoneSelectionValueConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactPhoneSelectionValueConverter"/> class.
        /// </summary>
        public ContactPhoneSelectionValueConverter()
        {
            PreferredPhoneOrder = PhoneOrder.MobileWorkHomeOther;
        }

        public PhoneOrder PreferredPhoneOrder { get; set; }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var contactValue = value as Contact;
            if (contactValue != null)
            {
                var home = contactValue.Phones.FirstOrDefault(x => x.Kind == ContactPhoneKind.Home);
                var mobile = contactValue.Phones.FirstOrDefault(x => x.Kind == ContactPhoneKind.Mobile);
                var work = contactValue.Phones.FirstOrDefault(x => x.Kind == ContactPhoneKind.Work);
                var other = contactValue.Phones.FirstOrDefault(x => x.Kind == ContactPhoneKind.Other);

                switch (PreferredPhoneOrder)
                {
                    case PhoneOrder.HomeWorkMobileOther:
                        if (home != null) return home.Number;
                        if (work != null) return work.Number;
                        if (mobile != null) return mobile.Number;
                        if (other != null) return other.Number;
                        break;
                    case PhoneOrder.HomeMobileWorkOther:
                        if (home != null) return home.Number;
                        if (mobile != null) return mobile.Number;
                        if (work != null) return work.Number;
                        if (other != null) return other.Number;
                        break;
                    case PhoneOrder.WorkHomeMobileOther:
                        if (work != null) return work.Number;
                        if (home != null) return home.Number;
                        if (mobile != null) return mobile.Number;
                        if (other != null) return other.Number;
                        break;
                    case PhoneOrder.WorkMobileHomeOther:
                        if (work != null) return work.Number;
                        if (mobile != null) return mobile.Number;
                        if (home != null) return home.Number;
                        if (other != null) return other.Number;
                        break;
                    case PhoneOrder.MobileHomeWorkOther:
                        if (mobile != null) return mobile.Number;
                        if (home != null) return home.Number;
                        if (work != null) return work.Number;
                        if (other != null) return other.Number;
                        break;
                    case PhoneOrder.MobileWorkHomeOther:
                        if (mobile != null) return mobile.Number;
                        if (work != null) return work.Number;
                        if (home != null) return home.Number;
                        if (other != null) return other.Number;
                        break;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }

        public enum PhoneOrder
        {
            HomeWorkMobileOther,
            HomeMobileWorkOther,
            WorkHomeMobileOther,
            WorkMobileHomeOther,
            MobileHomeWorkOther,
            MobileWorkHomeOther,
        }
    }
}