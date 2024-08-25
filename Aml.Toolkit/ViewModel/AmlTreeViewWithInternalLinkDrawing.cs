// Copyright (c) 2017 AutomationML e.V.
using Aml.Toolkit.View;
using Aml.Toolkit.ViewModel.Graph;
using System.Linq;

namespace Aml.Toolkit.ViewModel;

internal class AmlTreeViewWithInternalLinkDrawing
{
    #region Internal Constructors

    internal AmlTreeViewWithInternalLinkDrawing(AMLTreeView treeView)
    {
        Graph = new IlGraph();
        Tree = treeView;
    }

    #endregion Internal Constructors

    #region Internal Properties

    internal IlGraph Graph { get; set; }
    internal AMLTreeView Tree { get; set; }

    #endregion Internal Properties

    #region Internal Methods

    internal void Clear()
    {
        Graph.Clear();
    }

    internal void Connect(AMLNodeViewModel source, AMLNodeViewModel target)
    {
        if (!Graph.Vertices.TryGetValue(source, out var sourceVertex))
        {
            sourceVertex = Graph.AddVertex(source);
        }

        if (!Graph.Vertices.TryGetValue(target, out var targetVertex))
        {
            targetVertex = Graph.AddVertex(target);
        }

        if (sourceVertex != null && targetVertex != null)
        {
            _ = Graph.AddEdge(sourceVertex, targetVertex);
        }
    }

    internal void DisConnect(AMLNodeViewModel source, AMLNodeViewModel target)
    {
        _ = Graph.Vertices.TryGetValue(source, out var sourceVertex);
        _ = Graph.Vertices.TryGetValue(target, out var targetVertex);
        if (sourceVertex != null && targetVertex != null)
        {
            Graph.RemoveEdge(sourceVertex, targetVertex);
        }
    }

    internal void DisConnectAll(AMLNodeViewModel source)
    {
        if (!Graph.Vertices.TryGetValue(source, out var sourceVertex))
        {
            return;
        }

        foreach (var targetVertex in Graph.Edges[sourceVertex.Index].Where(e => e != null).ToList()
                     .Select(edge => edge.StartPoint == sourceVertex ? edge.EndPoint : edge.StartPoint))
        {
            //if (targetVertex.Item is AMLNodeWithClassReference acr && acr.ShowLinks)
            //    continue;

            Graph.RemoveEdge(sourceVertex, targetVertex);

            if (targetVertex.Item is AMLNodeWithClassReference { ShowLinks: true, HasLinks: false } acr)
            {
                acr.ShowLinks = false;
            }
        }
    }

    #endregion Internal Methods
}