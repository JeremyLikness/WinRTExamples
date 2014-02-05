namespace ThreadPoolExample
{
    using System;
    using System.Threading;

    using Windows.Foundation;
    using Windows.System.Threading;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private IAsyncAction piAsyncAction;

        private ThreadPoolTimer iterationTimer;

        private int hits;

        private int iterations = 1;

        private double PiEstimate
        {
            get
            {
                return 4.0 * (hits / (double)iterations);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Setup();            
        }

        private void Setup()
        {
            iterationTimer = ThreadPoolTimer.CreatePeriodicTimer(this.Iterate, TimeSpan.FromMilliseconds(500));
            piAsyncAction = ThreadPool.RunAsync(this.CalculatePi);
            piAsyncAction.Completed = this.CalculationComplete;
        }

        private void Cancel(bool completed)
        {
            if (piAsyncAction != null)
            {
                if (!completed)
                {
                    piAsyncAction.Cancel();
                }
                piAsyncAction = null;
                iterationTimer.Cancel();
                iterationTimer = null;
            }
        }

        private void Iterate(ThreadPoolTimer timer)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                    {
                        Iterations.Text = iterations.ToString();
                        Pi.Text = PiEstimate.ToString();
                    });
        }

        private void CalculationComplete(IAsyncAction asyncinfo, AsyncStatus asyncstatus)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                    {
                        Pi.Text = string.Format("{0} (final guess)", PiEstimate);
                        this.Cancel(true);
                    });
        }

        private void CalculatePi(IAsyncAction action)
        {
            var random = new Random();
            while (iterations < int.MaxValue && action.Status != AsyncStatus.Canceled)
            {
                var x = random.NextDouble() * 2.0 - 1.0;
                var y = random.NextDouble() * 2.0 - 1.0;
                var distance = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0));
                if (distance < 1.0)
                {
                    Interlocked.Increment(ref hits);
                }
                Interlocked.Increment(ref iterations);
            }
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            this.Cancel(false);
        }
    }
}
