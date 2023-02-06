using System.Windows.Controls;

namespace Aml.Toolkit.ViewModel.ValidationRules;

/// <summary>
///     abstract base class for the visualization of caex validation results
/// </summary>
/// <seealso cref="ValidationRule" />
public abstract class CAEXValidationRule : ValidationRule
{
    #region Public Properties

    /// <summary>
    ///     Gets or sets the assigned object.
    /// </summary>
    /// <value>
    ///     The assigned object.
    /// </value>
    public AssignedData AssignedObject { get; set; }

    #endregion Public Properties
}