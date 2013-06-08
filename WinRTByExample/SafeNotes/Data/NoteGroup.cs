// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoteGroup.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The note group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The note group.
    /// </summary>
    public class NoteGroup
    {
        /// <summary>
        /// The start date range this group is for 
        /// </summary>
        private DateTime startDate;

        /// <summary>
        /// The end date range this group is for 
        /// </summary>
        private DateTime endDate; 

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteGroup"/> class.
        /// </summary>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        public NoteGroup(DateTime startDate, DateTime endDate)
        {
            this.Notes = new ObservableCollection<SimpleNote>();
            this.startDate = startDate;
            this.endDate = endDate;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the notes.
        /// </summary>
        public ObservableCollection<SimpleNote> Notes { get; private set; }

        /// <summary>
        /// The try add note.
        /// </summary>
        /// <param name="note">
        /// The note.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TryAddNote(SimpleNote note)
        {
            if (note.DateModified >= this.startDate && note.DateModified < this.endDate)
            {
                this.Notes.Add(note);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} Notes)", this.Name, this.Notes.Count);
        }
    }
}
