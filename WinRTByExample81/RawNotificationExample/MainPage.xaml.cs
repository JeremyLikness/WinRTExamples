namespace RawNotificationExample
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.ApplicationModel.Background;
    using Windows.Networking.PushNotifications;
    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Navigation;

    using LockScreenTasks;

    using WinRTByExample.NotificationHelper.Badges;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string TaskName = "Background Push Notification";

        private readonly string messageKey = RawNotificationTask.GetMessageKey();

        private bool initialized = false;

        public MainPage()
        {
            this.InitializeComponent();            
        }

        private string RemoteText
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey(messageKey)
                    ? ApplicationData.Current.LocalSettings.Values[messageKey].ToString() 
                    : "waiting...";
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            BadgeHelper.ClearBadge().Set();
            ApplicationData.Current.LocalSettings.Values[RawNotificationTask.GetCountKey()] = 0;

            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            if (!string.IsNullOrWhiteSpace(App.ChannelError))
            {
                StatusText.Text = App.ChannelError;
                ChannelBox.Text = "N/A";
            }
            else
            {
                StatusText.Text = "Channel retrieved.";
                ChannelBox.Text = App.Channel.Uri;
                App.Channel.PushNotificationReceived += this.ChannelPushNotificationReceived;
                await this.ConfigureBackgroundTask();
                Remote.Text = RemoteText;            
            }
        }

        private async Task ConfigureBackgroundTask()
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Denied)
            {
                StatusText.Text = "Lock screen access was denied.";
                return;
            }

            if (BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == TaskName))
            {
                StatusText.Text = "Found background task.";
                var task = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == TaskName).Value;
                task.Completed += this.TaskCompleted;
            }
            else
            {
                try
                {
                    var builder = new BackgroundTaskBuilder
                                      {
                                          Name = TaskName,
                                          TaskEntryPoint = "LockScreenTasks.RawNotificationTask"
                                      };
                    builder.SetTrigger(new PushNotificationTrigger());
                    var task = builder.Register();
                    task.Completed += this.TaskCompleted;
                    StatusText.Text = "Registered background task."; 
                }
                catch (Exception ex)
                {
                    StatusText.Text = string.Format("Registration failed: {0}", ex.Message);
                }
            }
        }

        private async void TaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            await Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        StatusText.Text = string.Format("Received remote raw notification at {0}", DateTime.Now);
                        Remote.Text = RemoteText;
                    });
        }

        private async void ChannelPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (args != null && args.RawNotification != null)
            {
                args.Cancel = true;
                await Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                        {
                            StatusText.Text = string.Format("Received local raw notification at {0}", DateTime.Now);
                            Local.Text = string.Format("{0}: {1}", DateTime.Now, args.RawNotification.Content);
                        });
            }
        }
    }
}
