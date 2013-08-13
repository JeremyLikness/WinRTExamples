// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XamlParsingExample
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The XAML snippet to parse
        /// </summary>
        private const string XamlToParse = "<StackPanel " +
            "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
            "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
            "<Ellipse Width=\"200\" Height=\"200\" Fill=\"Red\"/>" +
            "<TextBlock Text=\"This was parsed from a XAML string constant.\"/>" +
            "</StackPanel>";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The button base on click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args
        /// </param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            var stackPanel = XamlReader.Load(XamlToParse);
            ParserContent.Content = stackPanel;
        }
    }
}
