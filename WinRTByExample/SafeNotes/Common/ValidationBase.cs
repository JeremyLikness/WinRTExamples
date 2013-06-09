// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationBase.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The validation base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Common
{
    /// <summary>
    /// The validation base.
    /// </summary>
    public abstract class ValidationBase : BindableBase
    {
            /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBase"/> class.
        /// </summary>
        protected ValidationBase()
        {
            this.ValidationErrors = new ValidationErrors();            
        }

        /// <summary>
        /// Gets or sets the validation errors.
        /// </summary>
        public ValidationErrors ValidationErrors { get; set; }

        /// <summary>
        /// Gets a value indicating whether it is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Validate the class 
        /// </summary>
        public void Validate()
        {
            this.ValidationErrors.Clear();
            this.ValidateSelf();
            this.IsValid = this.ValidationErrors.IsValid;
            // ReSharper disable ExplicitCallerInfoArgument
            this.OnPropertyChanged("IsValid");
            this.OnPropertyChanged("ValidationErrors");
            // ReSharper restore ExplicitCallerInfoArgument
        }

        /// <summary>
        /// The validate self.
        /// </summary>
        protected abstract void ValidateSelf();
    }
}