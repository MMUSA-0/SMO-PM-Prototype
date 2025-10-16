using Framework.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.SharedServices.Dto
{
    [Serializable]
    public class SettingsDto
    {
        [HiddenInput]
        public int Id { get; set; }

        [Display(Name = "SettingName", ResourceType = typeof(SharedResources))]
        public string Name { get; set; }

        public string ValueType { get; set; }

        [Display(Name = "SettingValue", ResourceType = typeof(SharedResources))]
        [Required]
        public string Value { get; set; }

        public string GroupName { get; set; }


    }
}
