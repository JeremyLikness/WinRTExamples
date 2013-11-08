using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LiveConnectExample
{
    public static class VisualTreeHelpers
    {
        /// <summary> 
        /// Gets the ancestors of the element, up to the root. 114:     
        /// </summary> 115:     
        /// <param name="node">The element to start from.</param> 116:     
        /// <returns>An enumerator of the ancestors.</returns> 117:     
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            var parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary> 
        /// Gets the visual parent of the element.
        /// </summary>  
        /// <param name="node">The element to check.</param> 131:     
        /// /// <returns>The visual parent.</returns> 132:     
        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return VisualTreeHelper.GetParent(node) as FrameworkElement;
        }
    }
}