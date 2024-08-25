// Copyright (c) 2017 AutomationML e.V.
using Aml.Toolkit.Operations;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Datatemplate selector for the display of tool bar items
/// </summary>
/// <seealso cref="System.Windows.Controls.DataTemplateSelector" />
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public class ToolBarItemTemplateSelector : DataTemplateSelector, INotifyPropertyChanged
{
    #region Public Events

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Methods

    /// <inheritdoc />
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item is not ItemOperationViewModel toolBarItem
            ? base.SelectTemplate(item, container)
            : toolBarItem.Identifier switch
            {
                EditOperations.Copy => CopyButtonTemplate,
                EditOperations.Cut => CutButtonTemplate,
                EditOperations.Delete => DeleteButtonTemplate,
                EditOperations.Paste => PasteButtonTemplate,
                EditOperations.Redo => RedoButtonTemplate,
                EditOperations.Undo => UndoButtonTemplate,
                SeparatorItem.Separator => SeparatorTemplate,
                _ => toolBarItem.ItemTemplate ?? ButtonTemplate
            };
    }

    #endregion Public Methods

    #region Private Methods

    private void RaisePropertyChanged(string p)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }

    #endregion Private Methods

    #region Private Fields

    /// <summary>
    ///     <see cref="ButtonTemplate" />
    /// </summary>
    private DataTemplate _buttonTemplate;

    /// <summary>
    ///     <see cref="CustomButtonTemplate" />
    /// </summary>
    private DataTemplate _customButtonTemplate;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     Gets and sets the ButtonTemplate
    /// </summary>
    public DataTemplate ButtonTemplate
    {
        get => _buttonTemplate;

        set
        {
            if (_buttonTemplate == value)
            {
                return;
            }

            _buttonTemplate = value;
            RaisePropertyChanged("ButtonTemplate");
        }
    }

    /// <summary>
    ///     Gets or sets the CheckBox template.
    /// </summary>
    /// <value>
    ///     The CheckBox template.
    /// </value>
    public DataTemplate CheckBoxTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the copy button template.
    /// </summary>
    /// <value>
    ///     The copy button template.
    /// </value>
    public DataTemplate CopyButtonTemplate { get; set; }

    /// <summary>
    ///     Gets and sets the CustomButtonTemplate
    /// </summary>
    public DataTemplate CustomButtonTemplate
    {
        get => _customButtonTemplate;

        set
        {
            if (_customButtonTemplate == value)
            {
                return;
            }

            _customButtonTemplate = value;
            RaisePropertyChanged("CustomButtonTemplate");
        }
    }

    /// <summary>
    ///     Gets or sets the cut button template.
    /// </summary>
    /// <value>
    ///     The cut button template.
    /// </value>
    public DataTemplate CutButtonTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the delete button template.
    /// </summary>
    /// <value>
    ///     The delete button template.
    /// </value>
    public DataTemplate DeleteButtonTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the label template.
    /// </summary>
    /// <value>
    ///     The label template.
    /// </value>
    public DataTemplate LabelTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the paste button template.
    /// </summary>
    /// <value>
    ///     The paste button template.
    /// </value>
    public DataTemplate PasteButtonTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the redo button template.
    /// </summary>
    /// <value>
    ///     The redo button template.
    /// </value>
    public DataTemplate RedoButtonTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the separator template.
    /// </summary>
    /// <value>
    ///     The separator template.
    /// </value>
    public DataTemplate SeparatorTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the undo button template.
    /// </summary>
    /// <value>
    ///     The undo button template.
    /// </value>
    public DataTemplate UndoButtonTemplate { get; set; }

    #endregion Public Properties
}