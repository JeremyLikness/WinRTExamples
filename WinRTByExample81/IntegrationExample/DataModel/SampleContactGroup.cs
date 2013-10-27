using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;

namespace IntegrationExample.Data
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleContactGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleContactGroup"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public SampleContactGroup(String key)
        {
            Key = key;
            Items = new ObservableCollection<Contact>();
        }

        /// <summary>
        /// Gets the group key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public String Key { get; private set; }

        /// <summary>
        /// Gets the group items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<Contact> Items { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Key;
        }
    }
}