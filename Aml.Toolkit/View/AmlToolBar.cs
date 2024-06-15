using Aml.Toolkit.Operations;
using System.Windows;

namespace Aml.Toolkit.View;

/// <summary>
///     Class defines a specific aml tool bar
/// </summary>
/// <seealso cref="StandardModernToolBar" />
public class AMLToolBar : StandardModernToolBar
{
    #region Public Constructors

    static AMLToolBar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AMLToolBar),
            new FrameworkPropertyMetadata(typeof(AMLToolBar)));
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        SeparatorItem.AddSeparatorItem(ToolBarOperations);
    }

    #endregion Public Methods
}