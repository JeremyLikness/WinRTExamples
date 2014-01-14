using System;
using System.Collections.Generic;

namespace InputsExample
{
    internal static class PointerExtensions
    {
        public static IEnumerable<String> GetPointerPointDump(this Windows.UI.Input.PointerPoint pointerPoint)
        {
            // Scratchpad for PointerProperties
            var values = new List<String>();

            values.Add(String.Format("PointerId: {0}", pointerPoint.PointerId));
            values.Add(String.Format("Device Type: {0}", pointerPoint.PointerDevice.PointerDeviceType)); 
            values.Add(String.Format("Device IsIntegrated: {0}", pointerPoint.PointerDevice.IsIntegrated)); 
            values.Add(String.Format("Device MaxContacts: {0}", pointerPoint.PointerDevice.MaxContacts)); 
            values.Add(String.Format("Position: {0}", pointerPoint.Position));
            values.Add(String.Format("IsInContact: {0}", pointerPoint.IsInContact));
            values.Add(String.Format("FrameId: {0}", pointerPoint.FrameId));
            values.Add(String.Format("Properties:"));
            values.Add(String.Format("  IsPrimary {0}", pointerPoint.Properties.IsPrimary));
            values.Add(String.Format("  Touch - ContactRect {0}", pointerPoint.Properties.ContactRect));
            values.Add(String.Format("Properties: Stylus"));
            values.Add(String.Format("  IsBarrelButtonPressed {0}", pointerPoint.Properties.IsBarrelButtonPressed));
            values.Add(String.Format("  IsEraser {0}", pointerPoint.Properties.IsEraser));
            values.Add(String.Format("  IsInRange {0}", pointerPoint.Properties.IsInRange));
            values.Add(String.Format("  IsInverted {0}", pointerPoint.Properties.IsInverted));
            values.Add(String.Format("  Orientation {0}", pointerPoint.Properties.Orientation));
            values.Add(String.Format("  Pressure {0}", pointerPoint.Properties.Pressure));
            values.Add(String.Format("  Twist {0}", pointerPoint.Properties.Twist));
            values.Add(String.Format("  XTilt {0}", pointerPoint.Properties.XTilt));
            values.Add(String.Format("  YTilt {0}", pointerPoint.Properties.YTilt));
            values.Add(String.Format("Properties: Mouse"));
            values.Add(String.Format("  IsLeftButtonPressed {0}", pointerPoint.Properties.IsLeftButtonPressed));
            values.Add(String.Format("  IsMiddleButtonPressed {0}", pointerPoint.Properties.IsMiddleButtonPressed));
            values.Add(String.Format("  IsRightButtonPressed {0}", pointerPoint.Properties.IsRightButtonPressed));
            values.Add(String.Format("  IsXButton1Pressed {0}", pointerPoint.Properties.IsXButton1Pressed));
            values.Add(String.Format("  IsXButton2Pressed {0}", pointerPoint.Properties.IsXButton2Pressed));
            values.Add(String.Format("  IsHorizontalMouseWheel {0}", pointerPoint.Properties.IsHorizontalMouseWheel));
            values.Add(String.Format("  MouseWheelDelta {0}", pointerPoint.Properties.MouseWheelDelta));
            values.Add(String.Format("Properties: HID"));

            foreach (var usage in pointerPoint.PointerDevice.SupportedUsages)
            {
                // USB HID Spec: http://www.usb.org/developers/devclass_docs/HID1_11.pdf
                values.Add(String.Format("  Usage Page: x{0:X2}, Id: x{1:X2}", usage.UsagePage, usage.Usage));
                values.Add(String.Format("      LogicalMin: {0} LogicalMax: {1}", usage.MinLogical, usage.MaxLogical));
                values.Add(String.Format("      PhysMin: {0} PhysMax: {1} PhysMultiplier {2}", usage.MinPhysical, usage.MaxPhysical, usage.PhysicalMultiplier));
                if (pointerPoint.Properties.HasUsage(usage.UsagePage, usage.Usage))
                {
                    var value = pointerPoint.Properties.GetUsageValue(usage.UsagePage, usage.Usage);
                    values.Add(String.Format("      Value: {0}", value));
                }
                else
                {
                    values.Add(String.Format("      NO VALUE"));
                }
            }

            return values;
        }
    }
}