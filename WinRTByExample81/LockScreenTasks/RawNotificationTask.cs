namespace LockScreenTasks
{
    using System;

    using Windows.ApplicationModel.Background;
    using Windows.Networking.PushNotifications;
    using Windows.Storage;
    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Badges;
    using WinRTByExample.NotificationHelper.Toasts;

    public sealed class RawNotificationTask : IBackgroundTask
    {
        private const string MessageKey = "RawMessage";

        private const string CountKey = "MessageCount";

        public static string GetMessageKey()
        {
            return MessageKey;
        }

        public static string GetCountKey()
        {
            return CountKey;
        }

        private int MessageCount
        {
            get
            {
                if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(CountKey))
                {
                    ApplicationData.Current.LocalSettings.Values[CountKey] = 0;
                }
                return int.Parse(ApplicationData.Current.LocalSettings.Values[CountKey].ToString());
            }

            set
            {
                ApplicationData.Current.LocalSettings.Values[CountKey] = value;
            }
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
            var count = MessageCount + 1;
            (count > 99 ? 99 : count).GetBadge().Set();
            MessageCount = count;
        }
    }
}
