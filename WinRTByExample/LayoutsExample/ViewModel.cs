// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LayoutsExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel 
    {
        /// <summary>
        /// The shapes.
        /// </summary>
        private readonly List<ShapeInstance> shapes = new List<ShapeInstance>();

        /// <summary>
        /// The states.
        /// </summary>
        private readonly string[] states = new[]
                                               {
                                                   "View List Box", 
                                                   "View Items Control",
                                                   "View List View",
                                                   "View Grid View",
                                                   "View Flip View"
                                               };

        /// <summary>
        /// The stretch.
        /// </summary>
        private readonly string[] stretch = new[]
                                                {
                                                    "None", 
                                                    "Fill", 
                                                    "Uniform", 
                                                    "UniformToFill"
                                                };

        /// <summary>
        /// The current state.
        /// </summary>
        private string state = string.Empty;

        /// <summary>
        /// The current stretch.
        /// </summary>
        private string currentStretch = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            for (var x = 0; x < 100; x++)
            {
                this.shapes.Add(ShapeFactory.GenerateShape());
            }

            if (this.GoToVisualState == null)
            {
                this.GoToVisualState = state => { };                
            }

            if (this.GoToStretch == null)
            {
                this.GoToStretch = stretch => { };
            }

            this.CurrentState = this.states[0];
            this.CurrentStretch = this.stretch[0];
        }

        /// <summary>
        /// Gets or sets the delegate to transition to a visual state
        /// </summary>
        public Action<string> GoToVisualState { get; set; }

        /// <summary>
        /// Gets or sets the delegate to change the stretch mode
        /// </summary>
        public Action<string> GoToStretch { get; set; }

        /// <summary>
        /// Gets the states.
        /// </summary>
        public IEnumerable<string> States
        {
            get
            {
                return this.states;
            }
        }
        
        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public string CurrentState
        {
            get
            {
                return this.state;
            }

            set
            {
                if (string.IsNullOrEmpty(value) || this.state.Equals(value)) 
                {
                    return;
                }

                this.state = value;
                var visualState = string.Format("{0}State", value.Replace(" ", string.Empty));
                this.GoToVisualState(visualState);
            }
        }

        /// <summary>
        /// Gets the stretch modes.
        /// </summary>
        public IEnumerable<string> Stretch
        {
            get
            {
                return this.stretch;
            }
        }

        /// <summary>
        /// Gets or sets the current stretch.
        /// </summary>
        public string CurrentStretch
        {
            get
            {
                return this.currentStretch;
            }

            set
            {
                if (string.IsNullOrEmpty(value) || this.currentStretch.Equals(value))
                {
                    return;
                }

                this.currentStretch = value;
                this.GoToStretch(value);
            }
        }

        /// <summary>
        /// Gets the shapes not grouped.
        /// </summary>
        public IEnumerable<ShapeInstance> ShapesNotGrouped
        {
            get
            {
                return this.shapes;
            }
        }

        /// <summary>
        /// Gets the shapes grouped.
        /// </summary>
        public IOrderedEnumerable<IGrouping<ShapeType, 
            ShapeInstance>> ShapesGrouped
        {
            get
            {
                return this.shapes
                    .GroupBy(shape => shape.Type)
                    .OrderBy(g => g.Key);
            }
        }
    }
}