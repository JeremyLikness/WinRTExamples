using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using Newtonsoft.Json.Linq;

namespace IntegrationExample.Data
{
    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private readonly ObservableCollection<SampleContactGroup> _groups = new ObservableCollection<SampleContactGroup>();

        public ObservableCollection<SampleContactGroup> Groups
        {
            get { return _groups; }
        }

        //public static async Task<SampleContactGroup> GetGroupAsync(String key)
        //{
        //    await DataSource.GetSampleDataAsync();

        //    // Simple linear search is acceptable for small data sets
        //    return  DataSource.Groups.FirstOrDefault(group => group.Key.Equals(key));
        //}

        public Contact GetItem(String id)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _groups.SelectMany(group => group.Items).Where(item => item.Id.Equals(id)).ToList();
            return matches.FirstOrDefault();
        }

        public async Task ProcessActivationFile(IStorageFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var sampleDataFileText = await FileIO.ReadTextAsync(file);
            var sampleDataJsonObject = JObject.Parse(sampleDataFileText);
            var jsonContactArray = sampleDataJsonObject["Contacts"];

            var sampleContacts = jsonContactArray.Select(ProcessJsonContact);
            AddContacts(sampleContacts);
        }

        private Contact ProcessJsonContact(JToken jToken)
        {
            var result = new Contact
                         {
                             Id = (jToken["Id"] ?? String.Empty).ToString(),
                             LastName = (jToken["LastName"] ?? String.Empty).ToString(),
                             FirstName = (jToken["FirstName"] ?? String.Empty).ToString(),
                         };

            foreach (var value in jToken["Emails"] ?? new JArray())
            {
                result.Emails.Add(new ContactEmail
                                    {
                                        Address = value["Address"].ToString(),
                                        Kind = (ContactEmailKind) Enum.Parse(typeof (ContactEmailKind), value["Kind"].ToString()),
                                    });
            }
            foreach (var value in jToken["PhoneNumbers"] ?? new JArray())
            {
                result.Phones.Add(new ContactPhone
                                  {
                                      Number = value["Number"].ToString(),
                                      Kind = (ContactPhoneKind) Enum.Parse(typeof (ContactPhoneKind), value["Kind"].ToString())
                                  });
            }

            // In case an id was not provided, just use its index based on the number of records.
            if (String.IsNullOrWhiteSpace(result.Id))
            {
                result.Id = Guid.NewGuid().ToString();
            }
            return result;
        }

        private void AddContacts(IEnumerable<Contact> sampleContacts)
        {
            var groupedSampleContacts = sampleContacts
                .OrderBy(x => x.LastName)
                .GroupBy(x => String.IsNullOrWhiteSpace(x.LastName) ? "NONE" : x.LastName.Substring(0, 1));


            foreach (var sampleContactGroup in groupedSampleContacts)
            {
                var existingGroup = _groups.FirstOrDefault(x => x.Key == sampleContactGroup.Key);
                if (existingGroup == null)
                {
                    existingGroup = new SampleContactGroup(sampleContactGroup.Key);
                    _groups.Add(existingGroup);
                }
                foreach (var item in sampleContactGroup)
                {
                    existingGroup.Items.Add(item);
                }
            }
        }

        public void AddContact(Contact contact)
        {
            if (contact == null) throw new ArgumentNullException("contact");

            var contactId = String.IsNullOrWhiteSpace(contact.Id) ? Guid.NewGuid().ToString() : contact.Id;
            var contactToAdd = new Contact
                               {
                                   Id = contactId,
                                   LastName = contact.LastName,
                                   FirstName = contact.FirstName,
                               };

            foreach (var value in contact.Emails)
            {
                contactToAdd.Emails.Add(new ContactEmail
                {
                    Address = value.Address,
                    Kind = value.Kind,
                    Description = value.Description
                });
            }
            foreach (var value in contact.Phones)
            {
                contactToAdd.Phones.Add(new ContactPhone
                {
                    Number = value.Number,
                    Kind = value.Kind,
                    Description = value.Description
                });
            }

            AddContacts(new[] { contactToAdd });
        }
    }
}