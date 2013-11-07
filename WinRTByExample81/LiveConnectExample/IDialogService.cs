using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LiveConnectExample
{
    public interface IDialogService
    {
        void ShowMessageBox(String content, String title);

        Task<IUICommand> ShowMessageBoxAsync(String content, String title, IEnumerable<UICommand> commands,
            UInt32 defaultCommandIndex = 0);

        void ShowError(String content);
    }

    public class DialogService : IDialogService
    {
        public async void ShowMessageBox(String content, String title)
        {
            var messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }

        public async Task<IUICommand> ShowMessageBoxAsync(String content, String title, IEnumerable<UICommand> commands, UInt32 defaultCommandIndex = 0)
        {
            var messageDialog = new MessageDialog(content, title);
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    messageDialog.Commands.Add(command);
                }
            }
            messageDialog.DefaultCommandIndex = defaultCommandIndex;
            return await messageDialog.ShowAsync();
        }

        public async void ShowError(String content)
        {
            var messageDialog = new MessageDialog(content, "Error");
            await messageDialog.ShowAsync();
        }
    }
}