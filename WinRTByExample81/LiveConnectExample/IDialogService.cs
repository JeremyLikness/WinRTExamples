using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace LiveConnectExample
{
    public interface IDialogService
    {
        void ShowMessageBox(String content, String title);

        void ShowMessageBoxAsync(String content, String title, IEnumerable<UICommand> commands,
            UInt32 defaultCommandIndex = 0);

        void ShowError(String content);
    }

    public class DialogService : IDialogService
    {
        private readonly CoreDispatcher _dispatcher;

        public DialogService(CoreDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;
        }

        public async void ShowMessageBox(String content, String title)
        {
            if (!_dispatcher.HasThreadAccess)
            {
                await _dispatcher.RunAsync(_dispatcher.CurrentPriority, () => ShowMessageBox(content, title));
                return;
            }
            var messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }

        public async void ShowMessageBoxAsync(String content, String title, IEnumerable<UICommand> commands, UInt32 defaultCommandIndex = 0)
        {
            if (!_dispatcher.HasThreadAccess)
            {
                await _dispatcher.RunAsync(_dispatcher.CurrentPriority, () => ShowMessageBoxAsync(content, title, commands, defaultCommandIndex));
                return;
            }
            
            var messageDialog = new MessageDialog(content, title);
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    messageDialog.Commands.Add(command);
                }
            }
            messageDialog.DefaultCommandIndex = defaultCommandIndex;
            await messageDialog.ShowAsync();
        }

        public async void ShowError(String content)
        {
            if (!_dispatcher.HasThreadAccess)
            {
                await _dispatcher.RunAsync(_dispatcher.CurrentPriority, () => ShowError(content));
                return;
            }

            var messageDialog = new MessageDialog(content, "Error");
            await messageDialog.ShowAsync();
        }
    }
}