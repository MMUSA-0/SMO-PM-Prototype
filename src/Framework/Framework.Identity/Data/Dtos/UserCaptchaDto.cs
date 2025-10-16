using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace Framework.Identity.Data.Dtos
{
    public class CapthcaDto
    {
        public string Captcha { get; set; }
        public string Text { get; set; }
        public string Key { get; set; }
    }
}