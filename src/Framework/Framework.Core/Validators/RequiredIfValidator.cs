using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Framework.Web.Validators
{
    //public class RequiredIfValidator<T> : PropertyValidator<T, object>
    //{
    //    public string DependentProperty { get; }
    //    public object TargetValue { get; }

    //    public override string Name => "RequiredIfValidator";

    //    public RequiredIfValidator(string dependentProperty, object targetValue)
    //    {
    //        DependentProperty = dependentProperty;
    //        TargetValue = targetValue;
    //    }

    //    public override bool IsValid(ValidationContext<T> context, object value)
    //    {
    //        // Retrieve the value of the dependent property
    //        var dependentPropertyValue = context.InstanceToValidate.GetType()
    //            .GetProperty(DependentProperty)
    //            ?.GetValue(context.InstanceToValidate, null);

    //        // If the dependent property's value matches the target value, then the current property should be required
    //        if (dependentPropertyValue != null && dependentPropertyValue.Equals(TargetValue))
    //        {
    //            return value != null;
    //        }

    //        // If the dependent property's value does not match the target value, validation passes
    //        return true;
    //    }

    //    protected override string GetDefaultMessageTemplate(string errorCode)
    //    {
    //        return "{PropertyName} is required.";
    //    }
    //}

    //public class RequiredIfClientValidator<T> : ClientValidatorBase where T : class
    //{
    //    RequiredIfValidator<T> RequiredIfValidator => (RequiredIfValidator<T>)Validator;

    //    public RequiredIfClientValidator(PropertyRule<T, > rule, IPropertyValidator validator) : base(rule, validator)
    //    {
    //    }

    //    public override void AddValidation(ClientModelValidationContext context)
    //    {
    //        MergeAttribute(context.Attributes, "data-val", "true");
    //        MergeAttribute(context.Attributes, "data-val-requiredif", GetErrorMessage(context));
    //        MergeAttribute(context.Attributes, "data-val-requiredif-dependentproperty", RequiredIfValidator.DependentProperty);
    //        MergeAttribute(context.Attributes, "data-val-requiredif-targetvalue", RequiredIfValidator.TargetValue.ToString());
    //    }

    //    private string GetErrorMessage(ClientModelValidationContext context)
    //    {
    //        var formatter = ValidatorOptions.Global.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName());
    //        string messageTemplate;
    //        try
    //        {
    //            messageTemplate = Validator.Options.ErrorMessageSource.GetString(null);
    //        }
    //        catch (FluentValidationMessageFormatException)
    //        {
    //            messageTemplate = ValidatorOptions.Global.LanguageManager.GetStringForValidator<NotEmptyValidator>();
    //        }
    //        var message = formatter.BuildMessage(messageTemplate);
    //        return message;
    //    }
    //}

}
