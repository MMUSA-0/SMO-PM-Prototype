using Framework.Core.DataAnnotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.Core.Extensions
{
    public static class HtmlEnumExtensions
    {
        private static string EnumToString(Type enumType)
        {
            var values = Enum.GetValues(enumType).Cast<int>();
            var enumDictionary = values.ToDictionary(value => Enum.GetName(enumType, value));

            return JsonConvert.SerializeObject(enumDictionary);
        }

        public static HtmlString JavaScriptEnums(this IHtmlHelper helper)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(s => s.FullName.StartsWith("ZATCA.")).ToList();
            var query = from a in assemblies
                        from t in a.GetTypes()//.Where(s => !s.FullName.Contains("mscorlib"))
                        from r in t.GetCustomAttributes<JavaScriptEnumAttribute>()
                        where t.BaseType == typeof(Enum)
                        select t;

            var buffer = new StringBuilder(10000);

            foreach (var jsEnum in query)
            {
                buffer.Append("var ");
                buffer.Append(jsEnum.Name);
                buffer.Append(" = ");
                buffer.Append(EnumToString(jsEnum));
                buffer.Append("; \r\n");
            }

            return new HtmlString(buffer.ToString());
        }
    }
}
