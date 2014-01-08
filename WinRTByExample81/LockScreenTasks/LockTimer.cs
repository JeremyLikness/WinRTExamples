using System;

namespace LockScreenTasks
{
    using Windows.ApplicationModel.Background;
    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Badges;
    using WinRTByExample.NotificationHelper.Tiles;

    public sealed class LockTimer : IBackgroundTask
    {
        private static readonly Random Random = new Random();

        public static void RefreshTiles()
        {
            var badge = Random.Next(1, 99);

            TileTemplateType.TileWide310x150Text04.GetTile()
                                                    .AddText(string.Format("Random badge {0} was refreshed on {1}.", badge, DateTime.Now))
                                                    .Set();

            badge.GetBadge().Set();   
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            RefreshTiles();            
        }
    }
}
