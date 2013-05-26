// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Toaster.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Toaster.Common;

    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Tiles;
    using WinRTByExample.NotificationHelper.Toasts;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : BindableBase, ICommand  
    {
        /// <summary>
        /// The toasts.
        /// </summary>
        private readonly List<ToastItem> toasts;

        /// <summary>
        /// The images
        /// </summary>
        private readonly string[] images; 

        /// <summary>
        /// The selected item
        /// </summary>
        private ToastItem selectedItem;

        /// <summary>
        /// The selected image
        /// </summary>
        private string selectedImage;

        /// <summary>
        /// First line of notification text
        /// </summary>
        private string textLine1;

        /// <summary>
        /// Second line of notification text
        /// </summary>
        private string textLine2;

        /// <summary>
        /// Third line of notification text
        /// </summary>
        private string textLine3;

        /// <summary>
        /// Delay in seconds
        /// </summary>
        private int delaySeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.Executed = (result, message) => { }; // default handler
            var toastList = ToastHelper.GetToasts();
            this.toasts = toastList.Select(item => new ToastItem(item)).ToList();
            this.selectedItem = this.toasts.First();
            this.images = new[] { "avatar.png", "buildingwind8cover.jpg", "paris.jpg", "slbookcover.png" };
            this.selectedImage = this.images[0];
            this.delaySeconds = 15;         
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        public Action<bool, string> Executed { get; set; }

        /// <summary>
        /// Gets the images
        /// </summary>
        public IEnumerable<string> Images
        {
            get
            {
                return this.images;
            }
        }

        /// <summary>
        /// Gets the toasts.
        /// </summary>
        public IEnumerable<ToastItem> Toasts
        {
            get
            {
                return this.toasts;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public ToastItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                this.selectedItem = value;
                this.OnPropertyChanged();
                // ReSharper disable ExplicitCallerInfoArgument
                this.OnPropertyChanged("ShowLine1");
                this.OnPropertyChanged("ShowLine2");
                this.OnPropertyChanged("ShowLine3");
                this.OnPropertyChanged("ShowImage");
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public string SelectedImage
        {
            get
            {
                return this.selectedImage;
            }

            set
            {
                this.selectedImage = value;
                this.OnPropertyChanged();
            }
        }
       
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public string TextLine1
        {
            get
            {
                return this.textLine1;
            }

            set
            {
                this.textLine1 = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public string TextLine2
        {
            get
            {
                return this.textLine2;
            }

            set
            {
                this.textLine2 = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public string TextLine3
        {
            get
            {
                return this.textLine3;
            }

            set
            {
                this.textLine3 = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the delay in seconds for the notification
        /// </summary>
        public int DelaySeconds
        {
            get
            {
                return this.delaySeconds;
            }

            set
            {
                this.delaySeconds = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether show line 1.
        /// </summary>
        public bool ShowLine1
        {
            get
            {
                return this.SelectedItem.Toast.TextLines > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether show line 2.
        /// </summary>
        public bool ShowLine2
        {
            get
            {
                return this.SelectedItem.Toast.TextLines > 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether show line 3.
        /// </summary>
        public bool ShowLine3
        {
            get
            {
                return this.SelectedItem.Toast.TextLines > 2;
            }
        }

        /// <summary>
        /// Gets a value indicating whether show image.
        /// </summary>
        public bool ShowImage
        {
            get
            {
                return this.SelectedItem.Toast.Images > 0;
            }
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute the send command
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public void Execute(object parameter)
        {
            try
            {
                var toast = this.SelectedItem.GetToast()
                    .WithArguments(this.SelectedItem.Toast.TemplateType);

                if (this.ShowLine1)
                {
                    toast.AddText(this.TextLine1);
                }

                if (this.ShowLine2)
                {
                    toast.AddText(this.TextLine2);
                }

                if (this.ShowLine3)
                {
                    toast.AddText(this.TextLine3);
                }

                BaseTile tile;

                if (this.ShowImage)
                {
                    var image = string.Format("ms-appx:///Assets/{0}", this.SelectedImage);
                    toast.AddImage(image, this.SelectedImage);
                    tile = TileTemplateType.TileSquarePeekImageAndText02.GetTile().AddImage(image, this.SelectedImage);
                }
                else
                {
                    tile = TileTemplateType.TileSquareText02.GetTile();
                }

                tile.WithNoBranding()
                    .WithNotifications()
                    .WithExpiration(TimeSpan.FromSeconds(this.DelaySeconds))
                    .AddText("Toast")
                    .AddText(
                        string.Format(
                            "{0} at {1}", toast.TemplateType, DateTime.Now.Add(TimeSpan.FromSeconds(this.delaySeconds))))
                    .Set();

                toast.ScheduleIn(TimeSpan.FromSeconds(this.DelaySeconds));
                this.Executed(true, string.Empty);
            }
            catch (Exception ex)
            {
                this.Executed(false, ex.Message);
            }
        }        
    }
}