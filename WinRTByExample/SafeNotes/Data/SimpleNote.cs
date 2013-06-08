// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleNote.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The simple note.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System;

    using SafeNotes.Common;

    /// <summary>
    /// The simple note.
    /// </summary>
    public class SimpleNote : BindableBase
    {
        /// <summary>
        /// The title.
        /// </summary>
        private string title;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The date created.
        /// </summary>
        private DateTime dateCreated;

        /// <summary>
        /// The date modified.
        /// </summary>
        private DateTime dateModified;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleNote"/> class.
        /// </summary>
        public SimpleNote()
        {
            this.DateCreated = DateTime.Now;
            this.ValidationErrors = new ValidationError();
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the validation errors.
        /// </summary>
        public ValidationError ValidationErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier 
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.DateModified = DateTime.Now;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
                this.DateModified = DateTime.Now;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return this.dateCreated;
            }
            
            set
            {
                this.dateCreated = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                return this.dateModified;
            }
            
            set
            {
                this.dateModified = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        public void Validate()
        {
            this.ValidationErrors.Clear();

            if (string.IsNullOrWhiteSpace(this.title))
            {
                this.ValidationErrors["Title"] = "Title is required.";
            }

            if (string.IsNullOrWhiteSpace(this.description))
            {
                this.ValidationErrors["Description"] = "You must type some text for the note.";
            }

            this.IsValid = this.ValidationErrors.IsValid;
// ReSharper disable ExplicitCallerInfoArgument
            this.OnPropertyChanged("ValidationErrors");
// ReSharper restore ExplicitCallerInfoArgument
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Title, this.Id);
        }
    }
}