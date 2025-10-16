using System;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Identity.Data.Dtos
{
    public class UserCreateUpdateDto : UserCreateOrUpdateDtoBase
    {
        [HiddenInput]
        public Guid Id { get; set; }
    }
}