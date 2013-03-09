// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPageAndGroupsManager.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The IPageAndGroupManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Skrape.Data;

    /// <summary>
    /// The PageAndGroupManager interface.
    /// </summary>
    public interface IPageAndGroupManager
    {
        /// <summary>
        /// The save page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> for asynchronous saving.
        /// </returns>
        Task SavePage(SkrapedPage page);

        /// <summary>
        /// The save group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        Task SaveGroup(SkrapeGroup group);

        /// <summary>
        /// The restore groups.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> for asynchronous operation.
        /// </returns>
        Task<IEnumerable<SkrapeGroup>> RestoreGroups();

        /// <summary>
        /// Delete the group
        /// </summary>
        /// <param name="group">The group to delete</param>
        void DeleteGroup(SkrapeGroup group);

        /// <summary>
        /// Delete the page
        /// </summary>
        /// <param name="page">The page to delete</param>
        void DeletePage(SkrapedPage page);
    }
}
