// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Jeremy Likness">
//   Jeremy Likness
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DesktopWinRT
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Principal;
    using System.Windows;

    using Windows.Management.Deployment;

    /// <summary>
    /// Interaction logic for the main window
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += this.MainWindowLoaded;
        }

        /// <summary>
        /// The main window_ loaded event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>        
        protected void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            
            var packageManager = new PackageManager();
            var identity = WindowsIdentity.GetCurrent();

            if (identity == null || identity.User == null)
            {
                throw new Exception("Unable to determine the current user's identity.");
            }

            var query = packageManager.FindPackagesForUser(identity.User.Value);
            foreach (var package in query)
            {
                var name = package.Id.Name;

                try
                {
                    list.Add(string.Format("Package {0} at {1}", name, package.InstalledLocation.Path));
                }
                catch (FileNotFoundException)
                {
                    list.Add(string.Format("Package {0} deleted.", name));
                }
            }

            Packages.ItemsSource = list;
        }
    }
}
