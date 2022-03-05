using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aml.Toolkit.ViewModel.Commands
{
    /// <summary>
    /// Implents an async command
    /// </summary>
    public static class AsyncCommand
    {
        #region Public Methods

        /// <summary>
        /// Creates an async command instance with the specified action.
        /// </summary>
        /// <param name="action">The Action.</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<Task> action)
        {
            return new AsyncCommand<object>(async _ =>
            {
                await action();
                return null;
            });
        }

        /// <summary>
        /// Creates an async command instance with the specified action.
        /// </summary>
        /// <param name="action">The Action.</param>
        /// <returns></returns>
        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> action)
        {
            return new AsyncCommand<TResult>(_ => action());
        }

        /// <summary>
        /// Creates an async command instance with the specified action.
        /// </summary>
        /// <param name="action">The Action.</param>
        /// <returns></returns>
        public static AsyncCommand<object> Create(Func<CancellationToken, Task> action)
        {
            return new AsyncCommand<object>(async token =>
            {
                await action(token);
                return null;
            });
        }

        /// <summary>
        /// Creates an async command instance with the specified action.
        /// </summary>
        /// <param name="action">The Action.</param>
        /// <returns></returns>
        public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> action)
        {
            return new AsyncCommand<TResult>(action);
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Class defining an async command
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{TResult}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
        {
            _command = command;
            _cancelCommand = new CancelAsyncCommand();
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Protected Methods

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods

        #region Private Classes

        private sealed class CancelAsyncCommand : ICommand
        {
            #region Public Properties

            public CancellationToken Token => _cts.Token;

            #endregion Public Properties

            #region Public Events

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            #endregion Public Events

            #region Private Methods

            private static void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }

            #endregion Private Methods

            #region Private Fields

            private bool _commandExecuting;
            private CancellationTokenSource _cts = new();

            #endregion Private Fields

            #region Public Methods

            bool ICommand.CanExecute(object parameter)
            {
                return _commandExecuting && !_cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                _cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                _commandExecuting = false;
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandStarting()
            {
                _commandExecuting = true;
                if (!_cts.IsCancellationRequested)
                {
                    return;
                }

                _cts = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            #endregion Public Methods
        }

        #endregion Private Classes

        #region Private Fields

        private readonly CancelAsyncCommand _cancelCommand;
        private readonly Func<CancellationToken, Task<TResult>> _command;
        private NotifyTaskCompletion<TResult> _execution;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>
        /// The cancel command.
        /// </value>
        public ICommand CancelCommand => _cancelCommand;

        /// <summary>
        /// Gets the execution task.
        /// </summary>
        /// <value>
        /// The execution.
        /// </value>
        public NotifyTaskCompletion<TResult> Execution
        {
            get => _execution;
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether this instance can execute the action.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return Execution == null || Execution.IsCompleted;
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public override async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            _cancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        #endregion Public Methods
    }
}