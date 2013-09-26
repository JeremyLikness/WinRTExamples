namespace TileExplorer
{
    using System;
    using System.Linq;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Common;
    using DataModel;
    using WinRTByExample.NotificationHelper.Badges;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Badges
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public Badges()
        {
            this.InitializeComponent();
            this.DefaultViewModel["Items"] = App.CurrentDataSource.Badges;        
        }

        private async void SetBadge_OnClick(object sender, RoutedEventArgs e)
        {
            var numeric = GlyphOrNumeric.IsOn;
            var title = "Success";
            string message;

            try
            {
                if (numeric)
                {
                    var badgeValue = (int)NumericSlider.Value;
                    badgeValue.GetBadge().Set();
                    message = "Numeric badge was set.";
                }
                else
                {
                    var item = this.Glyphs.SelectedItem as BadgeItem ?? App.CurrentDataSource.Badges.First();
                    item.Badge.Set();
                    message = "Glyph badge was set.";
                }
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
            this.Frame.GoBack();
        }

        private async void ClearBadge_OnClick(object sender, RoutedEventArgs e)
        {
            BadgeHelper.ClearBadge().Set();
            var dialog = new MessageDialog("The badge was cleared.");
            await dialog.ShowAsync();
            this.Frame.GoBack();
        }
    }
}
