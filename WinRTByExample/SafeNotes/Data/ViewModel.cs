// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using SafeNotes.Common;

    using Windows.UI.Notifications;
    using Windows.UI.Popups;

    using WinRTByExample.NotificationHelper.Tiles;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : BindableBase, ICommand 
    {
        /// <summary>
        /// The data source 
        /// </summary>
        private readonly DataSource dataSource = new DataSource();

        /// <summary>
        /// The group helper.
        /// </summary>
        private readonly GroupHelper groupHelper = new GroupHelper();
     
        /// <summary>
        /// The current group.
        /// </summary>
        private NoteGroup currentGroup;

        /// <summary>
        /// The current note.
        /// </summary>
        private SimpleNote currentNote;

        /// <summary>
        /// The current editable note
        /// </summary>
        private SimpleNote currentEditableNote;

        /// <summary>
        /// True if the current note being edited is new
        /// </summary>
        private bool isNew;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.NoteGroups = new ObservableCollection<NoteGroup>();
            this.NotableGroups = new ObservableCollection<NoteGroup>();

            foreach (var group in this.groupHelper.GetGroups())
            {
                this.NoteGroups.Add(group);
            }

            this.GoHome = delegate { };

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            // only design-time logic here 
            var random = new Random();

            for (var x = 0; x < 20; x++)
            {
                var note = new SimpleNote
                               {
                                   Title = string.Format("Sample Note {0}", x),
                                   Description = new string('*', 999),
                                   DateCreated = DateTime.Now.AddYears(-2),
                                   DateModified = DateTime.Now.AddDays(-360 * random.NextDouble())
                               };
                this.groupHelper.InsertNoteIntoGroups(note, this.NoteGroups);
            }

            this.GetNotableGroups();

            this.currentNote = this.CurrentEditableNote = this.NotableGroups[0].Notes[0];
        }       

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Gets the note groups.
        /// </summary>
        public ObservableCollection<NoteGroup> NoteGroups { get; private set; }

        /// <summary>
        /// Gets only groups with something in them
        /// </summary>
        public ObservableCollection<NoteGroup> NotableGroups { get; private set; }
        
        /// <summary>
        /// Gets or sets the current group.
        /// </summary>
        public NoteGroup CurrentGroup
        {
            get
            {
                return this.currentGroup;
            }

            set
            {
                this.currentGroup = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current note.
        /// </summary>
        public SimpleNote CurrentNote
        {
            get
            {
                return this.currentNote;
            }

            set
            {
                this.currentNote = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current editable note.
        /// </summary>
        public SimpleNote CurrentEditableNote
        {
            get
            {
                return this.currentEditableNote;
            }

            set
            {
                this.currentEditableNote = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is new.
        /// </summary>
        public bool IsNew
        {
            get
            {
                return this.isNew;
            }

            set
            {
                this.isNew = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets the go home navigation
        /// </summary>
        public Action GoHome { get; set; }

        /// <summary>
        /// Gets a value indicating whether is dirty.
        /// </summary>
        private bool IsDirty
        {
            get
            {
                if (this.isNew)
                {
                    return !string.IsNullOrWhiteSpace(this.currentEditableNote.Title)
                           || !string.IsNullOrWhiteSpace(this.CurrentEditableNote.Description);
                }

                return !this.CurrentEditableNote.Title.Equals(this.currentNote.Title)
                       || !this.CurrentEditableNote.Description.Equals(this.currentNote.Description);
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            if (this.IsInitialized)
            {
                return;
            }

            await this.dataSource.InitializeAsync();
            var notes = await this.dataSource.LoadNotesAsync();

            if (notes.Count == 0)
            {
                var defaultNote = new SimpleNote
                                      {
                                          Title = "Tap to Get Started",
                                          Description =
                                              "This is a simple note to help you get started. Feel free to edit the title and the content."
                                      };
                this.currentEditableNote = defaultNote;
                this.IsNew = true;
                this.Save();
            }
            else 
            {
                foreach (var note in notes)
                {
                    this.groupHelper.InsertNoteIntoGroups(note, this.NoteGroups);
                }

                this.CurrentNote = notes.FirstOrDefault();
                this.GetNotableGroups();
                this.SetTile();
            }

            this.IsInitialized = true;
        }

        /// <summary>
        /// Sets the view model to add mode
        /// </summary>
        public void SetNew()
        {
            this.CurrentEditableNote = new SimpleNote();
            this.IsNew = true;
        }

        /// <summary>
        /// Sets the view model to edit mode
        /// </summary>
        public void SetEdit()
        {
            this.CurrentEditableNote = new SimpleNote
                                           {
                                               Title = this.currentNote.Title,
                                               Description = this.currentNote.Description
                                           };
            this.IsNew = false;
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return parameter is string && !string.IsNullOrWhiteSpace((string)parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter))
            {
                return;
            }

            var command = (string)parameter;

            if (command.Equals("cancel"))
            {
                this.Cancel();
                return;
            }

            if (command.Equals("save"))
            {
                this.Save();
            }
        }

        /// <summary>
        /// The set tile.
        /// </summary>
        private void SetTile()
        {
            TileTemplateType.TileWideText09.GetTile()
                            .WithNotifications()
                            .AddText(this.currentNote.Title)
                            .AddText(this.currentNote.Description)
                            .WithTile(
                                TileTemplateType.TileSquareText02.GetTile()
                                                .AddText(this.currentNote.Title)
                                                .AddText(this.currentNote.Description))
                                                .Set();
        }

        /// <summary>
        /// The get notable groups.
        /// </summary>
        private void GetNotableGroups()
        {
            this.NotableGroups.Clear();
            foreach (var @group in this.NoteGroups.Where(@group => @group.Notes.Count > 0))
            {
                this.NotableGroups.Add(@group);
            }
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        private void Cancel()
        {
            if (this.IsDirty)
            {
                Action action = async () =>
                    {
                        var okCommand = new UICommand("OK", cmd => GoHome());
                        var cancelCommand = new UICommand("Cancel", cmd => { });

                        var dialog = new MessageDialog(
                            "Are you sure you wish to cancel? Changes will be lost.", "Confirm Cancel");
                        dialog.Commands.Add(okCommand);
                        dialog.Commands.Add(cancelCommand);
                        await dialog.ShowAsync();
                    };
                action();
            }
            else
            {
                this.GoHome();
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        private void Save()
        {
            this.currentEditableNote.Validate();
            if (!this.currentEditableNote.IsValid)
            {
                return;
            }

            if (this.isNew)
            {
                this.groupHelper.InsertNoteIntoGroups(this.CurrentEditableNote, this.NoteGroups);
                this.GetNotableGroups();
                this.CurrentNote = this.currentEditableNote;                
            }
            else
            {
                this.currentNote.Title = this.currentEditableNote.Title;
                this.currentNote.Description = this.currentEditableNote.Description;
            }

            Task.Run(async () => await this.dataSource.SaveNoteAsync(this.CurrentNote));
            this.SetTile();
            this.GoHome();
        }
    }
}
