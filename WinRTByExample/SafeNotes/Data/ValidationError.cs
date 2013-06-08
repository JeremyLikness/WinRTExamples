// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationError.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The validation error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System.Collections.Generic;

    using SafeNotes.Common;

    /// <summary>
    /// The validation error.
    /// </summary>
    public class ValidationError : BindableBase 
    {
        /// <summary>
        /// The validation errors.
        /// </summary>
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.validationErrors.Count < 1;
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string fieldName]
        {
            get
            {
                return this.validationErrors.ContainsKey(fieldName) ? 
                    this.validationErrors[fieldName] : string.Empty;
            }

            set
            {
                if (this.validationErrors.ContainsKey(fieldName))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Remove(fieldName);
                    }
                    else
                    {
                        this.validationErrors[fieldName] = value;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Add(fieldName, value);
                    }
                }

                this.OnPropertyChanged();
// ReSharper disable ExplicitCallerInfoArgument
                this.OnPropertyChanged("IsValid");
// ReSharper restore ExplicitCallerInfoArgument
            }
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            var keyList = new string[this.validationErrors.Count];
            this.validationErrors.Keys.CopyTo(keyList, 0);
            
            foreach (var key in keyList)
            {
                this[key] = string.Empty;
            }
        }
    }
}