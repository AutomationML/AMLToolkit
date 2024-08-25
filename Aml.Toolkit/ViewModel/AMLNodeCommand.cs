// Copyright (c) 2017 AutomationML e.V.
using Aml.Editor.MVVMBase;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     This class defines a command which can be added to a nodes command collection to be accessed from a context menu in
///     the tree view.
/// </summary>
/// <seealso cref="ViewModelBase" />
public class AMLNodeCommand : ViewModelBase
{
    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeCommand" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    public AMLNodeCommand(ICommand command)
    {
        Command = command;
    }

    #endregion Public Constructors

    #region Private Fields

    /// <summary>
    ///     <see cref="Command" />
    /// </summary>
    private ICommand _command;

    private ObservableCollection<AMLNodeCommand> _commands;

    /// <summary>
    ///     <see cref="Info" />
    /// </summary>
    private string _info;

    /// <summary>
    ///     <see cref="InputGesture" />
    /// </summary>
    private string _inputGesture;

    /// <summary>
    ///     <see cref="IsEnabled" />
    /// </summary>
    private bool _isEnabled;

    /// <summary>
    ///     <see cref="Name" />
    /// </summary>
    private string _name;

    /// <summary>
    ///     <see cref="Parameter" />
    /// </summary>
    private object _parameter;

    private string _icon;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     Gets and sets the Command
    /// </summary>
    public ICommand Command
    {
        get => _command;
        set => Set(ref _command, value);
    }

    /// <summary>
    ///     Gets the sub commands for this command
    /// </summary>
    public ObservableCollection<AMLNodeCommand> Commands =>
        _commands ??= [];

    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    public short Identifier { get; set; }

    /// <summary>
    ///     Gets and sets the Info
    /// </summary>
    public string Info
    {
        get => _info;
        set => Set(ref _info, value);
    }

    /// <summary>
    ///     Gets and sets the InputGesture
    /// </summary>
    public string InputGesture
    {
        get => _inputGesture;
        set => Set(ref _inputGesture, value);
    }

    /// <summary>
    ///     Gets and sets the IsEnabled
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => Set(ref _isEnabled, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is a separator.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is separator; otherwise, <c>false</c>.
    /// </value>
    public bool IsSeparator { get; set; }

    /// <summary>
    ///     Gets and sets the Name
    /// </summary>
    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    /// <summary>
    ///     Gets or sets the icon.
    /// </summary>
    /// <value>
    ///     The icon.
    /// </value>
    public string Icon
    {
        get => _icon ?? Name;
        set => _icon = value;
    }

    /// <summary>
    ///     Gets and sets the Parameter
    /// </summary>
    public object Parameter
    {
        get => _parameter;
        set => Set(ref _parameter, value);
    }

    #endregion Public Properties
}