// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomVisualStateManager.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The custom visual state manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VisualStateExample
{
    using System.Diagnostics;

    using Windows.UI.Xaml;

    /// <summary>
    /// The custom visual state manager.
    /// </summary>
    public class CustomVisualStateManager : VisualStateManager 
    {
        /// <summary>
        /// The go to state core.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="templateRoot">
        /// The template root.
        /// </param>
        /// <param name="stateName">
        /// The state name.
        /// </param>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="useTransitions">
        /// The use transitions.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool GoToStateCore(
            Windows.UI.Xaml.Controls.Control control, 
            FrameworkElement templateRoot, 
            string stateName, 
            VisualStateGroup group, 
            VisualState state, 
            bool useTransitions)
        {
            Debug.WriteLine(
                "Custom: {0} with template root {1} going to state {2}",
                control.GetType().FullName,
                templateRoot.GetType().FullName,
                stateName);
            return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
        }
    }
}
