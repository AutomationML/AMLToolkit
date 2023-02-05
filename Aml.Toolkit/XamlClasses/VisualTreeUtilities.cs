using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Aml.Toolkit.XamlClasses
{
    /// <summary>
    /// Utilities to search the visual tree
    /// </summary>
    public static class VisualTreeUtilities
    {
        #region Public Methods

        /// <summary>
        ///     Search the visual tree for template and instance it.
        /// </summary>
        public static FrameworkElement CreateVisual(Type type, FrameworkElement element, object dataContext)
        {
            var template = FindTemplateForType<DataTemplate>(type, element);
            if (template == null)
            {
                throw new ApplicationException("Failed to find DataTemplate for type " + type.Name);
            }

            var visual = (FrameworkElement)template.LoadContent();
            visual.Resources = element.Resources;
            visual.DataContext = dataContext;
            return visual;
        }

        /// <summary>
        ///     Recursively dump out all elements in the visual tree.
        /// </summary>
        public static void DumpVisualTree(Visual root)
        {
            DumpVisualTree(root, 0);
        }

        /// <summary>
        ///     Find the ancestor of a particular element based on the type of the ancestor.
        /// </summary>
        public static T FindAncestor<T>(FrameworkElement element) where T : class
        {
            if (element.Parent != null)
            {
                var ancestor = element.Parent;
                if (ancestor is T ancestor1)
                {
                    return ancestor1;
                }

                if (element.Parent is FrameworkElement parent)
                {
                    return FindAncestor<T>(parent);
                }
            }

            if (element.TemplatedParent != null)
            {
                switch (element.TemplatedParent)
                {
                    case T ancestor:
                        return ancestor;
                    case FrameworkElement parent:
                        return FindAncestor<T>(parent);
                }
            }

            var visualParent = VisualTreeHelper.GetParent(element);
            if (visualParent == null)
            {
                return null;
            }

            return visualParent switch
            {
                T visualAncestor => visualAncestor,
                FrameworkElement visualElement => FindAncestor<T>(visualElement),
                _ => null
            };
        }

        /// <summary>
        ///     Find the framework element for the specified connector.
        /// </summary>
        public static ElementT FindElementWithDataContext<DataContextT, ElementT>(Visual rootElement, DataContextT data)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            switch (rootElement)
            {
                case null:
                    throw new ArgumentNullException(nameof(rootElement));
                case FrameworkElement rootFrameworkElement:
                    rootFrameworkElement.UpdateLayout();
                    break;
            }

            var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (var i = 0; i < numChildren; ++i)
            {
                var childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                if (childElement is ElementT typedChildElement &&
                    typedChildElement.DataContext == data)
                {
                    return typedChildElement;
                }

                var foundElement = FindElementWithDataContext<DataContextT, ElementT>(childElement, data);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        /// <summary>
        ///     Find the framework element for the specified connector.
        /// </summary>
        public static ElementT FindElementWithDataContextAndName<DataContextT, ElementT>(Visual rootElement,
            DataContextT data, string name)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            Trace.Assert(rootElement != null);

            if (rootElement is FrameworkElement rootFrameworkElement)
            {
                rootFrameworkElement.UpdateLayout();
            }

            var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (var i = 0; i < numChildren; ++i)
            {
                var childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                if (childElement is ElementT typedChildElement &&
                    typedChildElement.DataContext == data)
                {
                    if (typedChildElement.Name == name)
                    {
                        return typedChildElement;
                    }
                }

                var foundElement = FindElementWithDataContextAndName<DataContextT, ElementT>(childElement, data, name);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        /// <summary>
        ///     Find the framework element with the specified name.
        /// </summary>
        public static ElementT FindElementWithName<ElementT>(Visual rootElement, string name)
            where ElementT : FrameworkElement
        {
            if (rootElement is FrameworkElement rootFrameworkElement)
            {
                rootFrameworkElement.UpdateLayout();
            }

            if (rootElement is Popup popup)
            {
                Visual childElement = popup.Child;

                if (childElement is ElementT typedChildElement)
                {
                    if (typedChildElement.Name == name)
                    {
                        return typedChildElement;
                    }
                }

                var foundElement = FindElementWithName<ElementT>(childElement, name);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }
            else
            {
                var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
                for (var i = 0; i < numChildren; ++i)
                {
                    var childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                    if (childElement is ElementT typedChildElement)
                    {
                        if (typedChildElement.Name == name)
                        {
                            return typedChildElement;
                        }
                    }

                    var foundElement = FindElementWithName<ElementT>(childElement, name);
                    if (foundElement != null)
                    {
                        return foundElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Find the framework element for the specified connector.
        /// </summary>
        public static ElementT FindElementWithType<ElementT>(Visual rootElement)
            where ElementT : FrameworkElement
        {
            if (rootElement == null)
            {
                throw new ArgumentNullException(nameof(rootElement));
            }

            //if (rootElement is FrameworkElement rootFrameworkElement)
            //{
            //    rootFrameworkElement.UpdateLayout();
            //}

            //
            // Check each child.
            //
            var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (var i = 0; i < numChildren; ++i)
            {
                var childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                if (childElement is ElementT typedChildElement)
                {
                    return typedChildElement;
                }
            }

            //
            // Check sub-trees.
            //
            for (var i = 0; i < numChildren; ++i)
            {
                var childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                var foundElement = FindElementWithType<ElementT>(childElement);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the parent.
        /// </summary>
        /// <param name="childElement">The child element.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static FrameworkElement FindParent(FrameworkElement childElement, Predicate<FrameworkElement> predicate)
        {
            var parent = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            if (parent != null)
            {
                if (predicate(parent))
                {
                    return parent;
                }

                parent = FindParent(parent, predicate);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent == null)
            {
                return null;
            }

            parent = childElement.TemplatedParent as FrameworkElement;
            if (parent != null && predicate(parent))
            {
                return parent;
            }

            parent = FindParent((FrameworkElement)childElement.TemplatedParent, predicate);
            return parent;
        }

        /// <summary>
        ///     Search up the element tree to find the Parent window for 'element'.
        ///     Returns null if the 'element' is not attached to a window.
        /// </summary>
        public static Window FindParentWindow(FrameworkElement element)
        {
            return element.Parent switch
            {
                null => null,
                Window window => window,
                FrameworkElement parentElement => FindParentWindow(parentElement),
                _ => null
            };
        }

        /// <summary>
        /// Finds the parent with data context.
        /// </summary>
        /// <typeparam name="DataContextT">The type of the ata context t.</typeparam>
        /// <param name="childElement">The child element.</param>
        /// <returns></returns>
        public static FrameworkElement FindParentWithDataContext<DataContextT>(FrameworkElement childElement)
            where DataContextT : class
        {
            if (childElement.Parent != null)
            {
                if (((FrameworkElement)childElement.Parent).DataContext is DataContextT)
                {
                    return (FrameworkElement)childElement.Parent;
                }

                var parent = FindParentWithDataContext<DataContextT>((FrameworkElement)childElement.Parent);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent == null)
            {
                return null;
            }

            {
                if (((FrameworkElement)childElement.TemplatedParent).DataContext is DataContextT)
                {
                    return (FrameworkElement)childElement.TemplatedParent;
                }

                var parent = FindParentWithDataContext<DataContextT>((FrameworkElement)childElement.TemplatedParent);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the parent with data context and name.
        /// </summary>
        /// <typeparam name="DataContextT">The type of the ata context t.</typeparam>
        /// <param name="childElement">The child element.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static FrameworkElement FindParentWithDataContextAndName<DataContextT>(FrameworkElement childElement,
            string name)
            where DataContextT : class
        {
            var parent = (FrameworkElement)childElement.Parent;
            if (parent != null)
            {
                if (parent.DataContext is DataContextT)
                {
                    if (parent.Name == name)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithDataContextAndName<DataContextT>(parent, name);
                if (parent != null)
                {
                    return parent;
                }
            }
            parent = (FrameworkElement)childElement.TemplatedParent;
            if (parent == null)
            {
                return null;
            }
            if (parent.DataContext is not DataContextT)
            {
                return FindParentWithDataContextAndName<DataContextT>(parent, name);
            }
            return parent.Name == name ? parent : FindParentWithDataContextAndName<DataContextT>(parent, name);
        }

        /// <summary>
        /// Finds the parent with the provided type.
        /// </summary>
        /// <typeparam name="ParentT">The type of the parent.</typeparam>
        /// <param name="childElement">The child element.</param>
        /// <returns></returns>
        public static ParentT FindParentWithType<ParentT>(DependencyObject childElement)
            where ParentT : class
        {
            if (VisualTreeHelper.GetParent(childElement) is { } parentElement)
            {
                if (parentElement is ParentT parent)
                {
                    return parent;
                }

                return FindParentWithType<ParentT>(parentElement);
            }

            if (childElement is not FrameworkElement fe || fe.TemplatedParent == null)
            {
                return null;
            }

            {
                if (fe.TemplatedParent is ParentT parent)
                {
                    return parent;
                }

                parent = FindParentWithType<ParentT>(fe.TemplatedParent);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the parent with type and data context.
        /// </summary>
        /// <typeparam name="ParentT">The type of the parent.</typeparam>
        /// <param name="childElement">The child element.</param>
        /// <param name="dataContext">The data context.</param>
        /// <returns></returns>
        public static ParentT FindParentWithTypeAndDataContext<ParentT>(FrameworkElement childElement,
            object dataContext)
            where ParentT : FrameworkElement
        {
            if (childElement.Parent != null)
            {
                if (childElement.Parent is ParentT parent)
                {
                    if (parent.DataContext == dataContext)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithTypeAndDataContext<ParentT>((FrameworkElement)childElement.Parent, dataContext);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent != null)
            {
                if (childElement.TemplatedParent is ParentT parent)
                {
                    if (parent.DataContext == dataContext)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithTypeAndDataContext<ParentT>((FrameworkElement)childElement.TemplatedParent,
                    dataContext);
                if (parent != null)
                {
                    return parent;
                }
            }

            var parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            if (parentElement == null)
            {
                return null;
            }

            {
                if (parentElement is ParentT parent)
                {
                    return parent;
                }

                return FindParentWithType<ParentT>(parentElement);
            }

        }

        /// <summary>
        ///     Walk up the visual tree and find a template for the specified type.
        ///     Returns null if none was found.
        /// </summary>
        public static DataTemplateT FindTemplateForType<DataTemplateT>(Type type, FrameworkElement element)
            where DataTemplateT : class
        {
            var resource = element.TryFindResource(new DataTemplateKey(type));
            if (resource is DataTemplateT dataTemplate)
            {
                return dataTemplate;
            }

            if (type.BaseType != null &&
                type.BaseType != typeof(object))
            {
                dataTemplate = FindTemplateForType<DataTemplateT>(type.BaseType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                dataTemplate = FindTemplateForType<DataTemplateT>(interfaceType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            return null;
        }

        /// <summary>
        ///     Finds a particular type of UI element int he visual tree that has the specified data context.
        /// </summary>
        public static ICollection<T> FindTypedElements<T>(DependencyObject rootElement) where T : DependencyObject
        {
            var foundElements = new List<T>();
            FindTypedElements(rootElement, foundElements);
            return foundElements;
        }

        /// <summary>
        /// Finds the visual parent.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        public static TObject FindVisualParent<TObject>(DependencyObject child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            DependencyObject parent;

            if (child is Visual || child is Visual3D)
            {
                parent = VisualTreeHelper.GetParent(child) as UIElement;
            }
            else
            {
                parent = LogicalTreeHelper.GetParent(child);
            }

            while (parent != null)
            {
                switch (parent)
                {
                    case TObject found:
                        return found;
                    case Visual:
                    case Visual3D:
                        parent = VisualTreeHelper.GetParent(parent) as UIElement;
                        break;
                    default:
                        parent = LogicalTreeHelper.GetParent(parent);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the visual parent with the provided type.
        /// </summary>
        /// <typeparam name="ParentT">The type of the parent.</typeparam>
        /// <param name="childElement">The child element.</param>
        /// <returns></returns>
        public static ParentT FindVisualParentWithType<ParentT>(FrameworkElement childElement)
            where ParentT : class
        {
            var parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            return parentElement switch
            {
                null => null,
                ParentT parent => parent,
                _ => FindVisualParentWithType<ParentT>(parentElement)
            };
        }

        /// <summary>
        ///     Hit test against the specified element for a child that has a data context
        ///     of the specified type.
        ///     Returns 'null' if nothing was 'hit'.
        /// </summary>
        public static DataContextT HitTestForDataContext<DataContextT, ElementT>(FrameworkElement rootElement,
            Point point, out ElementT hitFrameworkElement)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            DataContextT data = null;
            hitFrameworkElement = null;
            ElementT frameworkElement = null;

            VisualTreeHelper.HitTest(
                rootElement,
                // Hit test filter.
                null,
                // Hit test result.
                delegate (HitTestResult result)
                {
                    frameworkElement = result.VisualHit as ElementT;
                    if (frameworkElement == null)
                    {
                        return HitTestResultBehavior.Continue;
                    }

                    data = frameworkElement.DataContext as DataContextT;
                    return data != null ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;

                },
                new PointHitTestParameters(point));

            hitFrameworkElement = frameworkElement;
            return data;
        }

        /// <summary>
        ///     Hit test for a specific data context and name.
        /// </summary>
        public static DataContextT HitTestForDataContextAndName<DataContextT, ElementT>(FrameworkElement rootElement,
            Point point, string name, out ElementT hitFrameworkElement)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            DataContextT data = null;
            hitFrameworkElement = null;
            ElementT frameworkElement = null;

            VisualTreeHelper.HitTest(
                rootElement,
                // Hit test filter.
                null,
                // Hit test result.
                delegate (HitTestResult result)
                {
                    frameworkElement = result.VisualHit as ElementT;
                    if (frameworkElement == null)
                    {
                        return HitTestResultBehavior.Continue;
                    }

                    data = frameworkElement.DataContext as DataContextT;
                    if (data == null)
                    {
                        return HitTestResultBehavior.Continue;
                    }

                    return frameworkElement.Name == name ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(point));

            hitFrameworkElement = frameworkElement;
            return data;
        }

        /// <summary>
        ///     Hit test against the specified element for a child that has a data context
        ///     of the specified type.
        ///     Returns 'null' if nothing was 'hit'.
        ///     Return the highest level element that matches the hit test.
        /// </summary>
        public static T HitTestHighestForDataContext<T>(FrameworkElement rootElement, Point point)
            where T : class
        {
            return HitTestHighestForDataContext<T>(rootElement, point, out _);
        }

        /// <summary>
        ///     Hit test against the specified element for a child that has a data context
        ///     of the specified type.
        ///     Returns 'null' if nothing was 'hit'.
        ///     Return the highest level element that matches the hit test.
        /// </summary>
        public static T HitTestHighestForDataContext<T>(FrameworkElement rootElement,
            Point point, out FrameworkElement hitFrameworkElement)
            where T : class
        {
            hitFrameworkElement = null;

            var hitData = HitTestForDataContext<T, FrameworkElement>(rootElement, point, out var hitElement);
            if (hitData == null)
            {
                return null;
            }

            hitFrameworkElement = hitElement;

            //
            // Find the highest level parent below root element that still matches the data context.
            while (hitElement != null && hitElement != rootElement &&
                   hitElement.DataContext == hitData)
            {
                hitFrameworkElement = hitElement;

                if (hitElement.Parent != null)
                {
                    hitElement = hitElement.Parent as FrameworkElement;
                    continue;
                }

                if (hitElement.TemplatedParent != null)
                {
                    hitElement = hitElement.TemplatedParent as FrameworkElement;
                    continue;
                }

                break;
            }

            return hitData;
        }

        /// <summary>
        ///     Layout, measure and arrange the specified element.
        /// </summary>
        public static void InitaliseElement(FrameworkElement element)
        {
            element.UpdateLayout();
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
        }

        /// <summary>
        ///     Transform a point to an ancestors coordinate system.
        /// </summary>
        public static Point TransformPointToAncestor<T>(FrameworkElement element, Point point) where T : Visual
        {
            var ancestor = FindAncestor<T>(element);
            if (ancestor == null)
            {
                throw new ApplicationException("Find to find '" + typeof(T).Name + "' for element '" +
                                               element.GetType().Name + "'.");
            }

            return TransformPointToAncestor(ancestor, element, point);
        }

        /// <summary>
        ///     Transform a point to an ancestors coordinate system.
        /// </summary>
        public static Point TransformPointToAncestor(Visual ancestor, FrameworkElement element, Point point)
        {
            return element.TransformToAncestor(ancestor).Transform(point);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///     Recursively dump out all elements in the visual tree.
        /// </summary>
        private static void DumpVisualTree(Visual root, int indentLevel)
        {
            var indentStr = new string(' ', indentLevel * 2);
            Trace.Write(indentStr);
            Trace.Write(root.GetType().Name);

            if (root is FrameworkElement { DataContext: { } } rootElement)
            {
                Trace.Write(" [");
                Trace.Write(rootElement.DataContext.GetType().Name);
                Trace.Write("]");
            }

            Trace.WriteLine("");

            var numChildren = VisualTreeHelper.GetChildrenCount(root);
            if (numChildren <= 0)
            {
                return;
            }

            Trace.Write(indentStr);
            Trace.WriteLine("{");

            for (var i = 0; i < numChildren; ++i)
            {
                var child = (Visual)VisualTreeHelper.GetChild(root, i);
                DumpVisualTree(child, indentLevel + 1);
            }

            Trace.Write(indentStr);
            Trace.WriteLine("}");
        }

        /// <summary>
        ///     Finds a particular type of UI element int he visual tree that has the specified data context.
        /// </summary>
        private static void FindTypedElements<T>(DependencyObject rootElement, List<T> foundElements)
            where T : DependencyObject
        {
            var numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (var i = 0; i < numChildren; ++i)
            {
                var childElement = VisualTreeHelper.GetChild(rootElement, i);
                if (childElement is T element)
                {
                    foundElements.Add(element);
                    continue;
                }

                FindTypedElements(childElement, foundElements);
            }
        }

        #endregion Private Methods
    }
}