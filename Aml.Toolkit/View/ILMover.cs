// Copyright (c) 2017 AutomationML e.V.
using Aml.Toolkit.ViewModel.Graph;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Aml.Toolkit.View;

/// <summary>
///     This class implements anchor controls, displayed to move an internal link (when the link lines are shown)
/// </summary>
/// <seealso cref="Control" />
public class ILMover : Control
{
    #region Public Properties

    /// <summary>
    ///     Gets the adorner used to draw Internal link lines.
    /// </summary>
    /// <value>
    ///     The adorner.
    /// </value>
    public TreeViewLinksAdorner Adorner { get; }

    #endregion Public Properties

    #region Protected Methods

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var size = base.ArrangeOverride(arrangeBounds);

        ArrangeThumb(ToThumb, 3);
        ArrangeThumb(FromThumb, 0);

        return size;
    }

    #endregion Protected Methods

    #region Private Fields

    private Thumb FromThumb;
    private readonly ILMoverViewModel moverViewModel;

    private Thumb ToThumb;

    #endregion Private Fields

    #region Public Constructors

    static ILMover()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ILMover), new FrameworkPropertyMetadata(typeof(ILMover)));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ILMover" /> class.
    /// </summary>
    /// <param name="adorner">The adorner.</param>
    /// <param name="vm">The view model.</param>
    public ILMover(TreeViewLinksAdorner adorner, ILMoverViewModel vm)
    {
        DataContext = vm;
        moverViewModel = vm;
        Adorner = adorner;

        _ = InputBindings.Add(new KeyBinding(vm.DeleteInternalLinkCommand, Key.Delete, ModifierKeys.None));
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ToThumb = GetTemplateChild("ToThumb") as Thumb;
        FromThumb = GetTemplateChild("FromThumb") as Thumb;

        if (ToThumb == null || FromThumb == null)
        {
            return;
        }

        ToThumb.DragDelta += ToThumb_DragDelta;
        FromThumb.DragDelta += FromThumb_DragDelta;
        ToThumb.DragCompleted += ToThumb_DragCompleted;
        FromThumb.DragCompleted += FromThumb_DragCompleted;
        ToThumb.DragStarted += DragStart;
        FromThumb.DragStarted += DragStart;
        ArrangeThumb(ToThumb, 3);
        ArrangeThumb(FromThumb, 0);

        _ = Focus();
    }

    private void DragStart(object sender, DragStartedEventArgs e)
    {
        moverViewModel.Start();
    }

    #endregion Public Methods

    #region Private Methods

    private void ArrangeThumb(Thumb thumb, int index)
    {
        //Find the x-coordinate of the upper-left corner of the rectangle to draw.
        var x = moverViewModel.Points[index].X - 5;

        //Find y-coordinate of the upper-left corner of the rectangle to draw.
        var y = moverViewModel.Points[index].Y - 5;
        thumb.Arrange(new Rect(x, y, 10, 10));
    }

    private void FromThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!moverViewModel.IsSnapped || !moverViewModel.RedirectInternalLink())
        {
            moverViewModel.ResetPoints();
            ArrangeThumb(FromThumb, 0);
        }
        else
        {
            Adorner.ClearSelection(true);
        }
    }

    private void FromThumb_DragDelta(object sender, DragDeltaEventArgs args)
    {
        moverViewModel.UpdatePoint(0,
            new Point(moverViewModel.Points[0].X + args.HorizontalChange,
                moverViewModel.Points[0].Y + args.VerticalChange));
        InvalidateArrange();
        moverViewModel.DragOver(false);
    }

    private void ToThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!moverViewModel.IsSnapped || !moverViewModel.RedirectInternalLink())
        {
            moverViewModel.ResetPoints();
            ArrangeThumb(ToThumb, 3);
        }
        else
        {
            Adorner.ClearSelection(true);
        }
    }

    private void ToThumb_DragDelta(object sender, DragDeltaEventArgs args)
    {
        moverViewModel.UpdatePoint(3,
            new Point(moverViewModel.Points[3].X + args.HorizontalChange,
                moverViewModel.Points[3].Y + args.VerticalChange));
        InvalidateArrange();
        moverViewModel.DragOver(true);
    }

    #endregion Private Methods
}