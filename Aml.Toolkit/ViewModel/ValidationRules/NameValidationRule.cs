using System;
using System.Globalization;
using System.Windows.Controls;
using Aml.Engine.CAEX;
using Aml.Engine.Services;
using Aml.Engine.Services.Interfaces;

namespace Aml.Toolkit.ViewModel.ValidationRules;

/// <summary>
///     Validation rule for name validation
/// </summary>
/// <seealso cref="Aml.Toolkit.ViewModel.ValidationRules.CAEXValidationRule" />
public class NameValidationRule : CAEXValidationRule
{
    #region Public Methods

    /// <inheritdoc />
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var unregister = false;

        if (AssignedObject.CaexObject == null)
        {
            return new ValidationResult(true, null);
        }

        var validator = ServiceLocator.GetService<IValidator>();
        if (validator == null)
        {
            validator = ValidatorService.Register();
            unregister = true;
        }

        var strValue = Convert.ToString(value);

        var (IsValid, Message) = validator.NameValidation(AssignedObject.CaexObject as CAEXObject, strValue);

        if (unregister)
        {
            ValidatorService.UnRegister();
        }

        return new ValidationResult(IsValid, Message);
    }

    #endregion Public Methods
}