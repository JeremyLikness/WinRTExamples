// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.DataProtection;
    using Windows.Storage;
    using Windows.Storage.Streams;

    /// <summary>
    /// The data source.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "Notes";

        /// <summary>
        /// The scope
        /// </summary>
        /// <remarks>
        /// See this entry for more on what the scope means: http://msdn.microsoft.com/en-us/library/windows/apps/windows.security.cryptography.dataprotection.dataprotectionprovider.aspx
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here because it's a link.")]
        private const string Scope = "LOCAL=user";

        /// <summary>
        /// The encoding.
        /// </summary>
        private const BinaryStringEncoding Encoding = BinaryStringEncoding.Utf8;

        /// <summary>
        /// The notes folder.
        /// </summary>
        private StorageFolder notesFolder;

        /// <summary>
        /// The initialize async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            var localStorage = ApplicationData.Current.LocalFolder;
            this.notesFolder = await localStorage.CreateFolderAsync(Path, CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// The save note async.
        /// </summary>
        /// <param name="note">
        /// The note.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveNoteAsync(SimpleNote note)
        {
            var fileName = note.Id;
            var file = await this.notesFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var data = new[]
                           {
                               await ProtectDataAsync(note.Title), 
                               await ProtectDataAsync(note.Description),
                               await ProtectDataAsync(note.DateCreated.ToString()),
                               await ProtectDataAsync(note.DateModified.ToString())
                           };

            await FileIO.WriteLinesAsync(file, data);
        }

        /// <summary>
        /// The load notes async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ICollection<SimpleNote>> LoadNotesAsync()
        {
            var noteList = new List<SimpleNote>();
            var notes = await this.notesFolder.GetFilesAsync();
            
            // not good for LINQ expression due to await 
            foreach (var note in notes)
            {
                var loadedNote = await this.LoadNoteAsync(note.Name);
                if (loadedNote != null)
                {
                    noteList.Add(loadedNote);
                }
            }

            return noteList;
        }

        /// <summary>
        /// The protect data async.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<string> ProtectDataAsync(string data)
        {
            var dataProtection = new DataProtectionProvider(Scope);
            IBuffer dataBuffer = CryptographicBuffer.ConvertStringToBinary(data, Encoding);
            IBuffer encryptedBuffer = await dataProtection.ProtectAsync(dataBuffer);
            return CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        }

        /// <summary>
        /// The unprotect data async.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<string> UnprotectDataAsync(string data)
        {
            var dataProtection = new DataProtectionProvider(Scope);
            IBuffer encryptedBuffer = CryptographicBuffer.DecodeFromBase64String(data);
            IBuffer dataBuffer = await dataProtection.UnprotectAsync(encryptedBuffer);
            return CryptographicBuffer.ConvertBinaryToString(Encoding, dataBuffer);
        }

        /// <summary>
        /// The load note async.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<SimpleNote> LoadNoteAsync(string id)
        {
            var file = await this.notesFolder.GetFileAsync(id);
            var data = await FileIO.ReadLinesAsync(file);
            if (data.Count != 4)
            {
                return null;
            }

            return new SimpleNote
            {
                Id = id,
                Title = await UnprotectDataAsync(data[0]),
                Description = await UnprotectDataAsync(data[1]),
                DateCreated = DateTime.Parse(await UnprotectDataAsync(data[2])),
                DateModified = DateTime.Parse(await UnprotectDataAsync(data[3]))
            };
        }
    }
}
