using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LiveConnectExample.Common;
using Microsoft.Live;

namespace LiveConnectExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly IDialogService _dialogService;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        private readonly LiveConnectWrapper _liveConnectWrapper;

        private String _calendarId;
        private Boolean _canUpdateCalendarEvents;
        private readonly ObservableCollection<dynamic> _eventItems = new ObservableCollection<dynamic>();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        public CalendarPage()
        {
            InitializeComponent();

            _dialogService = new DialogService(Dispatcher);
            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;

            DefaultViewModel["RefreshCommand"] = new RelayCommand(Refresh);
            DefaultViewModel["RemoveCommand"] = new RelayCommand(Remove, CanRemove);
            DefaultViewModel["EditCommand"] = new RelayCommand(Edit, CanEdit);
            DefaultViewModel["SaveEditedCommand"] = new RelayCommand(SaveEdited);
            DefaultViewModel["AddCommand"] = new RelayCommand(Add, CanAdd);
            DefaultViewModel["SaveAddedCommand"] = new RelayCommand(SaveAdded);

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            _calendarId = e.Parameter as String;
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;
            DefaultViewModel["ImageSource"] = new Uri("ms-appx:///Assets/Calendar.png");
            DefaultViewModel["EventItems"] = _eventItems;
            DefaultViewModel["SelectedEventItem"] = null;

            _liveConnectWrapper.SessionChanged += OnLiveConnectWrapperSessionChanged;
            await UpdateContent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);

            _liveConnectWrapper.SessionChanged -= OnLiveConnectWrapperSessionChanged;
        }

        #endregion

        private async void OnLiveConnectWrapperSessionChanged(Object sender, EventArgs eventArgs)
        {
            await UpdateContent();
        }

        private async Task UpdateContent()
        {
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;

            if (_liveConnectWrapper.IsSessionAvailable)
            {
                var calendar = await _liveConnectWrapper.GetCalendarAsync(_calendarId);
                DefaultViewModel["Calendar"] = calendar;

                var eventInteractionPermissions = new List<String> {"read_write", "co_owner", "owner"};
                var calendarPermissions = calendar.permissions.ToString();
                _canUpdateCalendarEvents = eventInteractionPermissions.Contains(calendarPermissions);

                var contactItems = new Dictionary<String, Object>(calendar as IDictionary<String, Object>);
                contactItems.Remove("id");
                contactItems.Remove("name");

                var profileItemsList = contactItems.FlattenDynamicItems(String.Empty);
                DefaultViewModel["AdditionalDetails"] = profileItemsList.Select(x => new { x.Key, x.Value }).ToList();

                var calendarEvents = await _liveConnectWrapper.GetCalendarEventsAsync(_calendarId, DateTimeOffset.Now, DateTimeOffset.Now + TimeSpan.FromDays(60));
                var orderedCalendarEvents = new List<dynamic>(calendarEvents).OrderBy(x => x.start_time);
                _eventItems.Clear();
                foreach (var item in orderedCalendarEvents)
                {
                    _eventItems.Add(item);
                }
            }
        }

        private async void Refresh()
        {
            await UpdateContent();
        }

        private Boolean CanRemove()
        {
            return _canUpdateCalendarEvents && DefaultViewModel["SelectedEventItem"] != null;
        }

        private void Remove()
        {
            dynamic itemToRemove = DefaultViewModel["SelectedEventItem"];
            if (itemToRemove == null) return;

            var itemName = itemToRemove.name;

            var removeCommand = new UICommand("Remove", command => HandleRemoveItem(itemToRemove));
            var cancelCommand = new UICommand("Cancel");
            var commands = new[] {removeCommand, cancelCommand};
            _dialogService.ShowMessageBoxAsync("Are you sure you want to remove event " + itemName, "Remove Event?", commands, 1);
        }

        private async void HandleRemoveItem(dynamic itemToRemove)
        {
            try
            {
                await _liveConnectWrapper.DeleteCalendarEventAsync(itemToRemove.id);
                _eventItems.Remove(itemToRemove);
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private Boolean CanEdit()
        {
            return _canUpdateCalendarEvents && DefaultViewModel["SelectedEventItem"] != null;
        }

        private void Edit()
        {
            var eventItem = (dynamic)DefaultViewModel["SelectedEventItem"];

            var startTime = DateTimeOffset.Parse(eventItem.start_time).ToString("g");
            var endTime = DateTimeOffset.Parse(eventItem.end_time).ToString("g");

            DefaultViewModel["EventBeingEdited"] = new CalendarEvent
                                                   {
                                                       Name = eventItem.name,
                                                       StartTime = startTime,
                                                       EndTime = endTime,
                                                       Location = eventItem.location,
                                                   };
        }

        private async void SaveEdited()
        {
            dynamic itemToUpdate = DefaultViewModel["SelectedEventItem"];
            var editedEvent = (CalendarEvent)DefaultViewModel["EventBeingEdited"];

            var startTimeDate = DateTimeOffset.Parse(editedEvent.StartTime).ToString(LiveConnectWrapper.DateTimeFormatString);
            var endTimeDate = DateTimeOffset.Parse(editedEvent.EndTime).ToString(LiveConnectWrapper.DateTimeFormatString);

            var updatedValues = new Dictionary<String, Object>
                                 {
                                     {"name", editedEvent.Name},
                                     {"start_time",  startTimeDate},
                                     {"end_time", endTimeDate},
                                     {"location", editedEvent.Location}
                                 };
            try
            {
                var updatedEvent = await _liveConnectWrapper.UpdateCalendarEventAsync(itemToUpdate.id, updatedValues);

                // Swap pout the selected item with the updated returned item
                var index = _eventItems.IndexOf(itemToUpdate);
                _eventItems[index] = updatedEvent;
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private Boolean CanAdd()
        {
            return _canUpdateCalendarEvents;
        }

        private void Add()
        {
            var now = DateTimeOffset.Now;
            var sampleStartTime = (now + TimeSpan.FromHours(1) - TimeSpan.FromMinutes(now.Minute));
            DefaultViewModel["EventBeingAdded"] = new CalendarEvent
                                                     {
                                                         StartTime = sampleStartTime.ToString("g"),
                                                         EndTime = (sampleStartTime + TimeSpan.FromHours(1)).ToString("g"),
                                                     };
        }

        private async void SaveAdded()
        {
            var addedEvent = (CalendarEvent)DefaultViewModel["EventBeingAdded"];

            var startTimeDate = DateTimeOffset.Parse(addedEvent.StartTime).ToString(LiveConnectWrapper.DateTimeFormatString);
            var endTimeDate = DateTimeOffset.Parse(addedEvent.EndTime).ToString(LiveConnectWrapper.DateTimeFormatString);

            var updatedValues = new Dictionary<String, Object>
                                {
                                    {"name", addedEvent.Name},
                                    {"description", "Example Calendar Event"},
                                    {"start_time", startTimeDate},
                                    {"end_time", endTimeDate},
                                    {"location", addedEvent.Location},
                                    {"is_all_day_event", false},
                                    {"availability", "busy"},
                                    {"visibility", "public"}
                                };
            
            try
            {
                var savedEvent = await _liveConnectWrapper.AddCalendarEventAsync(_calendarId, updatedValues);
                if (savedEvent != null)
                {
                    _eventItems.Add(savedEvent);
                }
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void HandleEventsListingSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ((RelayCommand)DefaultViewModel["RemoveCommand"]).RaiseCanExecuteChanged();
            ((RelayCommand)DefaultViewModel["EditCommand"]).RaiseCanExecuteChanged();
        }
    }

    internal class CalendarEvent
    {
        public String Name { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public String Location { get; set; }
    }
}
