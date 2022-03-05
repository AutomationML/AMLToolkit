using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using Aml.Engine.Services;
using Aml.Engine.Xml.Extensions;
using Aml.Toolkit.View;
using Aml.Toolkit.XamlClasses;
using Aml.Editor.MVVMBase;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Aml.Editor.Plugin.Contracts;

namespace Aml.Toolkit.ViewModel.Graph
{
    /// <summary>
    /// View model class for the internal link mover control.
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class ILMoverViewModel : ViewModelBase
    {
        #region Internal Constructors

        internal ILMoverViewModel(UIElement adornedElement,
            InternalLinkType selectedLink,
            Edge selectedEdge)
        {
            _adornedElement = adornedElement;
            _selectedLink = selectedLink;
            SelectedEdge = selectedEdge;

            LineBrush = selectedEdge.Pen.Brush;
            LineThickness = selectedEdge.Pen.Thickness * 2;
            UpdatePoints(selectedEdge.Points);

            _originalPoints = new PointCollection(selectedEdge.Points);
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal Edge SelectedEdge { get; private set; }

        #endregion Internal Properties


        /// <summary>
        /// Called, when a move operation starts.
        /// </summary>
        public void Start()
        {
            var point = Mouse.GetPosition(_adornedElement);

            //TreeViewItem item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(adornedElement);
            var node = _adornedElement.InputHitTest(point) as DependencyObject;
            var movedNode = VisualTreeUtilities.FindVisualParent<TreeViewItem>(node);

            if (movedNode?.DataContext is AMLNodeWithClassReference tv)
            {
                _movedInterface = tv;
            }
        }

        #region Private Fields

        private TreeViewItem _current;
        private PointCollection _currentPoints;

        /// <summary>
        ///     <see cref="DeleteInternalLinkCommand" />
        /// </summary>
        private RelayCommand<object> _deleteInternalLinkCommand;

        /// <summary>
        ///     <see cref="IsSnapped" />
        /// </summary>
        private bool _isSnapped;

        /// <summary>
        ///     <see cref="LineBrush" />
        /// </summary>
        private Brush _lineBrush;

        /// <summary>
        ///     <see cref="LineThickness" />
        /// </summary>
        private double _lineThickness;

        private readonly PointCollection _originalPoints;
        private AMLNodeWithClassReference _selectedInterface;
        private readonly UIElement _adornedElement;
        private InternalLinkType _selectedLink;
        private AMLNodeViewModel _movedInterface;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        ///     The DeleteInternalLinkCommand - Command
        /// </summary>
        public ICommand DeleteInternalLinkCommand => _deleteInternalLinkCommand ??= new RelayCommand<object>(
            DeleteInternalLinkCommandExecute,
            DeleteInternalLinkCommandCanExecute);

        /// <summary>
        ///     Gets and sets the IsSnapped
        /// </summary>
        public bool IsSnapped
        {
            get => _isSnapped;
            set => Set(ref _isSnapped, value, nameof(IsSnapped));
        }

        /// <summary>
        ///     Gets and sets the LineBrush
        /// </summary>
        public Brush LineBrush
        {
            get => _lineBrush;
            set => Set(ref _lineBrush, value, nameof(LineBrush));
        }

        /// <summary>
        ///     Gets and sets the LineThickness
        /// </summary>
        public double LineThickness
        {
            get => _lineThickness;
            set
            {
                if (Math.Abs(_lineThickness - value) < double.Epsilon)
                {
                    return;
                }

                _lineThickness = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the start- and end points of the moved line.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public PointCollection Points
        {
            get => _currentPoints;
            set
            {
                _currentPoints = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Internal Methods

        internal void DeleteSelectedLink()
        {
            // delete the internal link
            _selectedLink.Remove();
            SelectedEdge.Delete();
            _selectedLink = null;
            SelectedEdge = null;

            ((AMLTreeView)_adornedElement).InternalLinksAdorner?.InvalidateSelection();
        }

        internal void DragOver(bool moveTarget)
        {
            if (_current != null)
            {
                _current.Tag = null;
            }

            var point = Mouse.GetPosition(_adornedElement);

            //TreeViewItem item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(adornedElement);
            var node = _adornedElement.InputHitTest(point) as DependencyObject;
            var dropNode = VisualTreeUtilities.FindVisualParent<TreeViewItem>(node);

            if (dropNode?.DataContext is AMLNodeWithClassReference tv &&
                tv.CAEXObject is ExternalInterfaceType extInterface)
            {
                // korrektur wenn bewegung nicht erkannt wurde
                if (_movedInterface == null)
                {
                    if (extInterface == _selectedLink.AInterface)
                    {
                        _movedInterface = tv;
                    }
                    else if (extInterface.Equals(_selectedLink.BInterface))
                    {
                        _movedInterface = tv;
                    }
                    else if (moveTarget)
                    {
                        _movedInterface = SelectedEdge.EndPoint.Item;
                    }
                    else
                    {
                        _movedInterface = SelectedEdge.StartPoint.Item;
                    }
                }

                if (IsConnectible(extInterface))
                {
                    dropNode.Tag = "drop";
                    _selectedInterface = tv;
                    _current = dropNode;
                    IsSnapped = true;
                    return;
                }
            }

            if (_selectedInterface == null)
            {
                return;
            }

            if (_current != null)
            {
                _current.Tag = null;
            }

            IsSnapped = false;
        }

        internal bool RedirectInternalLink()
        {
            if (_current != null)
            {
                _current.Tag = null;
            }

            if (_selectedInterface?.CAEXNode == null)
            {
                return false;
            }

            if (_selectedLink == null)
            {
                return false;
            }

            if (_selectedInterface.CAEXObject.Equals(_selectedLink.AInterface))
            {
                return false;
            }

            if (_selectedInterface.CAEXObject.Equals(_selectedLink.BInterface))
            {
                return false;
            }

            var started =
                ServiceLocator.UndoRedoService?.BeginTransaction(_selectedLink.CAEXDocument(),
                    "Redirect Internal Link") ?? false;
            if (_selectedLink.AInterface.Equals(MovedInterface))
            {
                _selectedLink.AInterface = SelectedInterface;
            }
            else if (_selectedLink.BInterface.Equals(MovedInterface))
            {
                _selectedLink.BInterface = SelectedInterface;
            }

            var linkAnchor = _selectedLink.ASystemUnitClass.LowestCommonParent(_selectedLink.BSystemUnitClass);
            if (linkAnchor == null)
            {
                linkAnchor = _selectedLink.ASystemUnitClass;
            }

            if (linkAnchor != _selectedLink.CAEXParent)
            {
                _selectedLink.Remove();
                _ = linkAnchor.Insert(_selectedLink, false);
            }

            if (started)
            {
                _ = (ServiceLocator.UndoRedoService?.EndTransaction(_selectedLink.CAEXDocument()));
            }

            SelectedEdge.StartPoint.Item.RefreshNodeInformation(false);
            SelectedEdge.EndPoint.Item.RefreshNodeInformation(false);
            _selectedInterface.RefreshNodeInformation(false);
            _movedInterface.RefreshNodeInformation(false);
            //if (!aref.ShowLinks && !bref.ShowLinks)
            //{
            //    _selectedInterface.ShowLinks = true;
            //}

            return true;
        }

        internal void ResetPoints()
        {
            Points = new PointCollection(_originalPoints);
        }

        internal void UpdatePoint(int index, Point point)
        {
            var points = Points.ToArray();
            points[index] = point;
            points[1].Y = points[0].Y;
            points[2].Y = points[3].Y;

            Points = new PointCollection(points);
        }

        internal void UpdatePoints(Point[] points)
        {
            Points = new PointCollection(points);
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        ///     Test, if the <see cref="DeleteInternalLinkCommand" /> can execute.
        /// </summary>
        /// <param name="parameter">
        ///     TODO The parameter.
        /// </param>
        /// <returns>
        ///     true, if command can execute
        /// </returns>
        private bool DeleteInternalLinkCommandCanExecute(object parameter)
        {
            return _selectedLink != null;
        }

        /// <summary>
        ///     The <see cref="DeleteInternalLinkCommand" /> Execution Action.
        /// </summary>
        /// <param name="parameter">
        ///     TODO The parameter.
        /// </param>
        private void DeleteInternalLinkCommandExecute(object parameter)
        {
            DeleteSelectedLink();
        }


        internal static bool ContainingClassesAreEqual(ICAEXWrapper source, ICAEXWrapper target)
        {
            // check if the parents are identical classes
            var sourceClass = source.Node.Ancestors().FirstOrDefault(a => a.IsCAEXClass());
            if (sourceClass == null)
            {
                return true;
            }

            var targetClass = target.Node.AncestorsAndSelf().FirstOrDefault(a => a.IsCAEXClass());
            return targetClass == sourceClass ||
                   targetClass != null && targetClass.IsInstanceHierarchy() && sourceClass.IsInstanceHierarchy();
        }

        internal static bool CanLinkInterfaces(CAEXWrapper source, CAEXWrapper target)
        {
            if (source is not ExternalInterfaceType aInterface || target is not ExternalInterfaceType bInterface)
            {
                return false;
            }

            if (aInterface.IsMirror || bInterface.IsMirror)
            {
                return false;
            }

            if (aInterface.AssociatedObject is not SystemUnitClassType aClass)
            {
                return false;
            }

            if (bInterface.AssociatedObject is not SystemUnitClassType bClass)
            {
                return false;
            }

            if (!ContainingClassesAreEqual(source, target))
            {
                return false;
            }

            SystemUnitClassType parent;

            if ((parent = aClass.LowestCommonParent(bClass)) == null)
            {
                parent = aClass;
            }

            return !parent.InternalLink.Any(il => il.AInterface == aInterface && il.BInterface == bInterface ||
                                                  il.RelatedObjects.AInterface == bInterface &&
                                                  il.RelatedObjects.BInterface == aInterface);
        }

        private bool IsConnectible(ExternalInterfaceType selectedInterface)
        {
            if (selectedInterface == null)
            {
                return false;
            }

            if (selectedInterface.Equals(_selectedLink.AInterface))
            {
                return false;
            }

            return !selectedInterface.Equals(_selectedLink.BInterface) &&
                   CanLinkInterfaces(selectedInterface, UnMovedInterface);
        }

        private ExternalInterfaceType MovedInterface => _movedInterface?.CAEXObject as ExternalInterfaceType;

        private ExternalInterfaceType UnMovedInterface => _selectedLink.AInterface.Equals(MovedInterface)
            ? _selectedLink.BInterface
            : _selectedLink.AInterface;

        private ExternalInterfaceType SelectedInterface => _selectedInterface?.CAEXObject as ExternalInterfaceType;

        #endregion Private Methods
    }
}