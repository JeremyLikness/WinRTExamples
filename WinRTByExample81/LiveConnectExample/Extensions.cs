using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveConnectExample
{
    internal static class Extensions
    {
        /// <summary>
        /// Flattens the provided dynamic items.
        /// </summary>
        /// <param name="profileItems">The profile items.</param>
        /// <param name="valuePreamble">The value preamble.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">profileItems</exception>
        public static IEnumerable<KeyValuePair<String, String>> FlattenDynamicItems(this IDictionary<String, Object> profileItems, String valuePreamble)
        {
            if (profileItems == null) throw new ArgumentNullException("profileItems");

            var profileItemsList = new List<KeyValuePair<String, String>>();
            foreach (var profileItem in profileItems)
            {
                var key = String.IsNullOrWhiteSpace(valuePreamble) ? profileItem.Key : String.Format("{0} - {1}", valuePreamble, profileItem.Key);

                if (profileItem.Value is IDictionary<String, Object>)
                {
                    var innerProfileItems = new Dictionary<String, Object>(profileItem.Value as IDictionary<String, Object>);
                    var innerItems = FlattenDynamicItems(innerProfileItems, key);
                    profileItemsList.AddRange(innerItems);
                }
                else if (profileItem.Value is IEnumerable && !(profileItem.Value is String))
                {
                    var enumerableProfileItemValue = profileItem.Value as IEnumerable;
                    foreach (var innerItem in enumerableProfileItemValue)
                    {
                        if (innerItem is IDictionary<String, Object>)
                        {
                            var innerProfileItems = new Dictionary<String, Object>(innerItem as IDictionary<String, Object>);
                            var innerItems = FlattenDynamicItems(innerProfileItems, key);
                            profileItemsList.AddRange(innerItems);
                        }
                        else
                        {
                            profileItemsList.Add(new KeyValuePair<String, String>(key,
                                (innerItem ?? "null").ToString()));
                        }
                    }
                }
                else
                {
                    profileItemsList.Add(new KeyValuePair<String, String>(key, (profileItem.Value ?? "null").ToString()));
                }
            }
            return profileItemsList;
        }

        /// <summary>
        /// Obtains a SkyDrive item sort order value based on the indicated item type.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        public static Int32 GetSkyDriveItemTypeOrder(this String itemType)
        {
            switch (itemType)
            {
                case "folder":
                    return 0;
                case "album":
                    return 1;
                case "audio":
                case "video":
                case "photo":
                    return 2;
                case "file":
                    return 3;
                case "notebook":
                    return 4;
                default:
                    return 5;
            }
        }
    }
}