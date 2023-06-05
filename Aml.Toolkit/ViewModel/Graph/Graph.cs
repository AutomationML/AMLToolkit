using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Aml.Engine.CAEX;
using Aml.Skins;
using Aml.Toolkit.View;

namespace Aml.Toolkit.ViewModel.Graph;

/// <summary>
///     Class, defining the properties and methods of an internal link line graph
/// </summary>
public class IlGraph
{
    internal static double ZoomFactor = 1.0;

    #region Internal Constructors

    internal IlGraph()
    {
        Vertices = new Dictionary<AMLNodeViewModel, Vertex>();
        Edges = new List<List<Edge>>();
    }

    #endregion Internal Constructors

    #region Public Properties

    /// <summary>
    ///     Gets the maximum width.
    /// </summary>
    /// <value>
    ///     The maximum width.
    /// </value>
    public double MaxWidth { get; private set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Gets the brush for connecting links.
    /// </summary>
    /// <param name="classPath">The class path.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static Brush GetBrushForConnection(string classPath, out string name)
    {
        var selectedBrush = (0, AvailableLinkBrushes[0]);

        if (!string.IsNullOrEmpty(classPath))
        {
            if (!InterfaceBrushes.TryGetValue(classPath, out selectedBrush))
            {
                LastUsedBrush++;
                if (LastUsedBrush < AvailableLinkBrushes.Count)
                {
                    selectedBrush = (LastUsedBrush, AvailableLinkBrushes[LastUsedBrush]);
                }

                InterfaceBrushes.Add(classPath, selectedBrush);
            }
        }

        try
        {
            name = AvailableLinkBrushes[selectedBrush.Item1].ToString();
        }
        catch
        {
            name = string.Empty;
        }

        return selectedBrush.Item2;
    }

    #endregion Public Methods

    #region Public Constructors

    private static void SetLinkColours(ResourceDictionary dictionary)
    {
        AvailableLinkBrushes = GenerateBrushes(dictionary);
    }

    private static List<Brush> GenerateBrushes(ResourceDictionary dictionary)
    {
        try
        {
            List<Brush> brushes = new();
            foreach (var key in dictionary.Keys.OfType<string>().OrderBy(s => s))
            {
                if (key.StartsWith("LINK"))
                {
                    brushes.Add((Brush)dictionary[key]);
                }
            }

            return brushes;
        }
        catch
        { 
            return null;
        }
    }

    static IlGraph()
    {
        AMLApp.ApplyColors = SetLinkColours;
    }

    #endregion Public Constructors

    #region Internal Fields

    internal static readonly Dictionary<string, (int, Brush)> InterfaceBrushes =
        new();

    internal static double PenThickness = 0.8;
    internal static Pen defaultPen;
    internal static bool useColor;
    internal readonly Dictionary<Edge, Range<double>> Ranges = new();

    #endregion Internal Fields

    #region Private Fields

    private static List<Brush> _availableBrushes;

    private static List<Brush> AvailableLinkBrushes
    {
        get => _availableBrushes ??= GenerateBrushes(AMLApp.ThemeColors);
        set => _availableBrushes = value;
    }

    //= new Brush[]
    //{
    //    (Brush)new BrushConverter().ConvertFrom("#ffaacc"),
    //    Brushes.Blue,
    //    Brushes.Green,
    //    Brushes.Red,
    //    Brushes.DarkMagenta,
    //    Brushes.Orange,
    //    Brushes.Goldenrod,
    //    Brushes.Fuchsia,
    //    Brushes.DarkCyan,
    //    Brushes.Brown,
    //    Brushes.Crimson,
    //    Brushes.Aqua,
    //    Brushes.Violet,
    //    Brushes.YellowGreen,
    //    Brushes.Turquoise,
    //    Brushes.Tomato,
    //    Brushes.Teal,
    //    Brushes.Tan,
    //    Brushes.Chartreuse
    //};
    private static int LastUsedBrush = -1;

    private static int yspace = 5;

    #endregion Private Fields

    #region Internal Properties

    internal List<List<Edge>> Edges { get; set; }
    internal Dictionary<AMLNodeViewModel, Vertex> Vertices { get; set; }


    /// <summary>
    ///     Gets a value indicating whether this instance has vertices.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has vertices; otherwise, <c>false</c>.
    /// </value>
    public bool HasVertices
    {
        get
        {
            if (Vertices is { Count: > 0 })
            {
                var keys = Vertices.Keys;
                foreach (var node in keys)
                {
                    if (node.DeletedInDocument)
                    {
                        RemoveVertex(node);
                    }

                    if (node.IsExpanded && node.CAEXObject is not ExternalInterfaceType)
                    {
                        RemoveVertex(node);
                    }
                }

                return Vertices.Count > 0;
            }

            return false;
        }
    }

    #endregion Internal Properties

    #region Internal Methods

    internal static bool LineSegementsIntersect(Vector2D p, Vector2D p2, Vector2D q, Vector2D q2,
        out Vector2D intersection, bool considerCollinearOverlapAsIntersect = false)
    {
        intersection = new Vector2D();

        var r = p2 - p;
        var s = q2 - q;
        var rxs = r.Cross(s);
        var qpxr = (q - p).Cross(r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (Vector2D.IsZero(rxs) && Vector2D.IsZero(qpxr))
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            if (!considerCollinearOverlapAsIntersect)
            {
                return false;
            }

            return (0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s);

            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (Vector2D.IsZero(rxs) && !Vector2D.IsZero(qpxr))
        {
            return false;
        }

        // t = (q - p) x s / (r x s)
        var t = (q - p).Cross(s) / rxs;

        // u = (q - p) x r / (r x s)

        var u = (q - p).Cross(r) / rxs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (Vector2D.IsZero(rxs) || !(0 <= t) || !(t <= 1) || !(0 <= u) || !(u <= 1))
        {
            return false;
        }

        // We can calculate the intersection point using either t or u.
        intersection = p + t * r;

        // An intersection was found.
        return true;

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
    }

    internal bool AddEdge(Vertex sourceVertex, Vertex targetVertex)
    {
        if (Edges[sourceVertex.Index][targetVertex.Index] != default(Edge))
        {
            return false;
        }

        var edge = new Edge(this) { StartPoint = sourceVertex, EndPoint = targetVertex };
        Edges[sourceVertex.Index][targetVertex.Index] = edge;
        Edges[targetVertex.Index][sourceVertex.Index] = edge;

        return true;
    }

    internal Vertex AddVertex(AMLNodeViewModel item)
    {
        var v = new Vertex { Item = item, Index = Edges.Count };
        Vertices.Add(item, v);
        Edges.Add(new List<Edge>());
        foreach (var edge in Edges)
        {
            while (edge.Count < Edges.Count)
            {
                edge.Add(default);
            }
        }

        return v;
    }

    internal void CalculateVisibelRanges(AMLTreeView treeView, bool clip)
    {
        MaxWidth = 0;

        // initialization of collections
        Ranges.Clear();

        foreach (var vertex in Vertices.Values)
        {
            vertex.isVisited = false;
        }

        for (var i = 0; i < Edges.Count; i++)
        {
            for (var j = 0; j < Edges[i].Count; j++)
            {
                if (Edges[i][j] != null)
                {
                    Edges[i][j].IsVisited = false;
                }
            }
        }

        var removed = Vertices.Keys.Where(c => c.CAEXNode.Parent == null).ToList();
        foreach (var verticeKey in removed)
        {
            RemoveVertex(verticeKey);
        }

        CheckVertices();

        // calculation
        foreach (var vertex in Vertices)
        {
            var treeViewItem = vertex.Key;

            if (clip && !vertex.Value.IsVisible())
            {
                continue;
            }

            for (var i = 0; i < Edges[vertex.Value.Index].Count; i++)
            {
                var edge = Edges[vertex.Value.Index][i];

                if (edge == default(Edge) || edge.IsVisited)
                {
                    continue;
                }

                edge.IsVisited = true;

                if ((edge.StartPoint.Item != treeViewItem || (clip && !edge.EndPoint.IsVisible())) &&
                    (edge.EndPoint.Item != treeViewItem || (clip && !edge.StartPoint.IsVisible())))
                {
                    continue;
                }

                if (!CalculateLinkLine(edge, treeView, out var width))
                {
                    continue;
                }

                if (width > MaxWidth)
                {
                    MaxWidth = width;
                }

                Ranges.Add(edge,
                    edge.Range = new Range<double>(edge.Points[0].Y, edge.Points[3].Y));
            }
        }

        foreach (var edge in Ranges.Keys.ToList())
        {
            edge.Points[1].X = MaxWidth + Vertex.widthOffset;
            edge.Points[2].X = edge.Points[1].X;
        }
    }

    internal void Clear()
    {
        Vertices.Clear();
        Edges.Clear();
    }

    internal void Draw(DrawingContext drawingContext, AMLTreeView treeView, bool clip = true)
    {
        var VerticalLines = new List<Tuple<Pen, Vector2D, Vector2D>>();
        var HorizontalLines = new List<Tuple<Pen, Vector2D, Vector2D>>();

        //zoomFactor = (double)Util.AMLConfig.Instance.GetPropertyValue(Util.AMLConfig.SET_ZOOM_FACTOR);
        useColor = treeView.TreeViewModel.TreeViewLayout.DrawColoredLines;

        defaultPen = new Pen(AvailableLinkBrushes[0], 1 / ZoomFactor);
        if (treeView.TreeViewModel.TreeViewLayout.DrawDashedLines)
        {
            defaultPen.DashStyle = DashStyles.Dash;
        }

        var verticalOffset = treeView.TreeViewModel.TreeViewLayout.VerticalLineOffset;
        var horizontalOffset = treeView.TreeViewModel.TreeViewLayout.HorizontalLineOffset;

        yspace = verticalOffset == 0 ? 0 : 5;
        var testIntersections = false;

        var jumpMode = treeView.TreeViewModel.TreeViewLayout.JumpMode;

        if (jumpMode != AMLLayout.LineJumpModeEnum.None)
        {
            horizontalOffset = 12;
            testIntersections = true;
        }

        var yCoordinates = new Dictionary<double, int>();

        CalculateVisibelRanges(treeView, clip);

        double Xoffset = 0;
        var Yoffset = verticalOffset;
        var WindowWidth = treeView.ActualWidth - 20;

        while (Ranges.Count > 0)
        {
            var painted = NonOverlappingRanges();

            // take the closest range
            if (painted.Count == 0)
            {
                double minLength = 1000000;
                Edge selectedEdge = null;

                foreach (var edge in Ranges.Keys.Where(edge => edge.Length < minLength))
                {
                    minLength = edge.Length;
                    selectedEdge = edge;
                }

                painted.Add(selectedEdge);
            }

            for (var index = 0; index < painted.Count; index++)
            {
                var paint = painted[index];
                if (!yCoordinates.TryGetValue(paint.Points[0].Y, out var outLines))
                {
                    yCoordinates.Add(paint.Points[0].Y, 1);
                }
                else
                {
                    yCoordinates[paint.Points[0].Y]++;
                }

                paint.Points[0].Y -= outLines * Yoffset;
                paint.Points[1].Y = paint.Points[0].Y;
                paint.Points[1].X += Xoffset;

                if (!yCoordinates.TryGetValue(paint.Points[3].Y, out outLines))
                {
                    yCoordinates.Add(paint.Points[3].Y, 1);
                }
                else
                {
                    yCoordinates[paint.Points[3].Y]++;
                }

                paint.Points[2].Y += outLines * Yoffset;
                paint.Points[2].X += Xoffset;
                paint.Points[3].Y = paint.Points[2].Y;

                _ = Ranges.Remove(paint);

                if (clip && (!(paint.Points[1].X < WindowWidth) || !(paint.Points[1].Y > 0)))
                {
                    continue;
                }

                HorizontalLines.Add(Tuple.Create(paint.Pen, new Vector2D(paint.Points[0]),
                    new Vector2D(paint.Points[1])));
                HorizontalLines.Add(Tuple.Create(paint.Pen, new Vector2D(paint.Points[3]),
                    new Vector2D(paint.Points[2])));
                VerticalLines.Add(Tuple.Create(paint.Pen, new Vector2D(paint.Points[1]),
                    new Vector2D(paint.Points[2])));
            }

            Xoffset += horizontalOffset;
        }

        var sortedVLines = VerticalLines.OrderBy(v => v.Item2.X).ToList();
        var sortedHLines = HorizontalLines.OrderBy(v => v.Item2.Y).ToList();

        foreach (var (item1, item2, item3) in sortedVLines)
        {
            drawingContext.DrawLine(item1, item2.P, item3.P);
        }

        foreach (var (item1, vector2D, vector2D1) in sortedHLines)
        {
            if (testIntersections)
            {
                var Intersections = new List<Vector2D>();

                foreach (var (_, item2, item3) in sortedVLines.Where(vline =>
                             !vline.Item2.Equals(vector2D1) && !vline.Item3.Equals(vector2D1)))
                {
                    if (LineSegementsIntersect(item2, item3, vector2D, vector2D1,
                            out var intersection))
                    {
                        Intersections.Add(intersection);
                    }
                }

                if (Intersections.Count == 0)
                {
                    drawingContext.DrawLine(item1, vector2D.P, vector2D1.P);
                    continue;
                }

                var start = vector2D.P;
                foreach (var intersection in Intersections) //.OrderBy(v => v.X))
                {
                    drawingContext.DrawLine(item1, start, new Point(intersection.X - 4, intersection.Y));
                    start = new Point(intersection.X + 4, intersection.Y);

                    if (jumpMode != AMLLayout.LineJumpModeEnum.JumpOver)
                    {
                        continue;
                    }

                    var geometry = new StreamGeometry
                    {
                        FillRule = FillRule.EvenOdd
                    };

                    // Open a StreamGeometryContext that can be used to describe this StreamGeometry object's contents.
                    using (var ctx = geometry.Open())
                    {
                        // Set the begin point of the shape.
                        ctx.BeginFigure(new Point(intersection.X - 4, intersection.Y), false /* is filled */,
                            false /* is closed */);

                        // Create an arc. Draw the arc from the begin point to 200,100 with the specified parameters.
                        ctx.ArcTo(new Point(intersection.X + 4, intersection.Y), new Size(4, 4),
                            90 /* rotation angle */, false /* is large arc */,
                            SweepDirection.Clockwise, true /* is stroked */, true /* is smooth join */);
                    }

                    // Freeze the geometry (make it unmodifiable)
                    // for additional performance benefits.
                    geometry.Freeze();
                    drawingContext.DrawGeometry(null, new Pen(item1.Brush, 1 / ZoomFactor), geometry);
                }

                drawingContext.DrawLine(item1, start, vector2D1.P);
            }
            else
            {
                drawingContext.DrawLine(item1, vector2D.P, vector2D1.P);
            }
        }
    }

    internal List<Edge> NonOverlappingRanges()
    {
        var notOverlapping = new ConcurrentBag<Edge>();

        _ = Parallel.ForEach(Ranges, range =>
        {
            var overlaps = Ranges.Values.Any(other => other != range.Value &&
                                                      range.Value.IsOverlapped(other));

            if (!overlaps)
            {
                notOverlapping.Add(range.Key);
            }
        });

        // sort ascending with length
        return notOverlapping.OrderBy(edge => edge.Length).ToList();
    }

    internal void RemoveEdge(Vertex sourceVertex, Vertex targetVertex)
    {
        Edges[sourceVertex.Index][targetVertex.Index] = default;
        Edges[targetVertex.Index][sourceVertex.Index] = default;

        if (Edges[sourceVertex.Index].All(i => i == default))
        {
            sourceVertex.Item.RaisePropertyChanged("HasLinks");
        }

        if (Edges[sourceVertex.Index].All(i => i == default))
        {
            targetVertex.Item.RaisePropertyChanged("HasLinks");
        }
    }

    internal void RemoveVertex(AMLNodeViewModel item)
    {
        if (!Vertices.TryGetValue(item, out var vertex))
        {
            return;
        }

        foreach (var edge in Edges[vertex.Index].Where(e => e != null).ToList())
        {
            if (vertex == edge?.StartPoint && edge.EndPoint != null)
            {
                RemoveEdge(vertex, edge.EndPoint);
            }
            else if (vertex == edge?.EndPoint && edge.StartPoint != null)
            {
                RemoveEdge(vertex, edge.StartPoint);
            }
        }

        _ = Vertices.Remove(item);
    }

    //internal void RemoveVertexWithNoEdges(AMLNodeViewModel node, Vertex vertex)
    //{
    //    //if (Edges.Any(e => e.Any(Edge => Edge.from == vertex || Edge.to == vertex)))
    //    //    return;

    //    //Vertices.Remove(node);
    //}

    #endregion Internal Methods

    #region Private Methods

    private static Point LocalPosToAncestorPos(FrameworkElement child, FrameworkElement ancestor)
    {
        if (child == null)
        {
            return new Point(0, 0);
        }

        var localPos = new Point(child.ActualWidth, child.ActualHeight * 0.5);
        try
        {
            var bounds = child.TransformToAncestor(ancestor)
                .TransformBounds(new Rect(0.0, 0.0, child.ActualWidth, child.ActualHeight));
            var rect = new Rect(0.0, 0.0, ancestor.ActualWidth, ancestor.ActualHeight);

            if (!rect.Contains(bounds.TopRight))
            {
            }

            return child.TransformToAncestor(ancestor).Transform(localPos);
        }
        catch (Exception)
        {
            return localPos;
        }
    }


    //    FrameworkElement container = item.AMLTreeViewModel.Parent as FrameworkElement;
    //    Rect bounds = item.TransformToAncestor(container).TransformBounds(new Rect(new Point(0, 0), new Size(item.DesiredSize.Width + widthOffset, item.DesiredSize.Height)));
    //    Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
    //    return (vertex.isVisible = rect.Contains(bounds.TopRight) && rect.Contains(bounds.BottomRight));
    //}
    private bool CalculateLinkLine(Edge edge, AMLTreeView treeView, out double width)
    {
        width = 0;
        edge.Pen = defaultPen;

        // check, if the defined class is selected for drawing
        if (edge.StartPoint.Item is AMLNodeWithClassReference cref && !string.IsNullOrEmpty(cref.ClassReference))
        {
            var name = Path.GetFileNameWithoutExtension(cref.ClassReference);

            if (treeView.TreeViewModel.FilteredClasses != null)
            {
                var (IsSelected, Name) =
                    treeView.TreeViewModel.FilteredClasses.FirstOrDefault(ic => ic.Name == name);
                if (!string.IsNullOrEmpty(Name))
                {
                    //edge.StartPoint?.Item?.RaisePropertyChanged("HasLinks");
                    //edge.EndPoint?.Item?.RaisePropertyChanged("HasLinks");

                    if (!IsSelected)
                    {
                        return false;
                    }

                    if (useColor)
                    {
                        edge.Pen = new Pen(GetBrushForConnection(
                                cref.CAEXNode.Attribute(cref.ClassPathReferenceAttribute)?.Value, out _),
                            1 / ZoomFactor);
                        if (treeView.TreeViewModel.TreeViewLayout.DrawDashedLines)
                        {
                            edge.Pen.DashStyle = DashStyles.Dash;
                        }
                    }
                }
            }
        }


        edge.Pen.LineJoin = PenLineJoin.Round;

        var from = edge.StartPoint.VisibleItem(out var isFromParent);
        var to = edge.EndPoint.VisibleItem(out var isToParent);

        if ((from == null && to == null) || (isFromParent && isToParent))
        {
            return false;
        }

        from ??= edge.StartPoint.Item;
        to ??= edge.EndPoint.Item;

        if (from != edge.StartPoint.Item && to != edge.EndPoint.Item)
        {
            return false;
        }

        var tFrom = from?.TreeViewItem;
        var tTo = to?.TreeViewItem;


        if (tFrom == null && tTo == null)
        {
            return false;
        }

        var a = tFrom != null
            ? LocalPosToAncestorPos((ContentPresenter)tFrom.Template.FindName("PART_Header", tFrom), treeView)
            : new Point(0, 0);
        var d = tTo != null
            ? LocalPosToAncestorPos((ContentPresenter)tTo.Template.FindName("PART_Header", tTo), treeView)
            : new Point(0, 0);

        if (tFrom == null)
        {
            a.X = d.X;
            var firstNode = ((AMLTreeViewModel)treeView.DataContext).FirstNode(from, to);

            a.Y = firstNode.Equals(to) ? treeView.ActualHeight : -10;
        }

        if (tTo == null)
        {
            d.X = a.X;
            var firstNode = ((AMLTreeViewModel)treeView.DataContext).FirstNode(from, to);

            d.Y = firstNode.Equals(from) ? treeView.ActualHeight : -10;
        }

        var b = new Point(a.X, a.Y);
        var c = new Point(d.X, d.Y);

        if (a.Y < d.Y)
        {
            a.Y += yspace;
            b.Y += yspace;
            c.Y -= yspace;
            d.Y -= yspace;
            edge.Points[0] = a;
            edge.Points[1] = b;
            edge.Points[2] = c;
            edge.Points[3] = d;

            width = CalculateMaxWidth(edge.StartPoint, edge.EndPoint, Math.Max(a.X, d.X), treeView);
        }
        else
        {
            a.Y -= yspace;
            b.Y -= yspace;
            c.Y += yspace;
            d.Y += yspace;

            edge.Points[0] = d;
            edge.Points[1] = c;
            edge.Points[2] = b;
            edge.Points[3] = a;

            width = CalculateMaxWidth(edge.EndPoint, edge.StartPoint, Math.Max(a.X, d.X), treeView);
        }

        edge.Length = edge.Points[3].Y - edge.Points[0].Y;

        return true;
    }

    //    if (!item.IsVisibleAndNotFiltered)
    //        return (vertex.isVisible = false);
    private double CalculateMaxWidth(Vertex vertex1, Vertex vertex2, double width, AMLTreeView treeView)
    {
        // start at the parent

        var source = vertex1.VisibleItem(out _) ?? vertex1.Item;
        var parent = source.Parent;
        var target = vertex2.VisibleItem(out _) ?? vertex2.Item;
        var itemFound = false;

        // Visuals
        var visuals = new List<AMLNodeViewModel>();

        if (source.IsExpanded)
        {
            itemFound = FindItem(source.VisibleChildren.ToList(), target, visuals);
        }

        while (parent != null && !itemFound)
        {
            itemFound = FindItemInParentCollection(source, target, visuals);
            if (itemFound)
            {
                continue;
            }

            source = parent;
            parent = source.Parent;
        }

        var maxWidth = width;

        if (!itemFound)
        {
            return maxWidth;
        }

        for (var index = 0; index < visuals.Count; index++)
        {
            var visual = visuals[index];
            var treeItem = visual.TreeViewItem;
            if (!visual.IsVisible || !(treeItem?.IsVisible ?? false))
            {
                continue;
            }

            var pos = LocalPosToAncestorPos(
                (ContentPresenter)treeItem.Template.FindName("PART_Header", treeItem), treeView);

            if (pos.X > maxWidth)
            {
                maxWidth = pos.X;
            }
        }

        return maxWidth;
    }

    private void CheckVertices()
    {
        foreach (var vertex in Vertices.ToList())
        {
            if (vertex.Key.CAEXNode == null)
            {
                _ = Vertices.Remove(vertex.Key);
                continue;
            }

            var item = vertex.Key.Tree.SelectCaexNode(vertex.Key.CAEXNode, false);
            if (item == null || item == vertex.Key || Vertices.ContainsKey(item))
            {
                continue;
            }

            Vertices.Add(item, vertex.Value);
            vertex.Value.Item = item;
            _ = Vertices.Remove(vertex.Key);
        }
    }

    //    var item = vertex.Item;
    //    vertex.isVisited = true;
    private bool FindItem(IList<AMLNodeViewModel> items, AMLNodeViewModel item, List<AMLNodeViewModel> Visuals)
    {
        for (var i = 0; i < items.Count; i++)
        {
            Visuals.Add(items[i]);
            if (items[i] == item)
            {
                return true;
            }

            if (!items[i].IsExpanded)
            {
                continue;
            }

            if (FindItem(items[i].VisibleChildren.ToList(), item, Visuals))
            {
                return true;
            }
        }

        return false;
    }

    //private static bool isVisible(Vertex vertex)
    //{
    //    if (vertex.isVisited)
    //        return vertex.isVisible;
    private bool FindItemInParentCollection(AMLNodeViewModel source, AMLNodeViewModel target,
        List<AMLNodeViewModel> Visuals)
    {
        // start at the parent

        if (source == null || target == null)
        {
            return false;
        }

        var parent = source.Parent; // as IObjectWithItemsCollection;
        if (parent?.VisibleChildren == null)
        {
            return false;
        }

        var visuals = parent.VisibleChildren.ToList();
        for (var i = 0; i < visuals.Count; i++)
        {
            // siblings before are not of interest
            if (visuals[i] != source)
            {
                continue;
            }

            for (var j = i + 1; j < visuals.Count; j++)
            {
                if (!Visuals.Contains(visuals[j]))
                {
                    Visuals.Add(visuals[j]);
                }

                if (visuals[j] == target)
                {
                    return true;
                }

                if (!visuals[j].IsExpanded)
                {
                    continue;
                }

                var itemFound = FindItem(visuals[j].VisibleChildren.ToList(), target, Visuals);
                if (itemFound)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    //private void RemoveEdgeList(int index)
    //{
    //    Edges.Remove(Edges[index]);
    //    for (int i = 0; i < Edges.Count; i++)
    //    {
    //        for (int j = 0; j < Edges[i].Count; j++)
    //        {
    //            Edges[i][j].from.Index = i;
    //        }
    //    }
    //}

    #endregion Private Methods
}