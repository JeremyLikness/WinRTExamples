namespace Toaster.Data
{
    using WinRTByExample.NotificationHelper.Toasts;

    /// <summary>
    /// A toast item
    /// </summary>
    public class ToastItem
    {
        /// <summary>
        /// Gest the underlying toast helper
        /// </summary>
        public BaseToast Toast { get; set; }
        
        /// <summary>
        /// Gets the identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the Xml
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Creates a new instance of the toast
        /// </summary>
        /// <param name="toast"></param>
        public ToastItem(BaseToast toast)
        {
            this.Toast = toast;
            this.Id = toast.TemplateType;
            this.Description = toast.GetDescription();
            this.Xml = toast.ToString();
        }

        /// <summary>
        /// Gets a fresh toast item to work with
        /// </summary>
        /// <returns>The <see cref="BaseToast"/></returns>
        public BaseToast GetToast()
        {
            return new BaseToast(this.Toast.Type);
        }
    }
}