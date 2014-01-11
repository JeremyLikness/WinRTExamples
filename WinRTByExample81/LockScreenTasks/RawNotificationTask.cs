namespace LockScreenTasks
{
    using System;

    using Windows.ApplicationModel.Background;
    using Windows.Networking.PushNotifications;
    using Windows.Storage;
    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Toasts;

    public sealed class RawNotificationTask : IBackgroundTask
    {
        private const string MessageKey = "RawMessage"; 

        public static string GetMessageKey()
        {
            return MessageKey;
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var notification = taskInstance.TriggerDetails as RawNotification;
            if (notification == null)
            {
                return;
            }
            var content = string.Format("{0}: {1}", DateTime.Now, notification.Content);
            ApplicationData.Current.LocalSettings.Values[MessageKey] = content;
            ToastTemplateType.ToastText01.GetToast().AddText(content).Send();
        }
    }
}
