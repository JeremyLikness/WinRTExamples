namespace NetworkInfoExample.Common
{
    using System;
    using System.Threading.Tasks;

    using Windows.UI.Core;
    using Windows.UI.Popups;

    public class DialogHandler : IDialog
    {
        private CoreDispatcher Dispatcher { get; set; }

        public DialogHandler(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        private bool showDialog = true;

        public async Task ShowMessageAsync(string message)
        {
            if (Dispatcher.HasThreadAccess)
            {
                await this.ShowDialog(message);
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await this.ShowDialog(message));
            }
        }

        private async Task ShowDialog(string message)
        {
            if (showDialog)
            {
                showDialog = false;
                var dialog = new MessageDialog(message);
                await dialog.ShowAsync();
                showDialog = true;
            }
        }
    }
}