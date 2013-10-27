using System;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Data;

namespace IntegrationExample.Data
{
    public class ContactEmailSelectionValueConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactEmailSelectionValueConverter"/> class.
        /// </summary>
        public ContactEmailSelectionValueConverter()
        {
            PreferredEmailOrder = EmailOrder.PersonalWorkOther;
        }

        public EmailOrder PreferredEmailOrder { get; set; }

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
                var personal = contactValue.Emails.FirstOrDefault(x => x.Kind == ContactEmailKind.Personal);
                var work = contactValue.Emails.FirstOrDefault(x => x.Kind == ContactEmailKind.Work);
                var other = contactValue.Emails.FirstOrDefault(x => x.Kind == ContactEmailKind.Other);

                if (PreferredEmailOrder == EmailOrder.PersonalWorkOther)
                {
                    if (personal != null) return personal.Address;
                    if (work != null) return work.Address;
                    if (other != null) return other.Address;
                }
                else
                {
                    if (work != null) return work.Address;
                    if (personal != null) return personal.Address;
                    if (other != null) return other.Address;
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

        public enum EmailOrder
        {
            PersonalWorkOther,
            WorkPersonalOther,
        }
    }
}