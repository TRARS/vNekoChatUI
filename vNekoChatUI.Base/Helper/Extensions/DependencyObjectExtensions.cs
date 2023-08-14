using System.Windows;
using System.Windows.Media;

namespace vNekoChatUI.Base.Helper.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T FindVisualAncestor<T>(this DependencyObject target) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(target);
            if (parent == null)
            {
                return null;
            }
            if (parent is T)
            {
                return (T)parent;
            }
            return parent.FindVisualAncestor<T>();
        }

        public static T FindVisualChild<T>(this DependencyObject target) where T : DependencyObject
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(target) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(target, i);

                if (child != null && child is T)
                    return child as T;
                else
                {
                    T childOfChildren = FindVisualChild<T>(child);
                    if (childOfChildren != null)
                        return childOfChildren;
                }
            }
            return null;
        }

        public static T GetParentObject<T>(this DependencyObject target) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(target);
            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
