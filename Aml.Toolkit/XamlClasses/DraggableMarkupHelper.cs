// Copyright (c) 2017 AutomationML e.V.
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// The Helper namespace.
/// </summary>
namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     helper class that defines just an attached property if an ui element is draggable
/// </summary>
public static class DraggableMarkupHelper
{
    #region Public Fields

    /// <summary>
    ///     The scroll on drag drop property
    /// </summary>
    public static readonly DependencyProperty ScrollOnDragDropProperty =
        DependencyProperty.RegisterAttached("ScrollOnDragDrop",
            typeof(bool),
            typeof(DraggableMarkupHelper),
            new PropertyMetadata(false, HandleScrollOnDragDropChanged));

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    ///     Gets the first visual child.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="depObj">The dep object.</param>
    /// <returns>T.</returns>
    public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null)
        {
            return null;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T dependencyObject)
            {
                return dependencyObject;
            }

            var childItem = GetFirstVisualChild<T>(child);
            if (childItem != null)
            {
                return childItem;
            }
        }

        return null;
    }

    /// <summary>
    ///     Gets the scroll on drag drop.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">element</exception>
    public static bool GetScrollOnDragDrop(DependencyObject element)
    {
        return element == null ? throw new ArgumentNullException(nameof(element)) : (bool)element.GetValue(ScrollOnDragDropProperty);
    }

    /// <summary>
    ///     Sets the scroll on drag drop.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <exception cref="ArgumentNullException">element</exception>
    public static void SetScrollOnDragDrop(DependencyObject element, bool value)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValue(ScrollOnDragDropProperty, value);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    ///     Handles the scroll on drag drop changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var container = d as FrameworkElement;
        if (d == null)
        {
            Debug.Fail("Invalid type!");
            return;
        }

        Unsubscribe(container);
        if (true.Equals(e.NewValue))
        {
            Subscribe(container);
        }
    }

    /// <summary>
    ///     Handles the <see cref="E:ContainerPreviewDragOver" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DragEventArgs" /> instance containing the event data.</param>
    private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
    {
        if (sender is not FrameworkElement container)
        {
            return;
        }

        var scrollViewer = GetFirstVisualChild<ScrollViewer>(container);
        if (scrollViewer == null)
        {
            return;
        }

        const double tolerance = 50;
        var verticalPos = e.GetPosition(container).Y;
        const double offset = 10;
        if (verticalPos < tolerance)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
        }
        else if (verticalPos > container.ActualHeight - tolerance)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
        }
        // Debug.WriteLine(scrollViewer.VerticalOffset + offset);
    }

    /// <summary>
    ///     Subscribes the specified container.
    /// </summary>
    /// <param name="container">The container.</param>
    private static void Subscribe(FrameworkElement container)
    {
        container.PreviewDragOver += OnContainerPreviewDragOver;
    }

    /// <summary>
    ///     Unsubscribes the specified container.
    /// </summary>
    /// <param name="container">The container.</param>
    private static void Unsubscribe(FrameworkElement container)
    {
        container.PreviewDragOver -= OnContainerPreviewDragOver;
    }

    #endregion Private Methods
}