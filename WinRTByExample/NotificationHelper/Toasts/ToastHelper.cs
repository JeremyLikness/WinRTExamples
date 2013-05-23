// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToastHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The tile helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Toasts
{
    using System;
    using System.Linq;
    using System.Text;

    using Windows.UI.Notifications;

    /// <summary>
    /// The tile helper.
    /// </summary>
    public static class ToastHelper
    {
        /// <summary>
        /// The base tiles.
        /// </summary>
        private static BaseToast[] baseToasts;

        /// <summary>
        /// The get tiles.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseToast"/> array.
        /// </returns>
        public static BaseToast[] GetToasts()
        {
            return baseToasts
                   ?? (baseToasts =
                       Enum.GetNames(typeof(ToastTemplateType))
                           .Select(toastType => (ToastTemplateType)Enum.Parse(typeof(ToastTemplateType), toastType))
                           .Select(toastTemplateType => new BaseToast(toastTemplateType))
                           .ToArray());
        }

        /// <summary>
        /// The get description.
        /// </summary>
        /// <param name="toast">
        /// The toast.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDescription(this BaseToast toast)
        {
            var sb = new StringBuilder();
            var type = toast.Type.ToString();
            sb.Append(type).Append(" toast with ");
            if (toast.TextLines > 0)
            {
                sb.Append(toast.TextLines == 1 ? "one line of text" : string.Format("{0} lines of text", toast.TextLines));
            }

            if (toast.Images > 0)
            {
                if (toast.TextLines > 0)
                {
                    sb.Append(" and");
                }

                sb.Append(toast.Images == 1 ? " one image" : string.Format(" {0} images", toast.Images));
            }

            sb.Append(".");
            return sb.ToString();
        }

        /// <summary>
        /// The get toast.
        /// </summary>
        /// <param name="toastTemplateType">
        /// The toast template type.
        /// </param>
        /// <returns>
        /// The <see cref="BaseToast"/>.
        /// </returns>
        public static BaseToast GetToast(this ToastTemplateType toastTemplateType)
        {
            return new BaseToast(toastTemplateType);
        }
    }
}