// Copyright (c) 2017 AutomationML e.V.
using Aml.Editor.MVVMBase;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Aml.Toolkit.Operations;

/// <summary>
///     Class ItemOperationViewModel is the view model for an item in a tool bar. The view model can be used for any view
///     like button, toggle button, or input control. It provides a property to assign a specific data template for
///     representation of the view model in the tool bar.
/// </summary>
public class ItemOperationViewModel : ViewModelBase
{
    #region Private Fields

    private bool _isActive;

    /// <summary>
    ///     <see cref="IsChecked" />
    /// </summary>
    private bool _isChecked;

    /// <summary>
    ///     <see cref="Command" />
    /// </summary>
    private ICommand _command;

    /// <summary>
    ///     <see cref="CommandParameter" />
    /// </summary>
    private object _commandParameter;

    /// <summary>
    ///     <see cref="IconData" />
    /// </summary>
    private Geometry _iconData;

    /// <summary>
    ///     <see cref="IsEnabled" />
    /// </summary>
    private bool _isEnabled;

    /// <summary>
    ///     <see cref="Name" />
    /// </summary>
    private string _name;

    /// <summary>
    ///     <see cref="ToolTip" />
    /// </summary>
    private string _toolTip;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     Gets and sets the Command which performs the operation
    /// </summary>
    /// <value>The command.</value>
    public virtual ICommand Command
    {
        get => _command;
        set => Set(ref _command, value);
    }

    /// <summary>
    ///     Gets and sets the CommandParameter for the <see cref="Command" />
    /// </summary>
    /// <value>The commandParameter.</value>
    public object CommandParameter
    {
        get => _commandParameter;
        set => Set(ref _commandParameter, value);
    }


    /// <summary>
    ///     Gets or sets the control template.
    /// </summary>
    /// <value>
    ///     The control template.
    /// </value>
    public ControlTemplate ControlTemplate { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance has separator.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has separator; otherwise, <c>false</c>.
    /// </value>
    public bool HasSeparator { get; set; }

    /// <summary>
    ///     Gets and sets the IconData used in the view representation of the operation
    /// </summary>
    /// <value>The icon data.</value>
    public Geometry IconData
    {
        get => _iconData;
        set => Set(ref _iconData, value);
    }

    /// <summary>
    ///     Gets or sets the identifier used to identify the operation. The identifier can be used in the Indexer of
    ///     <see cref="ItemOperations" />.
    /// </summary>
    /// <value>The identifier.</value>
    public int Identifier { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive
    {
        get => _isActive;
        set => Set(ref _isActive, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is checkable.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
    /// </value>
    public bool IsCheckable { get; set; }

    /// <summary>
    ///     Gets and sets a value indicating whether this instance is checked
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => Set(ref _isChecked, value);
    }

    /// <summary>
    ///     Gets and sets a value indicating whether this instance is a drop down item
    /// </summary>
    public bool IsDropDown { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is enabled. Set this to false, if the Item should be
    ///     invisible.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => Set(ref _isEnabled, value);
    }

    /// <summary>
    ///     Gets and sets a value indicating whether this instance is templated
    /// </summary>
    public bool IsTemplated { get; set; }

    /// <summary>
    ///     Gets or sets the items source.
    /// </summary>
    /// <value>
    ///     The items source.
    /// </value>
    public IEnumerable ItemsSource { get; set; }

    /// <summary>
    ///     Gets or sets the item template used for representing the item in a view.
    /// </summary>
    /// <value>The item data template.</value>
    public DataTemplate ItemTemplate { get; set; }

    /// <summary>
    ///     Gets and sets the Name for the item operation. Names can be used in a view representation as an alternative for
    ///     <see cref="IconData" />
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    /// <summary>
    ///     Gets and sets the ToolTip for the view representation
    /// </summary>
    /// <value>The tool tip.</value>
    public string ToolTip
    {
        get => _toolTip;
        set => Set(ref _toolTip, value);
    }

    #endregion Public Properties
}