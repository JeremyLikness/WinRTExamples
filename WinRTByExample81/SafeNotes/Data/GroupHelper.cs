// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The group helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The group helper.
    /// </summary>
    public class GroupHelper
    {
        /// <summary>
        /// The seconds in a day.
        /// </summary>
        private const int SecondsInADay = 86400;

        /// <summary>
        /// The January month.
        /// </summary>
        private const int January = 1;

        /// <summary>
        /// The December month.
        /// </summary>
        private const int December = 12;

        /// <summary>
        /// The date ranges.
        /// </summary>
        private readonly List<NoteGroup> dateRanges = new List<NoteGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupHelper"/> class.
        /// </summary>
        public GroupHelper()
        {
            // today 
            var today = DateTime.Now.Date; 
            this.dateRanges.Add(new NoteGroup(today, JustBeforeMidnight(today)) { Name = "Today" });

            // yesterday 
            var yesterday = today.AddDays(-1);
            this.dateRanges.Add(new NoteGroup(yesterday, today) { Name = "Yesterday" });

            // this week
            var oneWeekAgo = today.AddDays(-7);
            this.dateRanges.Add(new NoteGroup(oneWeekAgo, JustBeforeMidnight(today)) { Name = "This Week" });

            // last week
            var twoWeeksAgo = today.AddDays(-14);
            this.dateRanges.Add(new NoteGroup(twoWeeksAgo, oneWeekAgo) { Name = "Last Week" });

            // this month
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            this.dateRanges.Add(new NoteGroup(thisMonth, JustBeforeMidnight(today)) { Name = "This Month" });

            // last month
            var lastMonthYear = today.Month.Equals(January) ? today.Year - 1 : today.Year;
            var lastMonthMonth = today.Month.Equals(January) ? December : today.Month - 1;
            var lastMonth = new DateTime(lastMonthYear, lastMonthMonth, 1);
            this.dateRanges.Add(new NoteGroup(lastMonth, thisMonth) { Name = "Last Month" });

            // this year 
            var thisYear = new DateTime(today.Year, 1, 1);
            this.dateRanges.Add(new NoteGroup(thisYear, JustBeforeMidnight(today)) { Name = "This Year" });

            // last year 
            var lastYear = new DateTime(today.Year - 1, 1, 1);
            this.dateRanges.Add(new NoteGroup(lastYear, thisYear) { Name = "Last Year" });

            // older 
            this.dateRanges.Add(new NoteGroup(DateTime.MinValue, lastYear) { Name = "Older" });
        }

        /// <summary>
        /// The insert note into groups.
        /// </summary>
        /// <param name="note">
        /// The note.
        /// </param>
        /// <param name="groups">
        /// The groups.
        /// </param>
        public void InsertNoteIntoGroups(SimpleNote note, IEnumerable<NoteGroup> groups)
        {
            foreach (var group in groups)
            {
                group.TryAddNote(note);
            }
        }

        /// <summary>
        /// The get groups.
        /// </summary>
        /// <returns>
        /// The <see cref="NoteGroup"/> list.
        /// </returns>
        public IEnumerable<NoteGroup> GetGroups()
        {
            return this.dateRanges;
        }

        /// <summary>
        /// The just before midnight.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime JustBeforeMidnight(DateTime date)
        {
            return date.AddSeconds(SecondsInADay - 1);
        }
    }
}