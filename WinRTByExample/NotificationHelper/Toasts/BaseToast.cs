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

    using Windows.Data.Xml.Dom;
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
        /// Set the toast to have no audio
        /// </summary>
        /// <returns>
        /// The <see cref="BaseToast"/>.
        /// </returns>
        public BaseToast WithNoAudio()
        {
            var audio = this.Xml.CreateElement("audio");
            audio.SetAttribute("silent", "true");
            var selectSingleNode = this.Xml.SelectSingleNode("/toast");
            if (selectSingleNode != null)
            {
                selectSingleNode.AppendChild(audio);
            }

            return this;
        }

        /// <summary>
        /// The with audio.
        /// </summary>
        /// <param name="audioType">
        /// The audio type.
        /// </param>
        /// <returns>
        /// The <see cref="BaseToast"/>.
        /// </returns>
        public BaseToast WithAudio(AudioType audioType)
        {
            var audio = this.Xml.SelectSingleNode("toast/audio") as XmlElement ?? this.Xml.CreateElement("audio");
            audio.SetAttribute("src", string.Format("ms-winsoundevent:{0}", audioType.FullType));
            audio.SetAttribute("loop", "false");
            
            var toastNode = this.Xml.SelectSingleNode("/toast");

            if (toastNode != null)
            {
                toastNode.AppendChild(audio);
            }
            else
            {
                throw new InvalidOperationException("Unable to access the toast element.");
            }

            return this;
        }

        /// <summary>
        /// The with audio.
        /// </summary>
        /// <param name="audioType">
        /// The audio type.
        /// </param>
        /// <returns>
        /// The <see cref="BaseToast"/>.
        /// </returns>
        public BaseToast WithLoopingAudio(AudioLoopType audioType)
        {
            var audio = this.Xml.SelectSingleNode("toast/audio") as XmlElement ?? this.Xml.CreateElement("audio");
            audio.SetAttribute("src", string.Format("ms-winsoundevent:{0}", audioType.FullType));
            audio.SetAttribute("loop", "true");

            var toastNode = this.Xml.SelectSingleNode("/toast") as XmlElement;

            if (toastNode != null)
            {
                toastNode.SetAttribute("duration", "long");
                toastNode.AppendChild(audio);
            }
            else
            {
                throw new InvalidOperationException("Unable to access the toast element.");
            }

            return this;
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
            var visual = this.Xml.GetElementsByTagName("toast")[0];
            var launch = this.Xml.CreateAttribute("launch");
            launch.NodeValue = args;
            visual.Attributes.SetNamedItem(launch);
            return this;
        }

        /// <summary>
        /// The send method
        /// </summary>
        /// <returns>
        /// The <see cref="ToastNotification"/>.
        /// </returns>
        public ToastNotification Send()
        {
            var toast = new ToastNotification(this.Xml);
            this.SetExpiration(toast);
            
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
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
