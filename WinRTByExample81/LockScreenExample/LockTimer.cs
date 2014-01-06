using System;

namespace LockScreenExample
{
    using Windows.ApplicationModel.Background;
    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Badges;
    using WinRTByExample.NotificationHelper.Tiles;

    public class LockTimer : IBackgroundTask
    {
        private readonly Random random = new Random();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var badge = random.Next(1, 99);

            TileTemplateType.TileWide310x150Text03.GetTile()
                                                    .AddText(string.Format("Random badge {0}", badge))
                                                    .AddText("This is updated lockscreen text")
                                                    .AddText(string.Format("Refreshed on {0}", DateTime.Now))
                                                    .Set();

            badge.GetBadge().Set();
        }
    }
}
