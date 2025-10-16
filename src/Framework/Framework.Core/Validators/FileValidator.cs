using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Framework.Core.Validators
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            //RuleFor(a => a.Length).NotNull().WithMessage(SharedResources.Required);

            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(0)
                .WithMessage("File size is larger than allowed");

            //RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
            //    .WithMessage("File type is larger than allowed");
        }
    }
}
