// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTypeConverter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The message type converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TemplatesExample
{
    using System;

    using Windows.UI;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// The message type converter.
    /// </summary>
    public class MessageTypeConverter : IValueConverter 
    {
        /// <summary>
        /// The convert method
        /// </summary>
        /// <param name="value">
        /// The value to convert
        /// </param>
        /// <param name="targetType">
        /// The target type to convert to
        /// </param>
        /// <param name="parameter">
        /// The optional parameter.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> of the converted value
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageType)
            {
                var messageType = (MessageType)value;
                if (messageType == MessageType.Information)
                {
                    return new SolidColorBrush(Colors.DarkGreen);
                }

                return messageType == MessageType.Warning ? 
                    new SolidColorBrush(Colors.DarkOrange) : new SolidColorBrush(Colors.DarkRed);
            }

            return new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">This method is not implemented
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
