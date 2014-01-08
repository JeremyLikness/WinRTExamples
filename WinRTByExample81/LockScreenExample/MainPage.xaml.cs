using System;
using System.Linq;

using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LockScreenExample
{
    using Windows.ApplicationModel.Background;
    using Windows.UI.Core;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string TimerTask = "Lock Timer Task";
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += async (o, e) =>
                {
                    try
                    {
                        await BackgroundExecutionManager.RequestAccessAsync();
                    }
                    catch (Exception ex)
                    {
                        Status.Text = string.Format("Lock screen request: {0}", ex.Message);
                    }
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == TimerTask))
                {
                    Status.Text = "Already registered.";
                    foreach (var timerTask in
                        BackgroundTaskRegistration.AllTasks.Where(task => task.Value.Name == TimerTask)
                            .Select(task => task.Value))
                    {
                        timerTask.Completed += this.TaskRunCompleted;
                    }
                    return;
                }

                Status.Text = "Registering...";
                var trigger = new TimeTrigger(15, false);
                var builder = new BackgroundTaskBuilder
                                  {
                                      Name = TimerTask,
                                      TaskEntryPoint = "LockScreenTasks.LockTimer"
                                  };
                builder.SetTrigger(trigger);
                var registration = builder.Register();
                registration.Completed += this.TaskRunCompleted;
                Status.Text = "Registered.";
            }
            catch (Exception ex)
            {
                LastRun.Text = ex.Message;
            }
        }

        private async void TaskRunCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () => LastRun.Text = string.Format("Last Run: {0}", DateTime.Now));
        }  
    }
}
