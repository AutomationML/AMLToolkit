using System;
using System.Windows.Threading;

namespace Aml.Toolkit.ViewModel.Commands;

/// <summary>
///     Cass used to execute action using a dispatcher
/// </summary>
public static class Execute
{
    private static Func<Action, DispatcherOperation> _asyncExecutor;
    private static Action<Action> _syncExecutor;

    /// <summary>
    ///     Initializes the framework using the current dispatcher.
    /// </summary>
    public static void InitializeWithDispatcher()
    {
#if SILVERLIGHT
        var dispatcher = Deployment.Current.Dispatcher;
#else
        var dispatcher = Dispatcher.CurrentDispatcher;
#endif
        _asyncExecutor = action =>
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        };

        _syncExecutor = action =>
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(DispatcherPriority.Background, action);
            }
        };
    }

    /// <summary>
    ///     Executes the action on the UI thread asynchronous
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static DispatcherOperation OnUIThread(this Action action) => _asyncExecutor?.Invoke(action);

    /// <summary>
    ///     Executes the action on the UI thread synchronous.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void OnUIThreadSync(this Action action)
    {
        _syncExecutor?.Invoke(action);
    }
}