// Copyright (c) 2017 AutomationML e.V.
using System.Windows;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Binding support class, used to bind to data when the DataContext is not inherited.
/// </summary>
/// <seealso cref="Freezable" />
public class BindingProxy : Freezable
{
    #region Public Fields

    /// <summary>
    ///     The data property
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy),
            new PropertyMetadata(null, BindingChanged));

    #endregion Public Fields

    #region Public Properties

    /// <summary>
    ///     Gets or sets the data to bind to.
    /// </summary>
    /// <value>
    ///     The data.
    /// </value>
    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    #endregion Public Properties

    #region Protected Methods

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore() => new BindingProxy();

    #endregion Protected Methods

    #region Private Methods

    private static void BindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion Private Methods
}