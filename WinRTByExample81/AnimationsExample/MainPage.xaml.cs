// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using System.Collections.Generic;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The _types.
        /// </summary>
        private readonly IAnimationType[] types = {
                                                      new BounceEaseType(),
                                                      new CircleEaseType(),
                                                      new ColorAnimationType(),
                                                      new CubicEaseType(), 
                                                      new ElasticEaseType(), 
                                                      new ExponentialEaseType(), 
                                                      new FadeOutThemeType(),
                                                      new MultipleAnimationType(), 
                                                      new PopOutThemeType(),
                                                      new PowerEaseType(),
                                                      new QuadraticEaseType(), 
                                                      new SineEaseType(), 
                                                      new SwipeOutThemeType()
                                                  };

        /// <summary>
        /// The _animation types.
        /// </summary>
        private readonly List<IAnimationType> animationTypes;

        /// <summary>
        /// The storyboard.
        /// </summary>
        private Storyboard storyboard = new Storyboard();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.animationTypes = new List<IAnimationType>(this.types.OrderBy(t => t.Description));
            this.InitializeComponent();
            this.Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// The main page_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            Animations.ItemsSource = this.animationTypes;
            Animations.SelectedItem = this.animationTypes.First();
            BtnBegin.IsEnabled = true;
            BtnEnd.IsEnabled = false;
        }

        /// <summary>
        /// The animations_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Animations_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                if (this.storyboard.GetCurrentState() != ClockState.Stopped)
                {
                    this.storyboard.Stop();
                    BtnBegin.IsEnabled = true;
                    BtnEnd.IsEnabled = false;
                }

                this.storyboard = e.AddedItems.Cast<IAnimationType>().FirstOrDefault()
                    .GenerateAnimation(this.MainEllipse);
            }
        }

        /// <summary>
        /// The button begin_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnBegin_OnClick(object sender, RoutedEventArgs e)
        {
            this.storyboard.Begin();
            BtnBegin.IsEnabled = false;
            BtnEnd.IsEnabled = true;
        }

        /// <summary>
        /// The button end on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnEnd_OnClick(object sender, RoutedEventArgs e)
        {
            this.storyboard.Stop();
            BtnBegin.IsEnabled = true;
            BtnEnd.IsEnabled = false;
        }
    }
}
