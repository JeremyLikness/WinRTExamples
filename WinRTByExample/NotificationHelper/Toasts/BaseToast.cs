// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseToast.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base toast.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Toasts
{
    using System;

    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Common;

    /// <summary>
    /// The base tile.
    /// </summary>
    public class BaseToast : BaseNotification<BaseToast>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseToast"/> class.
        /// </summary>
        /// <param name="templateType">
        /// The template Type.
        /// </param>
        public BaseToast(ToastTemplateType templateType) : 
            base(ToastNotificationManager.GetTemplateContent(templateType), templateType.ToString())
        {
            this.Type = templateType;            
        }
        
        /// <summary>
        /// Gets the tile template type
        /// </summary>
        public ToastTemplateType Type { get; private set; }

        /// <summary>
        /// Expiration setter
        /// </summary>
        protected override Action<object, DateTime> ExpirationSetter
        {
            get
            {
                return (obj, expires) => ((ToastNotification)obj).ExpirationTime = expires;
            }
        }

        /// <summary>
        /// Pass arguments to the toast
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="BaseToast"/>.
        /// </returns>
        public BaseToast WithArguments(string args)
        {
            var visual = this.Xml.GetElementsByTagName("visual")[0];
            var launch = this.Xml.CreateAttribute("launch");
            launch.NodeValue = args;
            visual.Attributes.SetNamedItem(launch);
            return this;
        }

        /// <summary>
        /// The send method
        /// </summary>
        public void Send()
        {
            var toast = new ToastNotification(this.Xml);
            this.SetExpiration(toast);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        /// <summary>
        /// Schedule the toast at a given time
        /// </summary>
        /// <param name="schedule">The scheduled time</param>
        public void ScheduleAt(DateTime schedule)
        {
            var toast = new ScheduledToastNotification(this.Xml, schedule);
            this.SetExpiration(toast);

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }

        /// <summary>
        /// Schedule in a time frame
        /// </summary>
        /// <param name="timeSpan">The time span</param>
        public void ScheduleIn(TimeSpan timeSpan)
        {
            this.ScheduleAt(DateTime.Now.Add(timeSpan));
        }        
    }
}
