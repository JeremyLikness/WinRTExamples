using System;
using Windows.Graphics.Display;

namespace SensorsExample
{
    public static class SensorExtensions
    {
        /// <summary>
        /// Retrieves compass offset values based on the provided display orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Unexpected orientation</exception>
        public static Int32 CompassOffset(this DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Landscape:
                    return 0;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.Portrait:
                    return 270;
                case DisplayOrientations.PortraitFlipped:
                    return 90;
            }
            throw new InvalidOperationException("Unexpected orientation");
        }

        /// <summary>
        /// Retrieves per-axis adjustment factors based on the provided display orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Unexpected orientation</exception>
        public static AxisOffset AxisAdjustmentFactor(this DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Landscape:
                    return new AxisOffset {X = 1, Y = 1, Z = 1};
                case DisplayOrientations.LandscapeFlipped:
                    return new AxisOffset { X = -1, Y = -1, Z = 1 };
                case DisplayOrientations.Portrait:
                    return new AxisOffset { X = 1, Y = -1, Z = 1 };
                case DisplayOrientations.PortraitFlipped:
                    return new AxisOffset { X = -1, Y = 1, Z = 1 };
            }
            throw new InvalidOperationException("Unexpected orientation");
        }

        public struct AxisOffset
        {
            public static AxisOffset Default
            {
                get { return new AxisOffset {X = 1, Y = 1, Z = 1}; }
            }
            public Int32 X { get; set; }
            public Int32 Y { get; set; }
            public Int32 Z { get; set; }
        }
    }
}