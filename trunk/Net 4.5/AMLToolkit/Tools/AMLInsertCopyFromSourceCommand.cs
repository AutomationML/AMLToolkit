// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 04-27-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-27-2015
// ***********************************************************************
// <copyright file="AMLInsertCopyFromSourceCommand.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Xml;
using AMLToolkit.ViewModel;
using CAEX_ClassModel;

/// <summary>
/// The Tools namespace.
/// </summary>
namespace AMLToolkit.Tools
{
    /// <summary>
    /// Class AMLInsertCopyFromSourceCommand is a <see cref="IAMLCommand"/>. The Command can handle insertion and removal from
    /// CAEX-Nodes in any Hierarchy. The Effect is propagted to the ViewModel of the Hierarchy.
    /// </summary>
    public class AMLInsertCopyFromSourceCommand : IAMLCommand
    {
        #region Public Methods

        /// <summary>
        /// Creates and Excutes an <see cref="AMLInsertCopyFromSourceCommand"/> and if the execute Method returns true, the
        /// Command is stored in the defined AMLUndoRedo-Manager for an undo-Action.
        /// </summary>
        /// <param name="manager">The AMLUndoRedo-Manager used to manage the created command.</param>
        /// <param name="target">The target for the Insertion. </param>
        /// <param name="source">The source which is used to create a Copy which than is inserted as a new child in the target.</param>
        /// <returns><c>true</c> if Command is executed and stored for undo, <c>false</c> otherwise.</returns>
        public static bool ExcuteAndInsertCommand(AMLUndoRedoManager manager, AMLNodeViewModel target, AMLNodeViewModel source)
        {
            IAMLCommand cmd = new AMLInsertCopyFromSourceCommand(target, source);
            return manager.ExcuteAndInsertCommand(cmd);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns><c>true</c> if execute has an effect, which is reversable by <see cref="UnExecute" />, <c>false</c> otherwise.</returns>
        public bool Execute()
        {
            if (_nextNode == null)
            {
                var parent = CAEXBasicObject.CreateCAEXWrapper(_Target.CAEXNode);
                var child = CAEXBasicObject.CreateCAEXWrapper(_node);
                if (!parent.Insert_TypeBaseElement(child, false))
                {
                    return false;
                }
            }
            else
            {
                _Target.CAEXNode.InsertBefore(_node, _nextNode);
            }

            _Target.RefreshNodeInformation(true);
            return true;
        }

        /// <summary>
        /// Reverse Method for the effect, created by the <see cref="Execute"/>.
        /// </summary>
        /// <returns><c>true</c> if UnExecute  has an effect, which is reversable by <see cref="Execute" />, <c>false</c> otherwise.</returns>
        public bool UnExecute()
        {
            if (_Target != null && _node != null)
            {
                // if it is not the last node
                if (_node.NextSibling != null && _node.Name == _node.NextSibling.Name)
                {
                    _nextNode = _node.NextSibling;
                }
                _Target.CAEXNode.RemoveChild(_node);
                _Target.RefreshNodeInformation(false);

                return true;
            }
            return false;
        }

        #endregion Public Methods

        #region Private Fields

        private XmlNode _nextNode = null;

        private XmlNode _node;

        private AMLNodeViewModel _Target;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLInsertCopyFromSourceCommand"/> class. 
        /// A deep copy of the defined source will be created. The Copy is used in the <see cref="Execute"/> Method
        /// where the Copy is added to the Childs of the Target.
        /// </summary>
        /// <param name="target">The target for the Insertion Command.</param>
        /// <param name="source">The source for the Copy Operation.</param>
        public AMLInsertCopyFromSourceCommand(AMLNodeViewModel target, AMLNodeViewModel source)
        {
            _Target = target;
            _node = source.CAEXNode.CloneNode(true);

            DisplayName = "Insert Node";
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get;
            set;
        }

        #endregion Public Properties
    }
}