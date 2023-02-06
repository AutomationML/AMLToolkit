using System.Threading.Tasks;
using System.Windows.Input;

namespace Aml.Toolkit.ViewModel.Commands;

/// <summary>
///     Interface class, defining the methods of async commands
/// </summary>
/// <seealso cref="ICommand" />
public interface IAsyncCommand : ICommand
{
    #region Public Methods

    /// <summary>
    ///     Method, to execute the command async.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns></returns>
    Task ExecuteAsync(object parameter);

    #endregion Public Methods
}