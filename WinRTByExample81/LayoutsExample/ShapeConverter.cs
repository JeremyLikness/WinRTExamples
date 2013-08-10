// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShapeConverter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The shape converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LayoutsExample
{
    using System;

    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// The shape converter.
    /// </summary>
    public class ShapeConverter : IValueConverter
    {
        /// <summary>
        /// The random.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// The palette.
        /// </summary>
        private static readonly Color[] Palette = new[]
                                              {
                                                  Colors.Red, 
                                                  Colors.Orange, 
                                                  Colors.Yellow, 
                                                  Colors.Green, 
                                                  Colors.Blue,
                                                  Colors.Indigo, 
                                                  Colors.Violet
                                              };

        /// <summary>
        /// The convert.
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
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var shape = value as ShapeInstance;
            return this.CreateShape(shape);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <exception cref="NotImplementedException">Exception is thrown because this method is not currently implemented.</exception>
        /// <returns>The method is not implemented.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create shape.
        /// </summary>
        /// <param name="shape">
        /// The shape.
        /// </param>
        /// <returns>
        /// The <see cref="FrameworkElement"/> instance of the shape.
        /// </returns>
        private FrameworkElement CreateShape(ShapeInstance shape)
        {
            FrameworkElement uiElement;
            if (shape == null)
            {
                uiElement = new Grid();
                return uiElement;
            }

            if (shape.Type.Equals(ShapeType.Circle) || 
                shape.Type.Equals(ShapeType.Ellipse))
            {
                uiElement = new Ellipse();
            }
            else
            {
                uiElement = new Rectangle();
            }

            uiElement.SetValue(FrameworkElement.HeightProperty, 100.0);

            double width;
            if (shape.Type.Equals(ShapeType.Circle) || shape.Type.Equals(ShapeType.Square))
            {
                width = 100.0;
            }
            else
            {
                width = 200.0;
                uiElement.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 2.0);
            }

            uiElement.SetValue(FrameworkElement.WidthProperty, width);
            uiElement.SetValue(Shape.StrokeThicknessProperty, 5.0);
            uiElement.SetValue(
                Shape.StrokeProperty, 
                new SolidColorBrush(Palette[Random.Next(Palette.Length)]));
            uiElement.SetValue(
                Shape.FillProperty, 
                new SolidColorBrush(Palette[Random.Next(Palette.Length)]));

            return uiElement;
        }
    }
}