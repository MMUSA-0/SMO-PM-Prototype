using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Helpers
{
    public static class Base64FileHelper
    {
        public static IFormFile ConvertBase64ToFormFile(string base64Content, string fileName, string contentType = null)
        {
            // Convert to byte array
            var fileBytes = Convert.FromBase64String(base64Content);
            var stream = new MemoryStream(fileBytes);

            // Create IFormFile
            return new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType ?? GetContentTypeFromExtension(fileName)
            };
        }

        private static string GetContentTypeFromExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
