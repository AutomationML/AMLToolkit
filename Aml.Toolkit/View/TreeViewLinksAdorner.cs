// *********************************************************************** Assembly : Aml.Toolkit
// Author : Josef Prinz Created : 03-10-2015
//
// Last Modified By : Josef Prinz Last Modified On : 04-23-2015 ***********************************************************************
// <copyright file="AMLTreeView.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary>
// </summary>
// ***********************************************************************

using Aml.Engine.CAEX;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.ViewModel.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Aml.Toolkit.View;

/// <summary>
///     Class, implementing the adorner, used to draw internal link lines
/// </summary>
/// <seealso cref="Adorner" />
public class TreeViewLinksAdorner : Adorner
{
    private const double Tolerance = 3.0;

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeViewLinksAdorner" /> class.
    /// </summary>
    /// <param name="amlTree">The aml tree.</param>
    /// <param name="clip">if set to <c>true</c> [clip].</param>
    public TreeViewLinksAdorner(AMLTreeView amlTree, bool clip = true) :
        base(amlTree)
    {
        AllowDrop = true;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
        VisualEdgeMode = EdgeMode.Aliased;
        IsClipEnabled = true;
        ClipWindow = clip;
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

        _ILGraph = new AmlTreeViewWithInternalLinkDrawing(amlTree);

        _redrawHandler = VisibiltyChanged;
        AddHandlers();
    }

    #endregion Public Constructors

    /// <summary>
    ///     Gets or sets the ui zoom factor.
    /// </summary>
    /// <value>
    ///     The zoom factor.
    /// </value>
    public static double ZoomFactor
    {
        get => IlGraph.ZoomFactor;
        set => IlGraph.ZoomFactor = value;
    }

    #region Public Properties

    /// <summary>
    ///     Gets a value indicating whether [clip window].
    /// </summary>
    /// <value><c>true</c> if [clip window]; otherwise, <c>false</c>.</value>
    public bool ClipWindow { get; }

    #endregion Public Properties

    #region Protected Properties

    /// <summary>
    ///     Ruft die Anzahl der sichtbaren untergeordneten Elemente innerhalb dieses Elements ab.
    /// </summary>
    protected override int VisualChildrenCount => _visualChildren?.Count ?? 0;

    #endregion Protected Properties

    #region Internal Classes

    internal class Connector
    {
        #region Public Properties

        public Vertex Vertex { get; set; }
        public Rect Visual { get; set; }

        #endregion Public Properties
    }

    #endregion Internal Classes

    #region Private Fields

    private readonly AmlTreeViewWithInternalLinkDrawing _ILGraph;

    private readonly RoutedEventHandler _redrawHandler;

    private ILMoverViewModel _ilMover;

    private bool _isSuspended;

    private bool _isValid = true;

    private DateTime _renderTimer = DateTime.Now;

    // To store and manage the adorner's visual children.
    private VisualCollection _visualChildren;

    private double verticalOffset;

    #endregion Private Fields

    #region Internal Properties

    internal Edge SelectedEdge { get; private set; }

    internal Connector SelectedFrom { get; set; }

    internal InternalLinkType SelectedLink { get; private set; }

    internal Connector SelectedTo { get; set; }

    #endregion Internal Properties

    #region Public Methods

    /// <summary>
    ///     Invalidates this instance. If force is set, suspended layout updates are ignored.
    /// </summary>
    /// <param name="force">if set to <c>true</c> [force].</param>
    public void Invalidate(bool force)
    {
        if (force)
        {
            _isSuspended = false;
        }

        InvalidateVisual();
    }

    /// <summary>
    ///     Redraws this instance.
    /// </summary>
    public void Redraw()
    {
        if (_isSuspended)
        {
            return;
        }

        if (!ShouldRender())
        {
            return;
        }

        _renderTimer = DateTime.Now;
        InvalidateVisual();
    }

    private int _renderCall;

