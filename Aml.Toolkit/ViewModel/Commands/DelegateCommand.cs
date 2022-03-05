using System;
using System.Windows.Input;

namespace Aml.Toolkit.ViewModel.Commands
{
    /// <summary>
    /// Delegate command class
    /// </summary>
    /// <seealso cref="ICommand"/>
    public sealed class DelegateCommand : ICommand
    {
        #region Private Fields

        private readonly Action _command;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public DelegateCommand(Action command)
        {
            _command = command;
        }

        #endregion Public Constructors

        #region Public Events

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        #endregion Public Events

        #region Public Methods

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method that will be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public void Execute(object parameter)
        {
            _command();
        }

        #endregion Public Methods
    }
}