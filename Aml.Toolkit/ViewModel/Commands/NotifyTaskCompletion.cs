using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Aml.Toolkit.ViewModel.Commands;

/// <summary>
///     This class was originally published at   https://msdn.microsoft.com/magazine/dn605875
/// </summary>
/// <typeparam name="TResult"></typeparam>
public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
    #region Public Events

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotifyTaskCompletion{TResult}" /> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public NotifyTaskCompletion(Task<TResult> task)
    {
        Task = task;
        //if (!task.IsCompleted)
        //{
        //    var _ = WatchTaskAsync(task);
        //}
    }

    /// <summary>
    ///     Async activation of the task
    /// </summary>
    public void WatchTask()
    {
        if (!Task.IsCompleted)
        {
            var _ = WatchTaskAsync(Task);
        }
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets the error message.
    /// </summary>
    /// <value>
    ///     The error message.
    /// </value>
    public string ErrorMessage => InnerException?.Message;

    /// <summary>
    ///     Gets the task exception.
    /// </summary>
    /// <value>
    ///     The exception.
    /// </value>
    public AggregateException Exception => Task.Exception;

    /// <summary>
    ///     Gets the inner task exception, if exists.
    /// </summary>
    /// <value>
    ///     The inner exception.
    /// </value>
    public Exception InnerException => Exception?.InnerException;

    /// <summary>
    ///     Gets a value indicating whether the task is canceled.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is canceled; otherwise, <c>false</c>.
    /// </value>
    public bool IsCanceled => Task.IsCanceled;

    /// <summary>
    ///     Gets a value indicating whether the task is completed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is completed; otherwise, <c>false</c>.
    /// </value>
    public bool IsCompleted => Task.IsCompleted;

    /// <summary>
    ///     Gets a value indicating whether the task is faulted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is faulted; otherwise, <c>false</c>.
    /// </value>
    public bool IsFaulted => Task.IsFaulted;

    /// <summary>
    ///     Gets a value indicating whether the task is not completed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is not completed; otherwise, <c>false</c>.
    /// </value>
    public bool IsNotCompleted => !Task.IsCompleted;


    /// <summary>
    ///     Gets a value indicating whether the task is running.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is running; otherwise, <c>false</c>.
    /// </value>
    public bool IsRunning => Task.Status == TaskStatus.Running;

    /// <summary>
    ///     Gets a value indicating whether the task is successfully completed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the task is successfully completed; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccessfullyCompleted => Task.Status ==
                                           TaskStatus.RanToCompletion;

    /// <summary>
    ///     Gets the task result.
    /// </summary>
    /// <value>
    ///     The result.
    /// </value>
    public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default;

    /// <summary>
    ///     Gets the task status.
    /// </summary>
    /// <value>
    ///     The status.
    /// </value>
    public TaskStatus Status => Task.Status;

    /// <summary>
    ///     Gets the task.
    /// </summary>
    /// <value>
    ///     The task.
    /// </value>
    public Task<TResult> Task { get; }

    /// <summary>
    ///     Gets the task completion.
    /// </summary>
    /// <value>
    ///     The task completion.
    /// </value>
    public Task TaskCompletion => null;

    #endregion Public Properties

    #region Private Methods

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
        }

        Refresh();
    }

    /// <summary>
    ///     Gets or sets the attached information about the task.
    /// </summary>
    /// <value>
    ///     The information.
    /// </value>
    public string Info { get; set; }

    /// <summary>
    ///     Refreshes the task states by raising the property changed events
    /// </summary>
    public void Refresh()
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged == null)
        {
            return;
        }

        propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsRunning)));

        if (Task.IsCanceled)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        }
        else if (Task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else if (Task.IsCompleted)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }

    #endregion Private Methods
}