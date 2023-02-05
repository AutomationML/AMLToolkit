using System.Xml.Linq;

namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    /// Nodetype in an aml treeview, not assigned to a specific AML object
    /// </summary>
    /// <seealso cref="AMLNodeViewModel" />
    public class AMLExpandableDummyNode : AMLNodeViewModel
    {
        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AMLExpandableDummyNode"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="caexNode">The CAEX node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLExpandableDummyNode(AMLNodeViewModel parent, XElement caexNode, bool lazyLoadChildren)
            : base(parent, caexNode, lazyLoadChildren)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLExpandableDummyNode"/> class.
        /// </summary>
        /// <param name="tree">The TreeViewModel, containing the node</param>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The CAEX node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLExpandableDummyNode(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
            bool lazyLoadChildren)
            : base(tree, parent, CaexNode, lazyLoadChildren)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets and sets the MappedValue
        /// </summary>
        public override double MappedValue
        {
            get => 0;
            set => base.MappedValue = value;
        }

        /// <summary>
        /// Gets and sets the Name
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get => "Expand";
            set => base.Name = value;
        }

        #endregion Public Properties
    }
}