// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 04-27-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-27-2015
// ***********************************************************************
// <copyright file="AMLUndoRedoManager.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

/// <summary>
/// The Tools namespace.
/// </summary>
namespace AMLToolkit.Tools
{
    /// <summary>
    /// Class AMLUndoRedoManager provides Basic Undo Redo Feature for AML Documents using Command Pattern
    /// </summary>
    public class AMLUndoRedoManager : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// The redo command-stack
        /// </summary>
        private Stack<IAMLCommand> _Redocommands = new Stack<IAMLCommand>();

        /// <summary>
        /// The undo command-stack
        /// </summary>
        private Stack<IAMLCommand> _Undocommands = new Stack<IAMLCommand>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance can redo 
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo
        {
            get
            {
                return (_Redocommands.Count != 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        public bool CanUndo
        {
            get
            {
                return (_Undocommands.Count != 0);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates and excutes a command and if the command's execute method returns true, the command is stored for undo
        /// </summary>
        /// <typeparam name="T">Command Type</typeparam>
        /// <returns><c>true</c> if executed and stored, <c>false</c> otherwise.</returns>
        public bool ExcuteAndInsertCommand<T>() where T : IAMLCommand, new()
        {
            T cmd = new T();
            if (cmd.Execute())
            {
                _Undocommands.Push(cmd); _Redocommands.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Excutes the sprecified command and if the command's execute method returns true, the command is stored for undo
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ExcuteAndInsertCommand(IAMLCommand command)
        {
            if (command.Execute())
            {
                _Undocommands.Push(command);
                _Redocommands.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Redoes for all specified levels.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public void Redo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_Redocommands.Count != 0)
                {
                    var action = _Redocommands.Pop();
                    if (action.Execute())
                        _Undocommands.Push(action);
                }
            }
            base.RaisePropertyChanged(() => CanUndo);
            base.RaisePropertyChanged(() => CanRedo);
        }

        /// <summary>
        /// Gets the DisplayName for each stored redo command.
        /// </summary>
        /// <value>The redo commands.</value>
        public IEnumerable<string> RedoCommands 
        {
            get { return _Redocommands.ToArray().Select(cmd => cmd.DisplayName); }
        }

        /// <summary>
        /// Gets the DisplayName for each stored undo command.
        /// </summary>
        /// <value>The undo commands.</value>
        public IEnumerable<string> UndoCommands
        {
            get { return _Undocommands.ToArray().Select(cmd => cmd.DisplayName); }
        }

        /// <summary>
        /// Undoes for all specified levels.
        /// </summary>
        /// <param name="levels">The number of undos.</param>
        public void Undo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_Undocommands.Count != 0)
                {
                    var action = _Undocommands.Pop();
                    if (action.UnExecute())
                        _Redocommands.Push(action);
                }
            }
            base.RaisePropertyChanged(() => CanUndo);
            base.RaisePropertyChanged(() => CanRedo);
        }

        #endregion Public Methods
    }
}