// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace DataFormatsExample
{
    using System;

    using Windows.Data.Json;
    using Windows.Data.Xml.Dom;
    using Windows.Data.Xml.Xsl;
    using Windows.Storage;
    using Windows.UI.Xaml;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The books.
        /// </summary>
        private readonly Uri booksUri = new Uri("ms-appx:///Books.xml");

        /// <summary>
        /// The books XSLT 
        /// </summary>
        private readonly Uri booksXslt = new Uri("ms-appx:///Books.xslt");

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The parse book.
        /// </summary>
        /// <param name="book">
        /// The book.
        /// </param>
        /// <returns>
        /// The <see cref="JsonObject"/>.
        /// </returns>
        private static JsonObject ParseBook(XmlElement book)
        {
            var obj = new JsonObject { { "id", JsonValue.CreateStringValue(book.GetAttribute("id")) } };
            foreach (var node in book.ChildNodes)
            {
                var element = node as XmlElement;
                if (element != null)
                {
                    obj.Add(element.NodeName, JsonValue.CreateStringValue(element.InnerText));
                }
            }

            return obj;
        }

        /// <summary>
        /// The parse_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void Parse_OnClick(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(booksUri);
            var xml = await XmlDocument.LoadFromFileAsync(file);

            var json = new JsonObject();
            XmlElement catalog = xml.DocumentElement;
            if (catalog != null)
            {
                var books = new JsonArray();
                foreach (var childNode in catalog.GetElementsByTagName("book"))
                {
                    books.Add(ParseBook((XmlElement)childNode));
                }

                json.Add("catalog", books);
            }

            var jsonText = json.Stringify();
            TransformedText.Text = jsonText;
        }

        /// <summary>
        /// The transform_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void Transform_OnClick(object sender, RoutedEventArgs e)
        {
            var xsltFile = await StorageFile.GetFileFromApplicationUriAsync(booksXslt);
            var xslt = await XmlDocument.LoadFromFileAsync(xsltFile);
            var processor = new XsltProcessor(xslt);
            var xmlFile = await StorageFile.GetFileFromApplicationUriAsync(booksUri);
            var xml = await XmlDocument.LoadFromFileAsync(xmlFile);
            TransformedText.Text = processor.TransformToString(xml.DocumentElement);
        }
    }
}
