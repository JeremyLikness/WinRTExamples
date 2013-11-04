using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAzure.MobileServices;
using MobileServicesExample.Common;

namespace MobileServicesExample
{
    public class SubscribersViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService _dialogService;
        private IList<Subscriber> _subscribers = new List<Subscriber>();
        private Subscriber _selectedSubscriber;
        private Subscriber _subscriberBeingEdited;
        private RelayCommand _refreshItemsCommand;
        private RelayCommand _addSubscriberCommand;
        private RelayCommand _saveSubscriberBeingAddedCommand;
        private RelayCommand _editSelectedSubscriberCommand;
        private RelayCommand _saveSubscriberBeingEditedCommand;
        private RelayCommand _deleteSelectedSubscriberCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribersViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        public SubscribersViewModel(IDialogService dialogService)
        {
            if (dialogService == null) throw new ArgumentNullException("dialogService");
            _dialogService = dialogService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribersViewModel"/> class.
        /// </summary>
        /// <remarks>Added to support parameter-less ctor for the sample data class</remarks>
        protected SubscribersViewModel()
        {
        }

        public IList<Subscriber> Subscribers
        {
            get { return _subscribers; }
            set
            {
                _subscribers = value ?? new List<Subscriber>();
                OnPropertyChanged();
            }
        }

        public Subscriber SelectedSubscriber
        {
            get { return _selectedSubscriber; }
            set
            {
                if (_selectedSubscriber != value)
                {
                    _selectedSubscriber = value;

                    EditSelectedSubscriberCommand.RaiseCanExecuteChanged();
                    DeleteSelectedSubscriberCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged();
                }
            }
        }

        public Subscriber SubscriberBeingEdited
        {
            get { return _subscriberBeingEdited; }
            set
            {
                if (_subscriberBeingEdited != value)
                {
                    _subscriberBeingEdited = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand RefreshItemsCommand
        {
            get
            {
                return _refreshItemsCommand ??
                       (_refreshItemsCommand = new RelayCommand(
                           RefreshItems
                           ));
            }
        }

        public RelayCommand AddSubscriberCommand
        {
            get
            {
                return _addSubscriberCommand ??
                       (_addSubscriberCommand = new RelayCommand(
                           AddSubscriber
                           ));
            }
        }

        public RelayCommand SaveSubscriberBeingAddedCommand
        {
            get
            {
                return _saveSubscriberBeingAddedCommand ??
                       (_saveSubscriberBeingAddedCommand = new RelayCommand(
                           SaveSubscriberBeingAdded
                           ));
            }
        }


        public RelayCommand EditSelectedSubscriberCommand
        {
            get
            {
                return _editSelectedSubscriberCommand ??
                       (_editSelectedSubscriberCommand = new RelayCommand(
                           EditSelectedSubscriber,
                           CanUpdateSelectedSubscriber
                           ));
            }
        }

        public RelayCommand SaveSubscriberBeingEditedCommand
        {
            get
            {
                return _saveSubscriberBeingEditedCommand ??
                       (_saveSubscriberBeingEditedCommand = new RelayCommand(
                           SaveSubscriberBeingEdited
                           ));
            }
        }

        public RelayCommand DeleteSelectedSubscriberCommand
        {
            get
            {
                return _deleteSelectedSubscriberCommand ??
                       (_deleteSelectedSubscriberCommand = new RelayCommand(
                           DeleteSelectedSubscriber,
                           CanDeleteSelectedSubscriber
                           ));
            }
        }

        private void RefreshItems()
        {
            var table = App.WinRTByExampleBookClient.GetTable<Subscriber>();
            var query = table.CreateQuery()
                //.Where(x => x.Gender == Gender.Male)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName);
            Subscribers = query.ToIncrementalLoadingCollection();
        }

        //private async void GetCollection()
        //{
        //    var table = App.WinRTByExampleBookClient.GetTable<Subscriber>();
        //    var query = table.CreateQuery()
        //        .Where(x => x.Gender == Gender.Male)
        //        .OrderBy(x => x.LastName)
        //        .ThenBy(x => x.FirstName);
        //    var collection = await query.ToCollectionAsync(50);
        //}

        private void AddSubscriber()
        {
            SubscriberBeingEdited = new Subscriber();
        }

        private async void SaveSubscriberBeingAdded()
        {
            try
            {
                var table = App.WinRTByExampleBookClient.GetTable<Subscriber>();
                await table.InsertAsync(SubscriberBeingEdited);
                Subscribers.Add(SubscriberBeingEdited);
                _dialogService.ShowMessageBox(String.Format("Successfully saved a new subscriber with id {0}", SubscriberBeingEdited.Id), "Save Succeeded");
            }
            catch (MobileServiceInvalidOperationException e)
            {
                String errorMessage;
                if (e.Response != null && e.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    errorMessage = "You are required to log in through the Credentials tab in the application's settings before you can add records.";
                }
                else
                {
                    errorMessage = String.Format("An error occurred - {0}.", e.Message);
                }
                _dialogService.ShowError(errorMessage);
            }
        }

        private void EditSelectedSubscriber()
        {
            var subscriberToEdit = new Subscriber();
            SelectedSubscriber.CopyTo(subscriberToEdit);
            SubscriberBeingEdited = subscriberToEdit;
        }

        private Boolean CanUpdateSelectedSubscriber()
        {
            return SelectedSubscriber != null;
        }

        private async void SaveSubscriberBeingEdited()
        {
            try
            {
                var table = App.WinRTByExampleBookClient.GetTable<Subscriber>();
                await table.UpdateAsync(SubscriberBeingEdited);
                SubscriberBeingEdited.CopyTo(SelectedSubscriber);
                _dialogService.ShowMessageBox("The subscriber was successfully updated.", "Update Succeeded");
            }
            catch (MobileServiceInvalidOperationException e)
            {
                String errorMessage;
                if (e.Response != null && e.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    errorMessage = "You are required to log in through the Credentials tab in application's settings before you can edit records.";
                }
                else
                {
                    errorMessage = String.Format("An error occurred - {0}.", e.Message);
                }
                _dialogService.ShowError(errorMessage);
            }
        }

        private async void DeleteSelectedSubscriber()
        {
            try
            {
                var table = App.WinRTByExampleBookClient.GetTable<Subscriber>();
                await table.DeleteAsync(SelectedSubscriber);
                Subscribers.Remove(SelectedSubscriber);
                _dialogService.ShowMessageBox("The subscriber was successfully deleted.", "Delete Succeeded");
            }
            catch (MobileServiceInvalidOperationException e)
            {
                String errorMessage;
                if (e.Response != null && e.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    errorMessage = "You are required to log in through the Credentials tab in application's settings before you can delete records.";
                }
                else
                {
                    errorMessage = String.Format("An error occurred - {0}.", e.Message);
                }
                _dialogService.ShowError(errorMessage);
            }            
        }

        private Boolean CanDeleteSelectedSubscriber()
        {
            return SelectedSubscriber != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var tempPropertyChanged = PropertyChanged;
            if (tempPropertyChanged != null)
            {
                tempPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}