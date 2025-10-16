// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomValidationAttributeAdapterProvider.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    /// <summary>
    /// The custom validation attribute adapter provider.
    /// </summary>
    public class CustomValidationAttributeAdapterProvider : ValidationAttributeAdapterProvider, IValidationAttributeAdapterProvider
    {
        /// <summary>
        /// The get attribute adapter.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        /// <returns>
        /// The <see cref="IAttributeAdapter"/>.
        /// </returns>
        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            var adapter = base.GetAttributeAdapter(attribute, stringLocalizer);

            if (adapter == null)
            {
                //if (attribute is ArabicTextOnlyAttribute arabicTextOnly)
                //{
                //    return new ArabicTextOnlyAttributeAdaptor(arabicTextOnly, stringLocalizer);
                //}

                //if (attribute is DisableScriptsAttribute disableScripts)
                //{
                //    return new DisableScriptsAttributeAdaptor(disableScripts, stringLocalizer);
                //}

                //if (attribute is EnglishTextOnlyAttribute englishTextOnly)
                //{
                //    return new EnglishTextOnlyAttributeAdaptor(englishTextOnly, stringLocalizer);
                //}

                //if (attribute is LocationLatLonAttribute locationLatLon)
                //{
                //    return new LocationLatLonAttributeAdaptor(locationLatLon, stringLocalizer);
                //}

                //if (attribute is NumbersOnlyAttribute numbersOnly)
                //{
                //    return new NumbersOnlyAttributeAdaptor(numbersOnly, stringLocalizer);
                //}

                //if (attribute is YearsOnlyAttribute yearsOnly)
                //{
                //    return new YearsOnlyAttributeAdaptor(yearsOnly, stringLocalizer);
                //}

                //if (attribute is MustBeTrueAttribute mustBeTrue)
                //{
                //    return new MustBeTrueAttributeAdapter(mustBeTrue, stringLocalizer);
                //}

                //if (attribute is RequiredNoSpaceAttribute requiredNoSpace)
                //{
                //    return new RequiredNoSpaceAttributeAdapter(requiredNoSpace, stringLocalizer);
                //}

                if (attribute is GreaterThanAttribute greaterThan)
                {
                    return new GreaterThanAttributeAdapter(greaterThan, stringLocalizer);
                }

                if (attribute is IsDateAfterAttribute isDateAfter)
                {
                    return new IsDateAfterAttributeAdapter(isDateAfter, stringLocalizer);
                }

                if (attribute is DateRestrictionAttribute dateRestriction)
                {
                    return new DateRestrictionAttributeAdapter(dateRestriction, stringLocalizer);
                }

                var validatephonenumber = attribute as ValidatePhoneNumberAttribute;

                //if (validatephonenumber != null)
                //{
                //    return new ValidatePhoneNumberAttributeAdapter(validatephonenumber, stringLocalizer);
                //}

                if (attribute is ValidateFileUploadAttribute validateFileUpload)
                {
                    // return new ValidateFileUploadAttributeAdapter(validateFileUpload, stringLocalizer);
                }
            }

            return adapter;
        }
    }
}