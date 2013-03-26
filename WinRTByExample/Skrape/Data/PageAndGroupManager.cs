// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageAndGroupManager.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The page and group manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;

    using Skrape.Contracts;

    using Windows.Storage;

    /// <summary>
    /// The page and group manager.
    /// </summary>
    public class PageAndGroupManager : IPageAndGroupManager
    {
        /// <summary>
        /// The html entry.
        /// </summary>
        private const string HtmlEntry = "Html.htm";

        /// <summary>
        /// The text entry.
        /// </summary>
        private const string TextEntry = "Text.txt";

        /// <summary>
        /// The zip template.
        /// </summary>
        private const string ZipTemplate = "{0}.zip";

        /// <summary>
        /// The page key.
        /// </summary>
        private const string PageKey = "Pages";

        /// <summary>
        /// The group key.
        /// </summary>
        private const string GroupKey = "Groups";

        /// <summary>
        /// The page folder name
        /// </summary>
        private const string PageFolder = "Pages";

        /// <summary>
        /// The id property.
        /// </summary>
        private const string IdProperty = "Id";

        /// <summary>
        /// The title property.
        /// </summary>
        private const string TitleProperty = "Title";

        /// <summary>
        /// The thumbnail property
        /// </summary>
        private const string ThumbnailProperty = "ThumbnailPath";

        /// <summary>
        /// The description property.
        /// </summary>
        private const string DescriptionProperty = "Description";

        /// <summary>
        /// The page count property.
        /// </summary>
        private const string PageCountProperty = "Pages";

        /// <summary>
        /// The page loaded property
        /// </summary>
        private const string LoadedProperty = "Loaded";

        /// <summary>
        /// The page index.
        /// </summary>
        private const string PageIndex = "Page";

        /// <summary>
        /// The url property.
        /// </summary>
        private const string UrlProperty = "Url";

        /// <summary>
        /// The image count property.
        /// </summary>
        private const string ImageCountProperty = "Images";

        /// <summary>
        /// The image index.
        /// </summary>
        private const string ImageIndex = "Image";

        /// <summary>
        /// High priority roaming setting
        /// </summary>
        private const string HighPriority = "HighPriority";

        /// <summary>
        /// the current URI
        /// </summary>
        private Uri currentUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageAndGroupManager"/> class.
        /// </summary>
        public PageAndGroupManager()
        {
            ApplicationData.Current.DataChanged += this.CurrentDataChanged;
            this.currentUri = null;
        }

        /// <summary>
        /// The current page changed.
        /// </summary>
        public event EventHandler<Uri> NewUriAdded = delegate { };
                
        /// <summary>
        /// Gets the current synchronized page
        /// </summary>
        private static Uri CurrentUri
        {
            get
            {
                if (Roaming.Values.ContainsKey(HighPriority))
                {
                    Uri uri;
                    if (Uri.TryCreate(Roaming.Values[HighPriority].ToString(), UriKind.Absolute, out uri))
                    {
                        return uri;
                    }
                }

                return null;
            }
        }
           
        /// <summary>
        /// Gets the roaming.
        /// </summary>
        private static ApplicationDataContainer Roaming
        {
            get
            {
                return ApplicationData.Current.RoamingSettings;
            }
        }

        /// <summary>
        /// Gets the local.
        /// </summary>
        private static StorageFolder Local
        {
            get
            {
                return ApplicationData.Current.LocalFolder;
            }
        }

        /// <summary>
        /// Gets the local settings
        /// </summary>
        private static ApplicationDataContainer LocalSettings
        {
            get
            {
                return ApplicationData.Current.LocalSettings;
            }
        }

        /// <summary>
        /// The set current page.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        public void AddUri(Uri uri)
        {
            Roaming.Values[HighPriority] = uri == null ? 
                string.Empty : uri.ToString();
        }

        /// <summary>
        /// The save page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> for asynchronous saving.
        /// </returns>
        public async Task SavePage(SkrapedPage page)
        {
            // save loaded status locally, so remote machines will load on first use
            LocalSettings.Values["Page" + page.Id] = page.Loaded;

            var compositeValue = new ApplicationDataCompositeValue
                                        {
                                            { IdProperty, page.Id },
                                            { TitleProperty, page.Title },
                                            { ThumbnailProperty, page.ThumbnailPath.ToString() },
                                            { UrlProperty, page.Url.ToString() },
                                            { ImageCountProperty, page.Images.Count() }                                         
                                        };

            for (var idx = 0; idx < page.Images.Count(); idx++)
            {
                compositeValue.Add(ImageIndex + idx, page.Images[idx].ToString());
            }

            var container = Roaming.CreateContainer(
                PageKey, 
                ApplicationDataCreateDisposition.Always);
            container.Values[page.Id.ToString()] = compositeValue;
            await SavePageData(page);
        }

        /// <summary>
        /// The delete page method.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public void DeletePage(SkrapedPage page)
        {
            var container = Roaming.CreateContainer(
                PageKey,
                ApplicationDataCreateDisposition.Always);
            if (container.Values.ContainsKey(page.Id.ToString()))
            {
                container.Values.Remove(page.Id.ToString());
            }          
        }

        /// <summary>
        /// The save group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        public Task SaveGroup(SkrapeGroup group)
        {
            return Task.Run(
                () =>
                    {
                        var compositeValue = new ApplicationDataCompositeValue
                                                 {
                                                     { IdProperty, @group.Id },
                                                     { TitleProperty, @group.Title },
                                                     {
                                                         DescriptionProperty,
                                                         @group.Description
                                                     },
                                                     {
                                                         PageCountProperty,
                                                         @group.Pages.Count()
                                                     }
                                                 };
                        for (var idx = 0; idx < group.Pages.Count(); idx++)
                        {
                            compositeValue.Add(PageIndex + idx, group.Pages[idx].Id);
                        }

                        var container = Roaming.CreateContainer(GroupKey, ApplicationDataCreateDisposition.Always);
                        container.Values[group.Id.ToString()] = compositeValue;
                    });
        }

        /// <summary>
        /// The delete group method.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        public void DeleteGroup(SkrapeGroup group)
        {
            foreach (var page in group.Pages)
            {
                this.DeletePage(page);
            }

            var container = Roaming.CreateContainer(GroupKey, ApplicationDataCreateDisposition.Always);
            if (container.Values.ContainsKey(group.Id.ToString()))
            {
                container.Values.Remove(group.Id.ToString());
            }
        }

        /// <summary>
        /// The restore groups.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> for asynchronous operation.
        /// </returns>
        public async Task<IEnumerable<SkrapeGroup>> RestoreGroups()
        {
            var groups = new List<SkrapeGroup>();

            var groupsContainer = Roaming.CreateContainer(GroupKey, ApplicationDataCreateDisposition.Always);
            foreach (var group in groupsContainer.Values)
            {
                var restoredGroup = new SkrapeGroup { Id = int.Parse(@group.Key) };
                var compositeValue = group.Value as ApplicationDataCompositeValue;
                if (compositeValue != null)
                {
                    restoredGroup.Title = compositeValue[TitleProperty].ToString();
                    restoredGroup.Description = compositeValue[DescriptionProperty].ToString();
                    var pageCount = (int)compositeValue[PageCountProperty];
                    for (var idx = 0; idx < pageCount; idx++)
                    {
                        var pageId = (int)compositeValue[PageIndex + idx];
                        restoredGroup.Pages.Add(await RestorePage(pageId));
                    }
                }
                else
                {
                    throw new Exception("Error restoring groups.");
                }

                groups.Add(restoredGroup);
            }

            return groups;
        }

        /// <summary>
        /// Restore a page
        /// </summary>
        /// <param name="pageId">The page identifier</param>
        /// <returns>The restored page</returns>
        private static async Task<SkrapedPage> RestorePage(int pageId)
        {
            var restoredPage = new SkrapedPage { Id = pageId };
            var loaded = LocalSettings.Values.ContainsKey(LoadedProperty) 
                && (bool)LocalSettings.Values[LoadedProperty];
            var container = Roaming.CreateContainer(
                PageKey,
                ApplicationDataCreateDisposition.Always);
            var compositeValue = container.Values[pageId.ToString()] as ApplicationDataCompositeValue;
            if (compositeValue != null)
            {
                restoredPage.Url = new Uri(compositeValue[UrlProperty].ToString());
                restoredPage.Title = compositeValue[TitleProperty].ToString();
                restoredPage.ThumbnailPath = loaded
                                                 ? new Uri(compositeValue[ThumbnailProperty].ToString())
                                                 : new Uri("ms-appx:///Assets/ie.png");
                restoredPage.Loaded = loaded;
                var imageCount = (int)compositeValue[ImageCountProperty];
                for (var idx = 0; idx < imageCount; idx++)
                {
                    restoredPage.Images.Add(new Uri(compositeValue[ImageIndex + idx].ToString()));
                }
            }
            else
            {
                throw new Exception("Error restoring page.");
            }

            await RestorePageData(restoredPage);
            return restoredPage;
        }

        /// <summary>
        /// The restore page data.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task RestorePageData(SkrapedPage page)
        {
            var folder = await Local.CreateFolderAsync(PageFolder, CreationCollisionOption.OpenIfExists);
            try
            {
                using (var fileStream = await folder.OpenStreamForReadAsync(string.Format(ZipTemplate, page.Id)))
                {
                    using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                    {
                        var htmlEntry = zipArchive.GetEntry(HtmlEntry);
                        using (var htmlStream = new StreamReader(htmlEntry.Open()))
                        {
                            page.Html = await htmlStream.ReadToEndAsync();
                        }

                        var textEntry = zipArchive.GetEntry(TextEntry);
                        using (var textStream = new StreamReader(textEntry.Open()))
                        {
                            page.Text = await textStream.ReadToEndAsync();
                        }
                    }

                    // using (var decompressor = new Decompressor(fileStream.AsInputStream()))
                    // {
                    //     var decompressionStream = decompressor.AsStreamForRead();
                    //     // read the first 4 bytes to get the size of the entire buffer 
                    //     var sizeBytes = new byte[sizeof(int)];
                    //     await decompressionStream.ReadAsync(sizeBytes, 0, sizeof(int));                            
                    //     var totalSize = BitConverter.ToInt32(sizeBytes, 0);
                    //     byteBuffer = new byte[totalSize];
                    //     await decompressionStream.ReadAsync(byteBuffer, 0, totalSize);
                    //     page.Html = Encoding.UTF8.GetString(byteBuffer, 0, byteBuffer.Length);
                    // }
                }
            }
            catch (FileNotFoundException)
            {
                // empty catch block is because file will simply throw exception if not found
            }
        }

        /// <summary>
        /// The save page data.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> for asynchronous save operations.
        /// </returns>
        private static async Task SavePageData(SkrapedPage page)
        {
            if (string.IsNullOrWhiteSpace(page.Html))
            {
                return;
            }
            
            var folder = await Local.CreateFolderAsync(
                PageFolder,
                CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(string.Format(ZipTemplate, page.Id), CreationCollisionOption.ReplaceExisting);

            using (var zip = new ZipArchive(await file.OpenStreamForWriteAsync(), ZipArchiveMode.Create))
            {
                var htmlEntry = zip.CreateEntry(HtmlEntry);
                using (var htmlStream = new StreamWriter(htmlEntry.Open()))
                {
                    await htmlStream.WriteAsync(page.Html);
                }

                var textEntry = zip.CreateEntry(TextEntry);
                using (var textStream = new StreamWriter(textEntry.Open()))
                {
                    await textStream.WriteAsync(page.Text);
                }
            }

            // using (var compressor = new Compressor(await file.OpenAsync(FileAccessMode.ReadWrite)))
            // {
            //    var htmlBytes = Encoding.UTF8.GetBytes(page.Html);           
            //    await compressor.WriteAsync(BitConverter.GetBytes(htmlBytes.Length).AsBuffer());
            //    await compressor.WriteAsync(htmlBytes.AsBuffer());
            //    await compressor.FinishAsync();
            // }
        }        

        /// <summary>
        /// The current_ data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void CurrentDataChanged(ApplicationData sender, object args)
        {
            var uri = CurrentUri;
            
            if (uri == this.currentUri)
            {
                return;
            }

            this.currentUri = uri;
            
            this.NewUriAdded(this, this.currentUri);
        }
    }
}
