using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aml.Toolkit.ViewModel.Commands
{
    /// <summary>
    /// abstract base class for async commands
    /// </summary>
    /// <seealso cref="IAsyncCommand"/>
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        #region Public Events

        /// <summary>
        /// Occurs when changes occur that affect the execution of the command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion Public Events

        #region Public Methods

        /// <summary> Defines the method that determines whether the command can be executed in the
        /// current state. 
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Defines the method that will be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// Defines the method that will be called when the command is invoked  async.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(object parameter);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raises the can execute changed event.
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion Protected Methods
    }
}