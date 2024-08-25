// Copyright (c) 2017 AutomationML e.V.
using Aml.Toolkit.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aml.Toolkit.View;

/// <summary>
///     This Class implements the image, displayed as an aml node image in the aml treeview.
/// </summary>
/// <seealso cref="Control" />
public class NodeImage : Control
{
    #region Public Constructors

    static NodeImage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeImage),
            new FrameworkPropertyMetadata(typeof(NodeImage)));
    }

    #endregion Public Constructors

    #region Public Fields

    /// <summary>
    ///     The added data property
    /// </summary>
    public static readonly DependencyProperty AddedDataProperty =
        DependencyProperty.Register("AddedData", typeof(object), typeof(NodeImage), new PropertyMetadata(null));

    /// <summary>
    ///     The image property
    /// </summary>
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(NodeImage), new PropertyMetadata(null));

    /// <summary>
    ///     The source datatemplate property
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(DataTemplate), typeof(NodeImage),
            new PropertyMetadata(default(DataTemplate)));

    /// <summary>
    ///     The text adorner property
    /// </summary>
    public static readonly DependencyProperty TextAdornerProperty =
        DependencyProperty.Register(nameof(TextAdorner), typeof(string), typeof(NodeImage),
            new PropertyMetadata(null));

    /// <summary>
    ///     The text adorner visibility property
    /// </summary>
    public static readonly DependencyProperty TextAdornerVisibilityProperty =
        DependencyProperty.Register(nameof(TextAdornerVisibility), typeof(Visibility), typeof(NodeImage),
            new PropertyMetadata(Visibility.Collapsed));

    #endregion Public Fields

    #region Public Properties

    /// <summary>
    ///     Gets the added data.
    /// </summary>
    /// <value>The added data.</value>
    public object AddedData => (GetValue(DataContextProperty) as AMLNodeViewModel)?.AdditionalInformation;

    /// <summary>
    ///     Gets or sets the image.
    /// </summary>
    /// <value>The image.</value>
    public ImageSource Image
    {
        get => (ImageSource)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    /// <summary>
    ///     Gets or sets the source.
    /// </summary>
    /// <value>The source.</value>
    public DataTemplate Source
    {
        get => (DataTemplate)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    ///     Gets or sets the text adorner.
    /// </summary>
    /// <value>The text adorner.</value>
    public string TextAdorner
    {
        get => (string)GetValue(TextAdornerProperty);
        set => SetValue(TextAdornerProperty, value);
    }

    /// <summary>
    ///     Gets or sets the text adorner visibility.
    /// </summary>
    /// <value>The text adorner visibility.</value>
    public Visibility TextAdornerVisibility
    {
        get => (Visibility)GetValue(TextAdornerVisibilityProperty);
        set => SetValue(TextAdornerVisibilityProperty, value);
    }

    #endregion Public Properties
}