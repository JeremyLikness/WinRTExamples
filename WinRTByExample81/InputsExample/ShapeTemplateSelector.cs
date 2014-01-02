using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InputsExample
{
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SquareTemplate { get; set; }
        public DataTemplate EllipseTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var shapeModel = (ShapeModel)item;
            switch (shapeModel.Shape)
            {
                case ShapeModel.ShapeType.Ball:
                    return EllipseTemplate;
                case ShapeModel.ShapeType.Square:
                    return SquareTemplate;
                default:
                    throw new InvalidOperationException("Unexpected shape type.");
            }
        }
    }
}