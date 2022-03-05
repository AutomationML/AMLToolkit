using System.Windows;
using System.Windows.Media;

namespace Aml.Toolkit.ViewModel.Graph
{
    /// <summary>
    /// Edge in a graph used to draw Internal link lines
    /// </summary>
    public class Edge
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the drawing pen.
        /// </summary>
        /// <value>
        /// The pen.
        /// </value>
        public Pen Pen { get; set; }

        /// <summary>
        /// Gets the internal link line graph.
        /// </summary>
        /// <value>
        /// The graph.
        /// </value>
        public IlGraph Graph { get; }

        #endregion Public Properties

        #region Internal Fields

        internal readonly Point[] Points = new Point[4];
        internal Vertex StartPoint;
        internal bool IsVisited = false;
        internal double Length;
        internal Range<double> Range;
        internal Vertex EndPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="graph">The graph.</param>
        public Edge(IlGraph graph)
        {
            Graph = graph;
        }

        internal void Delete()
        {
            Graph.RemoveEdge(StartPoint, EndPoint);

        }

        #endregion Internal Fields
    }
}