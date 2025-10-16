// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    #region usings

    using Framework.Core;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.Encodings.Web;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// The web extensions.
    /// </summary>
    public static class WebExtensions
    {
        // TODO: Usama re-write below code without webhelper class 
        // public static HttpContext GetCurrentHttpContext(this HttpContext httpContext)
        // {
        // return WebHelper.CurrentHttpContext;
        // }

        // public static IHostingEnvironment GetHostingEnvironment(this HttpContext httpContext)
        // {
        // return WebHelper.HostingEnvironment;
        // }

        // public static RuntimeDetails GetCurrentRuntimeDetails(this HttpContext httpContext)
        // {
        // return WebHelper.RuntimeDetails;
        // }

        // public static IServiceProvider GetCServiceProvider(this HttpContext httpContext)
        // {
        // return WebHelper.CurrentHttpContext.RequestServices;
        // }

        // regex from http://detectmobilebrowsers.com/
        /// <summary>
        /// The browse regex.
        /// </summary>
        private static readonly Regex BrowseRegex = new Regex(
            @"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// The browser version regex.
        /// </summary>
        private static readonly Regex BrowserVersionRegex = new Regex(
            @"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// The absolute path.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AbsolutePath(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}";
        }

        // public static void RegisterCssInclude(this HttpResponse response, string key, string styleHrefUrl)
        // {
        // if (string.IsNullOrEmpty(styleHrefUrl) || string.IsNullOrEmpty(key))
        // {
        // return;
        // }

        // response.RegisterInclude(key, $@"<link id=""{key}"" href=""{styleHrefUrl}"" rel=""stylesheet"" type=""text/css""/>");
        // }

        // public static void RegisterCssInclude(this HttpResponse response, string scriptSrcUrl)
        // {
        // if (string.IsNullOrEmpty(scriptSrcUrl))
        // {
        // return;
        // }

        // var key = scriptSrcUrl.GetHashCode().ToString();

        // response.RegisterCssInclude(key, scriptSrcUrl);
        // }

        // internal static void RegisterWidgetCssInclude(this HttpResponse response, string scriptSrcUrl)
        // {
        // var scriptWithVersionNumber = Regex.Replace(scriptSrcUrl, @"\/widgets\/", $"/EmbeddedResources/Widgets/v{CommonsSettings.WidgetsStaticContentVersion}/", RegexOptions.IgnoreCase);
        // response.RegisterCssInclude(scriptWithVersionNumber);
        // }

        // private static void RegisterInclude(this HttpResponse response, string key, string scriptOrCssTagHtml)
        // {
        // Dictionary<string, string> scripts;
        // if (WebHelper.CurrentHttpContext.Items["RegisterScriptInclude"] != null)
        // {
        // scripts = (Dictionary<string, string>)WebHelper.CurrentHttpContext.Items["RegisterScriptInclude"];
        // }
        // else
        // {
        // scripts = new Dictionary<string, string>();
        // WebHelper.CurrentHttpContext.Items["RegisterScriptInclude"] = scripts;
        // }

        // if (!scripts.ContainsKey(key))
        // {
        // scripts.Add(key, scriptOrCssTagHtml);
        // }
        // }

        // public static void RegisterScriptInclude(this HttpResponse response, string key, string scriptSrcUrl)
        // {
        // if (string.IsNullOrEmpty(scriptSrcUrl) || string.IsNullOrEmpty(key))
        // {
        // return;
        // }

        // response.RegisterInclude(key, $@"<script id=""{key}"" src=""{scriptSrcUrl}""></script>");
        // }

        // public static void RegisterScriptInclude(this HttpResponse response, string scriptSrcUrl)
        // {
        // if (string.IsNullOrEmpty(scriptSrcUrl))
        // {
        // return;
        // }

        // var key = scriptSrcUrl.GetHashCode().ToString();

        // response.RegisterScriptInclude(key, scriptSrcUrl);
        // }

        // internal static void RegisterWidgetScriptInclude(this HttpResponse response, string scriptSrcUrl)
        // {
        // var scriptWithVersionNumber = Regex.Replace(scriptSrcUrl, @"\/widgets\/", $"/EmbeddedResources/Widgets/v{CommonsSettings.WidgetsStaticContentVersion}/", RegexOptions.IgnoreCase);
        // response.RegisterScriptInclude(scriptWithVersionNumber);
        // }

        // public static string GetRegisteredScriptsAndStyles(this HttpResponse response)
        // {
        // var outScripts = new StringBuilder();

        // // Scripts References
        // if (WebHelper.CurrentHttpContext.Items["RegisterScriptInclude"] != null)
        // {
        // var scripts = (Dictionary<string, string>)WebHelper.CurrentHttpContext.Items["RegisterScriptInclude"];
        // foreach (var script in scripts.Values)
        // {
        // outScripts.AppendLine(script);
        // }
        // }

        // // Script Blocks
        // if (WebHelper.CurrentHttpContext.Items["RegisterStartupScriptBlock"] != null)
        // {
        // var scripts = (Dictionary<string, string>)WebHelper.CurrentHttpContext.Items["RegisterStartupScriptBlock"];
        // foreach (var script in scripts.Values)
        // {
        // outScripts.AppendLine(script);
        // }
        // }

        // return outScripts.ToString();
        // }

        // public static void RegisterStartupScriptBlock(
        // this HttpResponse response,
        // string key,
        // string scriptSrcUrl)
        // {
        // var scriptTagHtml = $"<script type=\"text/javascript\" language=\"javascript\">{scriptSrcUrl}</script>";
        // Dictionary<string, string> scripts;
        // if (WebHelper.CurrentHttpContext.Items["RegisterStartupScriptBlock"] != null)
        // {
        // scripts = (Dictionary<string, string>)WebHelper.CurrentHttpContext.Items["RegisterStartupScriptBlock"];
        // }
        // else
        // {
        // scripts = new Dictionary<string, string>();
        // WebHelper.CurrentHttpContext.Items["RegisterStartupScriptBlock"] = scripts;
        // }

        // if (!scripts.ContainsKey(key))
        // {
        // scripts.Add(key, scriptTagHtml);
        // }
        // }

        /// <summary>
        /// The get html as string.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetHtmlAsString(this TagBuilder content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        // public static System.Net.IPAddress GetUserHostIpAddress(this HttpRequest request)
        // {
        // return WebHelper.CurrentHttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
        // }

        /// <summary>
        /// The get local ip address.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IPAddress"/>.
        /// </returns>
        public static IPAddress GetLocalIpAddress(this HttpContext context)
        {
            return context.Features.Get<IHttpConnectionFeature>()?.LocalIpAddress;
        }

        public static IPAddress GetRemoteIpAddress(this HttpContext context)
        {
            return context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
        }

        public static string GetRemoteIpAddressWithPort(this HttpContext context)
        {
            var feature = context.Features.Get<IHttpConnectionFeature>();
            return $"{feature.RemoteIpAddress}:{feature.RemotePort}";
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">
        /// The HTTP request.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="request"/> parameter is null (Nothing in Visual
        ///     Basic).
        /// </exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }

        /// <summary>
        /// The is mobile browser.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMobileBrowser(this HttpRequest request)
        {
            var userAgent = request.UserAgent();
            if (BrowseRegex.IsMatch(userAgent) || BrowserVersionRegex.IsMatch(userAgent.Substring(0, 4)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The parse query string.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ParseQueryString<T>(this HttpRequest request, string key, T defaultValue = default(T))
        {
            return request.Query[key].To(defaultValue);
        }

        /// <summary>
        /// The to byte array.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ToByteArray(this IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                var fileStream = file.OpenReadStream();
                fileStream.Position = 0;
                fileStream.Flush();
                fileStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// The to memory stream.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        public static MemoryStream ToMemoryStream(this IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                var fileStream = file.OpenReadStream();
                fileStream.Position = 0;
                fileStream.Flush();
                fileStream.CopyTo(memoryStream);
                return memoryStream;
            }
        }

        /// <summary>
        /// The url.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Url(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }

        /// <summary>
        /// The user agent.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"];
        }

        /// <summary>
        /// The cookies.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<Item> Cookies(this HttpRequest request)
        {
            return request?.Cookies?.Keys.Select(k => new Item(k, request.Cookies[k])).ToList();
        }

        /// <summary>
        /// The form.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<Item> Form(this HttpRequest request)
        {
            try
            {
                return request?.Form?.Keys.Select(k => new Item(k, request.Form[k])).ToList();
            }
            catch (InvalidOperationException)
            {
                // Request not a form POST or similar
            }

            return null;
        }

        /// <summary>
        /// The query string.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<Item> QueryString(this HttpRequest request)
        {
            return request?.Query?.Keys.Select(k => new Item(k, request.Query[k])).ToList();
        }

        /// <summary>
        /// The server variables.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<Item> ServerVariables(this HttpRequest request)
        {
            return request?.Headers?.Keys.Select(k => new Item(k, request.Headers[k])).ToList();
        }
    }
}