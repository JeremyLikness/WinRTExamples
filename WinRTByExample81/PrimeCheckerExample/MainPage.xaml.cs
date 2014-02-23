namespace PrimeCheckerExample
{
    using System;
    using System.Collections.ObjectModel;

    using Windows.Foundation;
    using Windows.System.Threading;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly ObservableCollection<int> primes = new ObservableCollection<int>();

        public MainPage()
        {
            this.InitializeComponent();                    
        }

        private async void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            primes.Clear();
            Primes.ItemsSource = primes;
            await ThreadPool.RunAsync(ComputePrimes);
            ((Button)sender).IsEnabled = true;
        }

        private async void ComputePrimes(IAsyncAction operation)
        {
            var checker = new WinRtExampleMath.PrimeChecker();

            for (var x = 2; x < 100000; x++)
            {
                if (!checker.IsPrime(x))
                {
                    continue;
                }

                var x1 = x;
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.primes.Add(x1));
            }
        }
    }
}