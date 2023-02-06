using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Aml.Engine.CAEX;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.ViewModel.ValidationRules;
using Aml.Toolkit.XamlClasses;

namespace Aml.Toolkit.View;

/// <summary>
///     This class is used to enable editing of a tree node in the aml treeview.
/// </summary>
/// <seealso cref="ContentControl" />
public class EditableTreeNodeHeader : ContentControl
{
    #region Public Fields

    /// <summary>
    ///     The edit text property
    /// </summary>
    public static readonly DependencyProperty EditTextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(EditableTreeNodeHeader),
            new PropertyMetadata(""));

    #endregion Public Fields

    #region Public Properties

    /// <summary>
    ///     Gets or sets the edit text.
    /// </summary>
    /// <value>
    ///     The edit text.
    /// </value>
    public string EditText
    {
        get => (string)GetValue(EditTextProperty);
        set => SetValue(EditTextProperty, value);
    }

    #endregion Public Properties

    #region Public Constructors

    static EditableTreeNodeHeader()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTreeNodeHeader),
            new FrameworkPropertyMetadata(typeof(EditableTreeNodeHeader)));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EditableTreeNodeHeader" /> class.
    /// </summary>
    public EditableTreeNodeHeader()
    {
        if (DataContext is AMLNodeViewModel vm)
        {
            AddListener(vm);
            SetValue(EditTextProperty, vm.Name);
        }

        DataContextChanged += HeaderDataContextChanged;
    }

    #endregion Public Constructors

    #region Private Methods

    private void AddListener(AMLNodeViewModel vm)
    {
        PropertyChangedEventManager.AddHandler(vm, AMLNodeViewModelPropertyChanged,
            nameof(AMLNodeViewModel.IsInEditMode));

        //_modelEventListener = new DelegatingWeakEventListener((EventHandler<PropertyChangedEventArgs>)this.AMLNodeViewModelPropertyChanged);

        //GenericWeakEventManager.AddListener(
        //    vm,
        //      "PropertyChanged",
        //    _modelEventListener);
    }

    private void AMLNodeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AMLNodeViewModel.IsInEditMode) && ((AMLNodeViewModel)sender).IsInEditMode)
        {
            _ = Dispatcher?.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                var textInput = VisualTreeUtilities.FindElementWithName<TextBox>(this, "InputTextBox");

                // _textInput = GetTemplateChild("InputTextBox") as TextBox;
                if (textInput == null)
                {
                    return;
                }

                textInput.DataContext = this;
                textInput.SetValue(TextBox.TextProperty, ((AMLNodeViewModel)DataContext).Name);

                var bindingExpression = textInput.GetBindingExpression(TextBox.TextProperty);

                if (bindingExpression?.ParentBinding != null)
                {
                    foreach (var rule in bindingExpression.ParentBinding.ValidationRules
                                 .OfType<CAEXValidationRule>())
                    {
                        if (rule.AssignedObject == null)
                        {
                            continue;
                        }

                        if (textInput.FindResource("proxy") is BindingProxy proxy)
                        {
                            rule.AssignedObject.CaexObject = proxy.Data as CAEXBasicObject;
                        }
                    }
                }

                textInput.LostFocus -= OnLostFocusHandler;
                textInput.KeyDown -= OnKeyDownHandler;
                textInput.LostFocus += OnLostFocusHandler;
                textInput.KeyDown += OnKeyDownHandler;
                textInput.MouseLeave += OnLostFocusHandler;
                textInput.CaretIndex = textInput.Text.Length;
                _ = textInput.Focus();
                textInput.SelectAll();
                _ = textInput.CaptureMouse();
                //_tv = VisualTreeUtilities.FindParentWithType<TreeView>(_textInput);
                //_tv.Focusable = false;
            }));
        }
    }

    private void HeaderDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not AMLNodeViewModel vm)
        {
            return;
        }

        AddListener(vm);
        SetValue(EditTextProperty, vm.Name);
    }

    private void OnKeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            OnLostFocusHandler(sender, e);
        }
    }

    private void OnLostFocusHandler(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not TextBox tb)
            {
                return;
            }

            if (e is MouseEventArgs margs)
            {
                var pos = margs.GetPosition(tb);
                var result = VisualTreeHelper.HitTest(tb, pos);
                if (result != null)
                {
                    _ = tb.Focus();
                    return;
                }
            }

            if (DataContext is not AMLNodeViewModel nodeViewModel || !nodeViewModel.IsInEditMode)
            {
                return;
            }

            nodeViewModel.Name = GetValue(EditTextProperty) as string;
            nodeViewModel.IsInEditMode = false;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    #endregion Private Methods
}