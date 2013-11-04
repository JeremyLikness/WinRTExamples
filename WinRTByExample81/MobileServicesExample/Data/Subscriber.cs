using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace MobileServicesExample
{
    [DataTable("Subscribers")]
    public class Subscriber : INotifyPropertyChanged
    {
        #region Fields

        private Int32 _id;
        private String _firstName;
        private String _lastName;
        private Gender _gender;
        private String _mailingAddress;
        private String _emailAddress;
        private String _phone; 
   
        #endregion     

        public Int32 Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public String FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        } 

        public String LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public String MailingAddress
        {
            get { return _mailingAddress; }
            set
            {
                _mailingAddress = value;
                OnPropertyChanged();
            }
        }

        public String EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                _emailAddress = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "PhoneNumber")]
        public String Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public String Unused { get; set; }

        public void CopyTo(Subscriber destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            destination.Id = Id;
            destination.FirstName = FirstName;
            destination.LastName = LastName;
            destination.Gender = Gender;
            destination.MailingAddress = MailingAddress;
            destination.EmailAddress = EmailAddress;
            destination.Phone = Phone;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}