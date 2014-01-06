using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LockScreenExample
{
    using System.Threading.Tasks;

    using Windows.ApplicationModel.Background;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string TimerTask = "Lock Timer Task";
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await BackgroundExecutionManager.RequestAccessAsync(); // subsequent calls are ignored 
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status != BackgroundAccessStatus.Denied)
            {
                if (BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == TimerTask))
                {
                    Status.Text = "Already registered.";
                    return;
                }

                Status.Text = "Registering...";
                var builder = new BackgroundTaskBuilder
                                  {
                                      Name = TimerTask,
                                      TaskEntryPoint = "LockScreenExample.LockTimer"
                                  };
                builder.SetTrigger(new TimeTrigger(15, false));
                var registration = builder.Register();
                registration.Completed += this.RegistrationCompleted;
                Status.Text = "Registered.";
            }
            else
            {
                Status.Text = "Access Denied.";
            }
        }

        private void RegistrationCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {

            LastRun.Text = string.Format("Last Run: {0}", DateTime.Now);
        }
    }
}
