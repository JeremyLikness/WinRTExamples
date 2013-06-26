// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model locator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewModelLocatorExample
{
    /// <summary>
    /// The view model locator.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// The view model.
        /// </summary>
        private IClockViewModel viewModel;

        /// <summary>
        /// Gets the clock view model.
        /// </summary>
        public IClockViewModel ClockViewModel
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return new DesignViewModel();
                }

                return this.viewModel = this.viewModel ?? new ClockViewModel();
            }
        }
    }
}