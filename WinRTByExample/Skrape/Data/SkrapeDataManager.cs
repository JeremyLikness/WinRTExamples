// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkrapeDataManager.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data manager for scrapes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Skrape.Common;
    using Skrape.Contracts;

    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Core;
    using Windows.UI.Popups;

    /// <summary>
    /// The data manager for scrapes.
    /// </summary>
    public class SkrapeDataManager : BasePropertyChange  
    {
        /// <summary>
        /// The group id provider.
        /// </summary>
        private readonly IdProvider groupIdProvider = new IdProvider();

        /// <summary>
        /// The page id provider.
        /// </summary>
        private readonly IdProvider pageIdProvider = new IdProvider();

        /// <summary>
        /// The _groups.
        /// </summary>
        private readonly ObservableCollection<SkrapeGroup> groups = new ObservableCollection<SkrapeGroup>();

        /// <summary>
        /// The image types.
        /// </summary>
        private readonly string[] imageTypes = new[]
                                                   {
                                                       "image/jpeg", "image/jpe", "image/jpg", "image/gif", "image/png",
                                                       "image/bmp", "image/tiff"
                                                   };

        /// <summary>
        /// The image extensions.
        /// </summary>
        private readonly string[] imageExtensions = new[] { "jpg", "jpg", "jpg", "gif", "png", "bmp", "tif" };

        /// <summary>
        /// The current group.
        /// </summary>
        private SkrapeGroup currentGroup;

        /// <summary>
        /// The current page.
        /// </summary>
        private SkrapedPage currentPage;

        /// <summary>
        /// The current image
        /// </summary>
        private Uri currentImage;

        /// <summary>
        /// The current page and group manager
        /// </summary>
        private IPageAndGroupManager pageAndGroupManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkrapeDataManager"/> class.
        /// </summary>
        public SkrapeDataManager()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.GenerateTestData();
            }
        }
        
        /// <summary>
        /// Gets or sets the implementation of the web scraper
        /// </summary>
        public IWebScraper Scraper { get; set; }
        
        /// <summary>
        /// Gets or sets the implementation of the page and group manager
        /// </summary>
        public IPageAndGroupManager Manager
        {
            get
            {
                return this.pageAndGroupManager;
            }

            set
            {
                this.pageAndGroupManager = value;
                if (this.pageAndGroupManager != null)
                {
                    this.pageAndGroupManager.NewUriAdded += this.PageAndGroupManagerOnNewUriAdded;
                }   
            }
        }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        public CoreDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        public ObservableCollection<SkrapeGroup> Groups
        {
            get
            {
                return this.groups;
            }
        }
        
        /// <summary>
        /// Gets or sets the current group.
        /// </summary>
        public SkrapeGroup CurrentGroup
        {
            get
            {
                return this.currentGroup;
            }

            set
            {
                this.currentGroup = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public SkrapedPage CurrentPage
        {
            get
            {
                return this.currentPage;
            }

            set
            {
                this.currentPage = value;
                this.OnPropertyChanged();
                this.CurrentGroup = this.GetGroupForPage(value);
            }
        }

        /// <summary>
        /// Gets or sets the current image.
        /// </summary>
        public Uri CurrentImage
        {
            get
            {
                return this.currentImage;
            }

            set
            {
                this.currentImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        public async Task Initialize()
        {
            if (this.groups.Any())
            {
                return;
            }
            
            foreach (var group in await this.Manager.RestoreGroups())
            {
                this.groups.Add(group);
                this.groupIdProvider.RegisterId(group.Id);
                foreach (var page in group.Pages)
                {
                    this.pageIdProvider.RegisterId(page.Id);
                }
            }

            if (this.groups.Any())
            {
                return;
            }

            await
                this.AddUrl(
                    new Uri("http://csharperimage.jeremylikness.com/2013/02/review-of-lenovo-ideapad-yoga-13-for.html"));
        }

        /// <summary>
        /// The get group for page.
        /// </summary>
        /// <param name="pageToCheck">
        /// The page to check.
        /// </param>
        /// <returns>
        /// The <see cref="SkrapeGroup"/>.
        /// </returns>
        public SkrapeGroup GetGroupForPage(SkrapedPage pageToCheck)
        {
            return this.groups.SelectMany(
                @group => @group.Pages, 
                (@group, page) => new { @group, page })
                .Where(pageAndGroup => pageAndGroup.page.Id == pageToCheck.Id)
                .Select(pageAndGroup => pageAndGroup.group)
                .FirstOrDefault();
        }

        /// <summary>
        /// The add url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        public async Task AddUrl(Uri url)
        {
            var existingPage = this.groups.SelectMany(g => g.Pages, (g, p) => new { g, p })
                                    .Where(@t => !@t.p.Deleted && @t.p.Url == url)
                                    .Select(@t => @t.p).Any();

            if (existingPage)
            {
                return;
            }

            var page = new SkrapedPage
                           {
                               Id = this.pageIdProvider.GetId(),
                               Url = url,
                               Title = "New Skrape - Tap to Load",
                               ThumbnailPath = new Uri("ms-appx:///Assets/ie.png")
                           };

            var domain = url.Host.ToLower();
            var group = this.groups.FirstOrDefault(g => g.Title == domain);

            if (group != null)
            {
                group.Pages.Add(page);
                await this.Manager.SaveGroup(group);
            }
            else
            {
                var newGroup = new SkrapeGroup
                                   {
                                       Id = this.groupIdProvider.GetId(),
                                       Title = domain,
                                       Description =
                                           string.Format("Collection of Skrapes for the {0} domain.", domain)
                                   };
                newGroup.Pages.Add(page);
                this.Groups.Add(newGroup);
                await this.Manager.SaveGroup(newGroup);
            }

            await this.Manager.SavePage(page);

            this.Manager.AddUri(page.Url);
        }

        /// <summary>
        /// The delete page method
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public void DeletePage(SkrapedPage page)
        {
            var parent = this.GetGroupForPage(page);
            page.Deleted = true;
            parent.Pages.Remove(page);

            this.Manager.DeletePage(page);

            if (parent.Pages.Any())
            {
                return;
            }

            this.groups.Remove(parent);
            this.Manager.DeleteGroup(parent);
        }
        
        /// <summary>
        /// The process image routine
        /// </summary>
        /// <param name="imageUri">
        /// The image uri.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Uri> ProcessImage(Uri imageUri, int id)
        {
            try
            {
                // this can be refactored to use the DownloadImage method
                var client = new HttpClient();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, imageUri));
                response.EnsureSuccessStatusCode();
                var buffer = await response.Content.ReadAsByteArrayAsync();
                var type = response.Content.Headers.ContentType;
                var idx = Array.IndexOf(this.imageTypes, type.MediaType);
                string extension;
                if (idx >= 0)
                {
                    extension = this.imageExtensions[idx];
                }
                else
                {
                    return null;
                }

                var file = string.Format("{0}.{1}", id, extension);
                var path = string.Format("ms-appdata:///local/Images/{0}", file);
                var folder =
                    await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    "Images", 
                    CreationCollisionOption.OpenIfExists);
                var storageFile = await folder.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(storageFile, buffer);
                return new Uri(path);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// The save image routine
        /// </summary>
        /// <param name="imageUri">
        /// The image uri.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveImage(Uri imageUri)
        {
            var result = await this.DownloadImage(imageUri);

            if (string.IsNullOrEmpty(result.Extension))
            {
                return;
            }

            var extension = string.Format(".{0}", result.Extension);

            var saveFilePicker = 
                new FileSavePicker
                    {
                        SuggestedFileName = "WebImage",
                        SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                        DefaultFileExtension = extension
                    };
            
            saveFilePicker.FileTypeChoices.Add(
                "Image File", new List<string>(new[] { extension }));

            var storageFile = await saveFilePicker.PickSaveFileAsync();

            if (storageFile == null)
            {
                return;
            }

            CachedFileManager.DeferUpdates(storageFile);
            await FileIO.WriteBytesAsync(storageFile, result.Buffer);
            await CachedFileManager.CompleteUpdatesAsync(storageFile);
        }

        /// <summary>
        /// The download image method.
        /// </summary>
        /// <param name="imageUri">
        /// The image uri.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to return asynchronously.
        /// </returns>
        private async Task<ImageDownloadResult> DownloadImage(Uri imageUri)
        {
            var result = new ImageDownloadResult();
            var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, imageUri));
            response.EnsureSuccessStatusCode();
            result.Buffer = await response.Content.ReadAsByteArrayAsync();
            var type = response.Content.Headers.ContentType;
            var idx = Array.IndexOf(this.imageTypes, type.MediaType);
            if (idx >= 0)
            {
                result.Extension = this.imageExtensions[idx];
            }

            return result;
        }

        /// <summary>
        /// The page and group manager on current page changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="newUri">
        /// The new uri
        /// </param>
        private async void PageAndGroupManagerOnNewUriAdded(object sender, Uri newUri)
        {
            var page = (from g in this.groups from p in g.Pages where p.Url == newUri select p).FirstOrDefault();

            if (page != null)
            {
                return;
            }

            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                    {
                        var dialog = new MessageDialog(
                            "A new URL has been entered on another device. Exit the program and re-launch to synchronize.", newUri.ToString());
                        await dialog.ShowAsync();
                    });
        }

        /// <summary>
        /// The generate test data method.
        /// </summary>
        private void GenerateTestData()
        {
            var baseuri = new Uri("http://csharperimage.jeremylikness.com/", UriKind.Absolute);
            var groupNames = new[] { "Alpha", "Beta", "Omega " };
            foreach (var groupName in groupNames)
            {
                var pages = new[]
                                {
                                    "/2013/02/review-of-lenovo-ideapad-yoga-13-for.html",
                                    "/2010/04/model-view-viewmodel-mvvm-explained.html",
                                    "/2010/10/so-whats-fuss-about-silverlight.html",
                                    "/2013/01/traveling-with-microsoft-and-asus.html"
                                };
                var images = new[]
                                 {
                                     new Uri(
                                         "http://1.bp.blogspot.com/-R7LqKNm9BBk/Tl4oD5P9JdI/AAAAAAAAAZ8/pZ8J8gz_e5E/s1600/BlogHeader4.png"),                                     
                                     new Uri(
                                         "http://lh6.ggpht.com/-YPUxFruIDFA/USFp0raqHAI/AAAAAAAAA6E/bHy3vg05Ed4/picture005_thumb2.jpg?imgmax=800"),                                     
                                     new Uri(
                                         "http://lh6.ggpht.com/-WYkLA5K30mw/UObJc7Ls6bI/AAAAAAAAA3A/2gng-TfN6aE/WP_000232_thumb%25255B1%25255D.jpg?imgmax=800")
                                 };

                var group = new SkrapeGroup();
                group.Id = this.groupIdProvider.GetId();
                group.Title = groupName;
                group.Description =
                    string.Format(
                        "The {0} group for holding a set of test pages for the sample data of the application.",
                        groupName);
                for (var x = 0; x < 50; x++)
                {
                    var pageName = pages[x % pages.Length];
                    var pageUri = new Uri(baseuri, pageName);
                    var page = new SkrapedPage
                                   {
                                       Id = this.pageIdProvider.GetId(), 
                                       Url = pageUri,
                                       Title = pageName.Replace("/", " "),
                                       ThumbnailPath = new Uri("ms-appx:///Assets/Logo.png")
                                   };
                    foreach (var image in images)
                    {
                        page.Images.Add(image);
                    }

                    group.Pages.Add(page);                    
                }

                this.groups.Add(group);                
            }            
        }        
    }
}
