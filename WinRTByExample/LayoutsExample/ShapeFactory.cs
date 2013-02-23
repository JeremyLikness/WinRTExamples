// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShapeFactory.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The shape factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LayoutsExample
{
    using System;

    /// <summary>
    /// The shape factory.
    /// </summary>
    public static class ShapeFactory
    {
        /// <summary>
        /// The random number generator
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// The types.
        /// </summary>
        private static readonly ShapeType[] Types = new[]
                                               {
                                                   ShapeType.Circle, 
                                                   ShapeType.Ellipse, 
                                                   ShapeType.Rectangle,
                                                   ShapeType.Square
                                               };

        /// <summary>
        /// The generate shape method.
        /// </summary>
        /// <returns>
        /// The <see cref="ShapeInstance"/> generated.
        /// </returns>
        public static ShapeInstance GenerateShape()
        {
            var type = Types[Random.Next(Types.Length)];
            return ShapeInstance.CreateShape(type);
        }
    }
}
