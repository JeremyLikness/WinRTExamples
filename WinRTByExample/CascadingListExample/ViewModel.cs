// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the ViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CascadingListExample
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CascadingListExample.Common;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : BindableBase 
    {
        /// <summary>
        /// List of digits
        /// </summary>
        private readonly List<int> digits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// The selected one.
        /// </summary>
        private int selectedOne;

        /// <summary>
        /// The selected ten.
        /// </summary>
        private int selectedTen;

        /// <summary>
        /// The selected hundred.
        /// </summary>
        private int selectedHundred;

        /// <summary>
        /// The selected thousand.
        /// </summary>
        private int selectedThousand;

        /// <summary>
        /// The number.
        /// </summary>
        private int number;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.Tens = new ObservableCollection<int>();
            this.Hundreds = new ObservableCollection<int>();
            this.Thousands = new ObservableCollection<int>();
            this.LoadTens();
            this.LoadHundreds();
            this.LoadThousands();
        }

        /// <summary>
        /// Gets or sets the selected one.
        /// </summary>
        public int SelectedOne
        {
            get
            {
                return this.selectedOne;
            }

            set
            {
                this.SetProperty(ref this.selectedOne, value);
                this.LoadTens();
            }
        }

        /// <summary>
        /// Gets or sets the selected ten.
        /// </summary>
        public int SelectedTen
        {
            get
            {
                return this.selectedTen;
            }
            
            set
            {
                this.SetProperty(ref this.selectedTen, value);  
                this.LoadHundreds();
            }
        }

        /// <summary>
        /// Gets or sets the selected hundred.
        /// </summary>
        public int SelectedHundred
        {
            get
            {
                return this.selectedHundred;
            }

            set
            {
                this.SetProperty(ref this.selectedHundred, value);
                this.LoadThousands();
            }
        }

        /// <summary>
        /// Gets or sets the selected thousand.
        /// </summary>
        public int SelectedThousand
        {
            get
            {
                return this.selectedThousand;
            }
            
            set
            {
                this.SetProperty(ref this.selectedThousand, value);
                this.Number = value;
            }
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Number
        {
            get
            {
                return this.number;
            }
            
            set
            {
                this.SetProperty(ref this.number, value);
            }
        }

        /// <summary>
        /// Gets the digits.
        /// </summary>
        public List<int> Digits
        {
            get
            {
                return this.digits;
            }
        }

        /// <summary>
        /// Gets the tens.
        /// </summary>
        public ObservableCollection<int> Tens { get; private set; }

        /// <summary>
        /// Gets the hundreds.
        /// </summary>
        public ObservableCollection<int> Hundreds { get; private set; }

        /// <summary>
        /// Gets the thousands.
        /// </summary>
        public ObservableCollection<int> Thousands { get; private set; }

        /// <summary>
        /// The get list.
        /// </summary>
        /// <param name="starter">
        /// The starter.
        /// </param>
        /// <returns>
        /// The list of digits starting at the starting value
        /// </returns>
        private IEnumerable<int> GetList(int starter)
        {
            var numbers = new List<int>();
            for (var offset = 0; offset < 10; offset++)
            {
                numbers.Add(starter + offset);
            }

            return numbers;
        }

        /// <summary>
        /// The load service.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        /// <param name="setter">
        /// The setter.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        private void LoadService(int selected, Action<int> setter, IList<int> list)
        {
            var starter = selected * 10;

            if (list.Contains(starter))
            {
                return;
            }

            list.Clear();
            foreach (var newNumber in this.GetList(starter))
            {
                list.Add(newNumber);
            }

            if (!list.Contains(selected))
            {
                setter(list[0]);
            }
        }

        /// <summary>
        /// The load tens.
        /// </summary>
        private void LoadTens()
        {
            this.LoadService(this.selectedOne, value => this.SelectedTen = value, this.Tens);
        }

        /// <summary>
        /// The load tens.
        /// </summary>
        private void LoadHundreds()
        {
            this.LoadService(this.selectedTen, value => this.SelectedHundred = value, this.Hundreds);
        }

        /// <summary>
        /// The load tens.
        /// </summary>
        private void LoadThousands()
        {
            this.LoadService(this.selectedHundred, value => this.SelectedThousand = value, this.Thousands);
        }
    }
}