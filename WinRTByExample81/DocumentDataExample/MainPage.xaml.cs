// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Example of building up document data
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DocumentDataExample
{
    using System;
    using System.Linq;

    using Windows.Storage;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Main page for app
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPageLoaded;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// The main page_ loaded event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var text = await PathIO.ReadTextAsync("ms-appx:///LoremIpsum.txt");
            var options = text.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var fonts = await PathIO.ReadLinesAsync("ms-appx:///Fonts.txt");

            var idx = 0;
            var random = new Random();
            for (var x = 0; x < 100; x++)
            {
                var regularRun = new Run { Text = options[idx++ % options.Length] };
                var boldRun = new Run { Text = options[idx++ % options.Length] };
                var italicRun = new Run { Text = options[idx++ % options.Length] };
                var underlineRun = new Run { Text = options[idx++ % options.Length] };

                var fontFamily = new FontFamily(fonts[random.Next(fonts.Count())]);

                var p = new Paragraph
                {
                    FontSize = 9.0 + (random.NextDouble() * 10.0),
                    FontFamily = fontFamily
                };

                p.Inlines.Add(regularRun);
                var bold = new Bold();
                bold.Inlines.Add(boldRun);
                p.Inlines.Add(bold);
                var italic = new Italic();
                italic.Inlines.Add(italicRun);
                p.Inlines.Add(italic);
                var underline = new Underline();
                underline.Inlines.Add(underlineRun);
                p.Inlines.Add(underline);

                MainText.Blocks.Add(p);
            }
        }
    }
}
