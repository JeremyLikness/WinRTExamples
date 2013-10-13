// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClockViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The clock view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewModelLocatorExample
{
    using System;
    using System.Threading.Tasks;

    using Common;

    /// <summary>
    /// The clock view model.
    /// </summary>
    public class ClockViewModel : BindableBase, IClockViewModel
    {        
        /// <summary>
        /// The time.
        /// </summary>
        private DateTime time;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockViewModel"/> class.
        /// </summary>
        public ClockViewModel()
        {
            this.time = DateTime.Now;
            this.TimeTicks();
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return this.time;
            }
         
            set
            {
                this.time = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The time ticks.
        /// </summary>
        private async void TimeTicks()
        {
            while (this.time != DateTime.Now)
            {
                this.Time = DateTime.Now;
                await Task.Delay(1000);
            }
        }
    }
}
