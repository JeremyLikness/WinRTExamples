// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Data source
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SyndicationExample.DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Web.Syndication;

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// SingleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// Author signature
        /// </summary>
        private const string AuthorSignature = "by Jeremy Likness";

        /// <summary>
        /// The _sample data source.
        /// </summary>
        private static readonly DataSource SingleDataSource = new DataSource();

        /// <summary>
        /// The c sharper image uri.
        /// </summary>
        private static readonly Uri CSharperImageUri = new Uri(
            "http://feeds.feedburner.com/CSharperImage/", UriKind.Absolute);

        /// <summary>
        /// The _all groups.
        /// </summary>
        private readonly ObservableCollection<DataFeed> allGroups = new ObservableCollection<DataFeed>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        public DataSource()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.PopulateSampleData();
            }
        }

        /// <summary>
        /// Gets the all groups.
        /// </summary>
        public ObservableCollection<DataFeed> AllGroups
        {
            get
            {
                return this.allGroups;
            }
        }

        /// <summary>
        /// The load syndicated content.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        public static async Task LoadSyndicatedContent()
        {
            var client = new SyndicationClient();
            var feed = await client.RetrieveFeedAsync(CSharperImageUri);

            var group = new DataFeed(
                feed.Id, feed.Title.Text, AuthorSignature, feed.ImageUri.ToString(), feed.Subtitle.Text);

            SingleDataSource.allGroups.Add(group);

            var idx = 0;
            var urls = new[] { "DarkGray.png", "LightGray.png", "MediumGray.png" };

            foreach (var dataItem in 
                from item in feed.Items
                let content = Windows.Data.Html.HtmlUtilities.ConvertToText(item.Content.Text)
                let summary = string.Format("{0} ...", content.Length > 255 ? content.Substring(0, 255) : content)
                select
                    new DataItem(
                    item.Id, 
                    item.Title.Text, 
                    AuthorSignature, 
                    string.Format("ms-appx:///Assets/{0}", urls[idx++ % urls.Length]), 
                    summary, 
                    content, 
                    @group))
            {
                @group.Items.Add(dataItem);
            }
        }

        /// <summary>
        /// The get group.
        /// </summary>
        /// <returns>
        /// The <see cref="DataFeed"/>.
        /// </returns>
        public static DataFeed GetGroup()
        {
            // Simple linear search is acceptable for small data sets
            return SingleDataSource.AllGroups.FirstOrDefault();
        }

        /// <summary>
        /// The get item.
        /// </summary>
        /// <param name="uniqueId">
        /// The unique id.
        /// </param>
        /// <returns>
        /// The <see cref="DataItem"/>.
        /// </returns>
        public static DataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            return
                SingleDataSource.AllGroups.SelectMany(group => @group.Items)
                                .FirstOrDefault(item => item.UniqueId.Equals(uniqueId));
        }

        /// <summary>
        /// The populate sample data.
        /// </summary>
        private void PopulateSampleData()
        {
            var itemContent = string.Format(
                "Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}", 
                "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new DataFeed(
                "Group-1", 
                "Group Title: 1", 
                "Group Subtitle: 1", 
                "Assets/DarkGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group1.Items.Add(
                new DataItem(
                    "Group-1-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group1));

            group1.Items.Add(
                new DataItem(
                    "Group-1-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group1));

            group1.Items.Add(
                new DataItem(
                    "Group-1-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group1));

            group1.Items.Add(
                new DataItem(
                    "Group-1-Item-4", 
                    "Item Title: 4", 
                    "Item Subtitle: 4", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group1));

            group1.Items.Add(
                new DataItem(
                    "Group-1-Item-5", 
                    "Item Title: 5", 
                    "Item Subtitle: 5", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group1));
            this.AllGroups.Add(group1);

            var group2 = new DataFeed(
                "Group-2", 
                "Group Title: 2", 
                "Group Subtitle: 2", 
                "Assets/LightGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group2.Items.Add(
                new DataItem(
                    "Group-2-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group2));
            group2.Items.Add(
                new DataItem(
                    "Group-2-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group2));
            group2.Items.Add(
                new DataItem(
                    "Group-2-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group2));
            this.AllGroups.Add(group2);

            var group3 = new DataFeed(
                "Group-3", 
                "Group Title: 3", 
                "Group Subtitle: 3", 
                "Assets/MediumGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-4", 
                    "Item Title: 4", 
                    "Item Subtitle: 4", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-5", 
                    "Item Title: 5", 
                    "Item Subtitle: 5", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-6", 
                    "Item Title: 6", 
                    "Item Subtitle: 6", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            group3.Items.Add(
                new DataItem(
                    "Group-3-Item-7", 
                    "Item Title: 7", 
                    "Item Subtitle: 7", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group3));
            this.AllGroups.Add(group3);

            var group4 = new DataFeed(
                "Group-4", 
                "Group Title: 4", 
                "Group Subtitle: 4", 
                "Assets/LightGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-4", 
                    "Item Title: 4", 
                    "Item Subtitle: 4", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-5", 
                    "Item Title: 5", 
                    "Item Subtitle: 5", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            group4.Items.Add(
                new DataItem(
                    "Group-4-Item-6", 
                    "Item Title: 6", 
                    "Item Subtitle: 6", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group4));
            this.AllGroups.Add(group4);

            var group5 = new DataFeed(
                "Group-5", 
                "Group Title: 5", 
                "Group Subtitle: 5", 
                "Assets/MediumGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group5.Items.Add(
                new DataItem(
                    "Group-5-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group5));
            group5.Items.Add(
                new DataItem(
                    "Group-5-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group5));
            group5.Items.Add(
                new DataItem(
                    "Group-5-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group5));
            group5.Items.Add(
                new DataItem(
                    "Group-5-Item-4", 
                    "Item Title: 4", 
                    "Item Subtitle: 4", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group5));
            this.AllGroups.Add(group5);

            var group6 = new DataFeed(
                "Group-6", 
                "Group Title: 6", 
                "Group Subtitle: 6", 
                "Assets/DarkGray.png", 
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-1", 
                    "Item Title: 1", 
                    "Item Subtitle: 1", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-2", 
                    "Item Title: 2", 
                    "Item Subtitle: 2", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-3", 
                    "Item Title: 3", 
                    "Item Subtitle: 3", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-4", 
                    "Item Title: 4", 
                    "Item Subtitle: 4", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-5", 
                    "Item Title: 5", 
                    "Item Subtitle: 5", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-6", 
                    "Item Title: 6", 
                    "Item Subtitle: 6", 
                    "Assets/MediumGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-7", 
                    "Item Title: 7", 
                    "Item Subtitle: 7", 
                    "Assets/DarkGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            group6.Items.Add(
                new DataItem(
                    "Group-6-Item-8", 
                    "Item Title: 8", 
                    "Item Subtitle: 8", 
                    "Assets/LightGray.png", 
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.", 
                    itemContent, 
                    group6));
            this.AllGroups.Add(group6);
        }
    }
}