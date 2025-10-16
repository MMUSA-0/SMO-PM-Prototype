using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";

        public string OtherProperty { get; private set; }
        public string OtherPropertyName { get; private set; }

        public UnlikeAttribute(
            string otherProperty,
            string otherPropertyName)
            : base(DefaultErrorMessage)
        {
            OtherProperty = otherProperty;
            OtherPropertyName = otherPropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

                if (value.Equals(otherPropertyValue))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;

        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherPropertyName);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-cannotbered", errorMessage);
        }
        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
