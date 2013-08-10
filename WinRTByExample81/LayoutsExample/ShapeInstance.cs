// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShapeInstance.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The shape instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LayoutsExample
{
    using System;

    /// <summary>
    /// The shape instance.
    /// </summary>
    public class ShapeInstance
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ShapeInstance"/> class from being created.
        /// </summary>
        private ShapeInstance()
        {            
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ShapeType Type { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The create shape method.
        /// </summary>
        /// <param name="type">
        /// The type of shape to create.
        /// </param>
        /// <returns>
        /// The <see cref="ShapeInstance"/> created.
        /// </returns>
        public static ShapeInstance CreateShape(ShapeType type)
        {
            var shape = new ShapeInstance
            {
                Type = type,
                Description = Enum.GetName(typeof(ShapeType), type)
            };
            return shape;
        }
    }
}