    /// <summary>
    ///     Renders the specified drawing context.
    /// </summary>
    /// <param name="drawingContext">The drawing context.</param>
    public void Render(DrawingContext drawingContext)
    {
        if (AdornedElement is AMLTreeView treeView && treeView.ScrollViewer.ViewportWidth > 0 && ClipWindow)
        {
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, treeView.ScrollViewer.ViewportWidth,
                treeView.ScrollViewer.ViewportHeight)));
        }

        // continue drawing

        _isValid = true;
        if (!_ILGraph.Graph.HasVertices)
        {
            return;
        }

        Debug.WriteLine($"{_renderCall++}. Render");

        //this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
        // {
        _ILGraph.Graph.Draw(drawingContext, (AMLTreeView)AdornedElement, false);

        if (SelectedEdge != null)
        {
            Debug.WriteLine(SelectedEdge.StartPoint.Item.Name);
            Debug.WriteLine(SelectedEdge.EndPoint.Item.Name);
            if (SelectedEdge.StartPoint.IsVisible(false) ||
                SelectedEdge.EndPoint.IsVisible(false))
            {
                HighlightEdge();
            }
        }

        _renderTimer = DateTime.Now;
        //}));
    }

    #endregion Public Methods

    #region Internal Methods

    internal void Clear()
    {
        _ILGraph.Clear();
        ClearSelection(true);
        Invalidate();
    }

    internal void ClearSelection(bool clearAll)
    {
        if (clearAll)
        {
            SelectedEdge = null;
            SelectedLink = null;
            SelectedFrom = null;
            SelectedTo = null;
        }

        _visualChildren?.Clear();

        _ilMover = null;
        _visualChildren = null;
    }

    /// <summary>
    ///     connect the source and target with an internal link line
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void Connect(AMLNodeViewModel source, AMLNodeViewModel target)
    {
        if (source != target)
        {
            _ILGraph.Connect(source, target);
        }
    }

    internal void DisConnectAll(AMLNodeViewModel source)
    {
        _ILGraph.DisConnectAll(source);

        if (SelectedLink?.Document == null)
        {
            ClearSelection(true);
        }
        else if (SelectedLink?.AInterface?.Node == source?.CAEXNode ||
                 SelectedLink?.BInterface?.Node == source?.CAEXNode)
        {
            ClearSelection(true);
        }
    }

    internal IEnumerable<AMLNodeViewModel> GetPartnerNodes(AMLNodeViewModel source)
    {
        _ = _ILGraph.Graph.Vertices.TryGetValue(source, out var sourceVertex);
        if (sourceVertex == null)
        {
            yield break;
        }

        var edges = _ILGraph.Graph.Edges[sourceVertex.Index];
        for (var index = 0; index < edges.Count; index++)
        {
            var edge = edges[index];
            if (edge != null)
            {
                yield return edge.EndPoint.Item != source ? edge.EndPoint.Item : edge.StartPoint.Item;
            }
        }
    }

    internal void InvalidateSelection()
    {
        ClearSelection(true);
        Invalidate();
    }

    internal void RemoveVertex(AMLNodeViewModel node)
    {
        _ILGraph.Graph.RemoveVertex(node);
    }

    internal void Resume()
    {
        _isSuspended = false;
    }

    internal InternalLinkType SelectLink(Point pt)
    {
        if (_ILGraph?.Graph?.Edges == null)
        {
            return null;
        }

        if (AdornedElement is AMLTreeView tv && tv.TreeViewModel.IsReadonly)
        {
            return null;
        }

        ClearSelection(true);
        for (var i = 0; i < _ILGraph.Graph.Edges.Count; i++)
        {
            var edgeList = _ILGraph.Graph.Edges[i];
            for (var j = 0; j < edgeList.Count; j++)
            {
                var edge = edgeList[j];
                if (edge == null)
                {
                    continue;
                }

                Geometry line = new LineGeometry(edge.Points[0], edge.Points[1]);


                var isSelected = line.FillContains(pt, Tolerance, ToleranceType.Absolute);
                //isSelected = line.StrokeContains(edge.Pen, pt);
                if (!isSelected)
                {
                    line = new LineGeometry(edge.Points[1], edge.Points[2]);
                    isSelected = line.FillContains(pt, Tolerance, ToleranceType.Absolute);
                }

                if (!isSelected)
                {
                    line = new LineGeometry(edge.Points[2], edge.Points[3]);

                    isSelected = line.FillContains(pt, Tolerance, ToleranceType.Absolute);
                    // isSelected = line.StrokeContains(edge.Pen, pt);
                }

                if (!isSelected)
                {
                    continue;
                }

                if (edge.StartPoint.Item is not AMLNodeWithClassReference a ||
                    edge.EndPoint.Item is not AMLNodeWithClassReference b)
                {
                    return null;
                }

                SelectedLink = a.GetInternalLink(b);
                SelectedEdge = edge;

                InvalidateVisual();
                return SelectedLink;
            }
        }

        InvalidateVisual();
        return SelectedLink;
    }


    /// <summary>
    ///     Selects the internal link.
    /// </summary>
    /// <param name="internalLink">The internal link.</param>
    public void SelectInternalLink(InternalLinkType internalLink)
    {
        ClearSelection(true);
        SelectedLink = internalLink;
        for (var i = 0; i < _ILGraph.Graph.Edges.Count; i++)
        {
            var edgeList = _ILGraph.Graph.Edges[i];
            for (var j = 0; j < edgeList.Count; j++)
            {
                var edge = edgeList[j];
                if (edge == null)
                {
                    continue;
                }

                if (edge.StartPoint.Item is not AMLNodeWithClassReference a ||
                    edge.EndPoint.Item is not AMLNodeWithClassReference b)
                {
                    continue;
                }

                if (a.CAEXObject != null && a.CAEXObject.Equals(internalLink.AInterface) &&
                    b.CAEXObject != null && b.CAEXObject.Equals(internalLink.BInterface))
                {
                    SelectedEdge = edge;
                    InvalidateVisual();
                    return;
                }

                if (a.CAEXObject != null && a.CAEXObject.Equals(internalLink.BInterface) &&
                    b.CAEXObject != null && b.CAEXObject.Equals(internalLink.AInterface))
                {
                    SelectedEdge = edge;
                    InvalidateVisual();
                    return;
                }
            }
        }
    }

    /// <summary>
    ///     Suspends the automatic layout updates.
    /// </summary>
    internal void Suspend()
    {
        _isSuspended = true;
    }

    #endregion Internal Methods

    #region Protected Methods

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (SelectedEdge != null)
        {
            ((ILMover)_visualChildren?[0])?.Arrange(new Rect(finalSize));
            return finalSize;
        }

        return base.ArrangeOverride(finalSize);
    }

    /// <summary>
    ///     Überschreibt <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />,
    ///     und ein untergeordnetes Element am angegebenen Index aus einer Auflistung von
    ///     untergeordneten Elementen zurückgegeben.
    /// </summary>
    /// <param name="index">
    ///     Der nullbasierte Index des angeforderten untergeordneten Elements in der Auflistung.
    /// </param>
    /// <returns>
    ///     Das angeforderte untergeordnete Element. Es sollte nicht <see langword="null" />
    ///     zurückgeben; wenn der angegebene Index außerhalb des Bereichs liegt, wird eine Ausnahme ausgelöst.
    /// </returns>
    protected override Visual GetVisualChild(int index) => _visualChildren?[index];

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var result = base.MeasureOverride(constraint);
        // ... add custom measure code here if desired ...
        InvalidateVisual();
        return result;
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (_isSuspended)
        {
            return;
        }

        //if (!ShouldRender())
        //    return;
        ////RemoveHandlers();
        Render(drawingContext);
        //AddHandlers();
    }

    #endregion Protected Methods

    #region Private Methods

    private void AddHandlers()
    {
        if (AdornedElement is not AMLTreeView treeView)
        {
            return;
        }

        treeView.AddHandler(TreeViewItem.CollapsedEvent, _redrawHandler);
        treeView.AddHandler(TreeViewItem.ExpandedEvent, _redrawHandler);
        treeView.AddHandler(ScrollViewer.ScrollChangedEvent, _redrawHandler);
    }

    // Resizing adorner uses Thumbs for visual elements.
    // The Thumbs have built-in mouse input handling.
    //Thumb top,  bottom;
    private void HighlightEdge()
    {
        if (AdornedElement is AMLTreeView tv && tv.TreeViewModel.IsReadonly)
        {
            return;
        }

        if (SelectedEdge == null)
        {
            return;
        }

        var newMover = new ILMoverViewModel(AdornedElement, SelectedLink, SelectedEdge);
        var redraw = false;

        if (_ilMover != null && _ilMover.Points.Count == newMover.Points.Count)
        {
            for (var i = 0; i < _ilMover.Points.Count; i++)
            {
                if (!_ilMover.Points[i].Equals(newMover.Points[i]))
                {
                    _ilMover.Points[i] = newMover.Points[i];
                    redraw = true;
                }
            }
        }
        else
        {
            redraw = true;
        }

        if (!redraw)
        {
            return;
        }


        if (_ilMover == null)
        {
            _ilMover = newMover;
            _visualChildren = new VisualCollection(this)
            {
                new ILMover(this, _ilMover)
            };
        }
        else
        {
            ((ILMover)_visualChildren[0]).InvalidateArrange();
        }

        InvalidateArrange();
    }

    private void Invalidate()
    {
        if (!_isValid)
        {
            return;
        }

        _isValid = false;
        Redraw();
    }


    // verhindert, dass zu oft new gezeichnet wird
    private bool ShouldRender()
    {
        var currentTime = DateTime.Now;
        var diff = currentTime - _renderTimer;

        if (diff.TotalMilliseconds < 40)
        {
            return false;
        }

        _renderTimer = currentTime;
        return true;
    }

    private void VisibiltyChanged(object sender, RoutedEventArgs e)
    {
        //RemoveHandlers();

        if (_isSuspended)
        {
            return;
        }

        //if (!ShouldRender())
        //{
        //    return;
        //}

        _renderTimer = DateTime.Now;

        e.Handled = true;
        switch (e.OriginalSource)
        {
            case ScrollViewer sv:
                if (Math.Abs(sv.VerticalOffset - verticalOffset) < 5)
                {
                    return;
                }

                verticalOffset = sv.VerticalOffset;
                break;

            case TreeViewItem:
                break;
        }

        //_renderTimer -= TimeSpan.FromMilliseconds(120);
        ClearSelection(false);
        InvalidateVisual();
    }

    #endregion Private Methods
}