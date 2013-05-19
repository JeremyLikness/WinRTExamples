// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BadgeHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The badge helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Badges
{
    using System;
    using System.Linq;

    /// <summary>
    /// The tile helper.
    /// </summary>
    public static class BadgeHelper
    {
        /// <summary>
        /// The base badges.
        /// </summary>
        private static BaseBadge[] baseBadges;

        /// <summary>
        /// The get badges.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseBadge"/> array.
        /// </returns>
        public static BaseBadge[] GetBadges()
        {
            return baseBadges
                   ?? (baseBadges =
                       Enum.GetNames(typeof(BadgeGlyphTypes))
                           .Select(badgeType => (BadgeGlyphTypes)Enum.Parse(typeof(BadgeGlyphTypes), badgeType))
                           .Select(badgeTemplateType => badgeTemplateType.Equals(BadgeGlyphTypes.Numeric) ? 
                                                            new BaseBadge(1) : 
                                                            new BaseBadge(badgeTemplateType))
                           .ToArray());
        }

        /// <summary>
        /// The get description.
        /// </summary>
        /// <param name="badge">
        /// The badge.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDescription(this BaseBadge badge)
        {
            string badgeText;

            if (badge.GlyphType.Equals(BadgeGlyphTypes.None))
            {
                badgeText = "Empty badge (clears the badge)";
            }
            else if (badge.GlyphType.Equals(BadgeGlyphTypes.Numeric))
            {
                badgeText = string.Format("Numeric badge with the value {0}", badge.NumericValue);
            }
            else
            {
                badgeText = string.Format("Glyph badge with icon {0}", badge.TemplateType);
            }

            return badgeText;
        }

        /// <summary>
        /// Initializes static members of the <see cref="BadgeHelper"/> class.
        /// </summary>
        /// <param name="badgeTemplateType">
        /// The badge Template Type.
        /// </param>
        /// <returns>
        /// The <see cref="BaseBadge"/>.
        /// </returns>
        public static BaseBadge GetBadge(this BadgeGlyphTypes badgeTemplateType)
        {
            return new BaseBadge(badgeTemplateType);
        }

        /// <summary>
        /// Gets the numeric badge
        /// </summary>
        /// <param name="badgeValue">The value to initialize the badge with</param>
        /// <returns>The <see cref="BaseBadge"/> instance</returns>
        public static BaseBadge GetBadge(this int badgeValue)
        {
            return new BaseBadge(badgeValue);
        }

        /// <summary>
        /// Gets the clear badge
        /// </summary>
        /// <returns>The clear badge</returns>
        public static BaseBadge ClearBadge()
        {
            return new BaseBadge(BadgeGlyphTypes.None);
        }
    }
}