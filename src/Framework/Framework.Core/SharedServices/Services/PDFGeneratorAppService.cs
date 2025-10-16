using Microsoft.AspNetCore.Hosting;
using SelectPdf;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Services
{
    public class PDFGeneratorAppService : IPDFGenerator
    {
        private readonly IWebHostEnvironment env;

        public PDFGeneratorAppService(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public async Task<(bool status, string fullPath)> URLToPDFAsync(Uri uri, string saveToFullPath = null , bool useAbsoluteUnlessOrgenal = false)
        {
            return await Task.Run(() =>
             {
                 try
                 {
                     HtmlToPdf converter = new HtmlToPdf();
                     // create a new pdf document converting an url
                     PdfDocument doc = converter.ConvertUrl(useAbsoluteUnlessOrgenal? uri.AbsoluteUri : uri.OriginalString);
                     // save pdf document
                     if (string.IsNullOrEmpty(saveToFullPath))
                     {
                         var webRoot = env.ContentRootPath;
                         Guid fileId = Guid.NewGuid();
                         string newFilePath = $@"{webRoot}\PDFGeneratedFiles\";
                         newFilePath += fileId + ".pdf";
                         saveToFullPath = newFilePath;
                         string pdfTemplate = Path.Combine(webRoot, saveToFullPath);
                     }
                     if (File.Exists(saveToFullPath))
                         File.Delete(saveToFullPath);
                     doc.Save(saveToFullPath);
                     // close pdf document
                     doc.Close();
                     return (true, saveToFullPath);
                 }
                 catch (Exception _exp) { return (false, _exp.ToString()); }
             });
        }

        public (bool status, string fullPath) URLToPDF(Uri uri, string saveToFullPath = null , bool useAbsoluteUnlessOrgenal = false)
        {

                try
                {
                    HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertUrl(useAbsoluteUnlessOrgenal?uri.AbsoluteUri:uri.OriginalString);
                    // save pdf document
                    if (string.IsNullOrEmpty(saveToFullPath))
                    {
                        var webRoot = env.ContentRootPath;
                        Guid fileId = Guid.NewGuid();
                        string newFilePath = $@"{webRoot}\PDFGeneratedFiles\";
                        newFilePath += fileId + ".pdf";
                        saveToFullPath = newFilePath;
                        string pdfTemplate = Path.Combine(webRoot, saveToFullPath);
                    }
                    if (File.Exists(saveToFullPath))
                        File.Delete(saveToFullPath);
                    doc.Save(saveToFullPath);
                    // close pdf document
                    doc.Close();
                    return (true, saveToFullPath);
                }
                catch (Exception _exp) { return (false, _exp.ToString()); }
        }

        public (bool status, string fullPath) FullPathToPDF(string path, string saveToFullPath = null)
        {

            try
            {
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                string htmlFileString = File.ReadAllText(path,Encoding.Default).ToString();
                PdfDocument doc = converter.ConvertHtmlString(htmlFileString);
                // save pdf document
                if (string.IsNullOrEmpty(saveToFullPath))
                {
                    var webRoot = env.ContentRootPath;
                    Guid fileId = Guid.NewGuid();
                    string newFilePath = $@"{webRoot}\PDFGeneratedFiles\";
                    newFilePath += fileId + ".pdf";
                    saveToFullPath = newFilePath;
                    string pdfTemplate = Path.Combine(webRoot, saveToFullPath);
                }
                if (File.Exists(saveToFullPath))
                    File.Delete(saveToFullPath);
                doc.Save(saveToFullPath);
                // close pdf document
                doc.Close();
                return (true, saveToFullPath);
            }
            catch (Exception _exp) { return (false, _exp.ToString()); }
        }

        public async Task<(bool status, string fullPath)> FullPathToPDFAsync(string path, string saveToFullPath = null)
        {

            try
            {
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                string htmlString = await File.ReadAllTextAsync(path, Encoding.Default);
                PdfDocument doc = converter.ConvertHtmlString(htmlString);
                // save pdf document
                if (string.IsNullOrEmpty(saveToFullPath))
                {
                    var webRoot = env.ContentRootPath;
                    Guid fileId = Guid.NewGuid();
                    string newFilePath = $@"{webRoot}\PDFGeneratedFiles\";
                    newFilePath += fileId + ".pdf";
                    saveToFullPath = newFilePath;
                    string pdfTemplate = Path.Combine(webRoot, saveToFullPath);
                }
                if (File.Exists(saveToFullPath))
                    File.Delete(saveToFullPath);
                doc.Save(saveToFullPath);
                // close pdf document
                doc.Close();
                return (true, saveToFullPath);
            }
            catch (Exception _exp) { return (false, _exp.ToString()); }
        }

        public (bool status, byte[] fileStreamArray) URLToPDFStream(Uri uri)
        {
            try
            {
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertUrl(uri.AbsoluteUri);
                // save pdf document
                  string saveToFullPath = Guid.NewGuid().ToString();
                    var webRoot = env.ContentRootPath;
                    Guid fileId = Guid.NewGuid();
                    string newFilePath = $@"{webRoot}\PDFGeneratedFiles\";
                    newFilePath += fileId + ".pdf";
                    saveToFullPath = newFilePath;
                    string pdfTemplate = Path.Combine(webRoot, saveToFullPath);
                if (File.Exists(saveToFullPath))
                    File.Delete(saveToFullPath);
                doc.Save(saveToFullPath);
                // close pdf document
                doc.Close();
                var result =  (true, File.ReadAllBytes(saveToFullPath));
                // delete file after getting results 
                if (File.Exists(saveToFullPath))
                    File.Delete(saveToFullPath);
                return result;
            }
            catch (Exception) { return (false, Array.Empty<byte>()); }
        }

        public byte[] HtmlStringToPDF(string htmlString)
        {
            try
            {
                HtmlToPdf converter = new HtmlToPdf();                
                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertHtmlString(htmlString);                

                // save pdf document
                var webRoot = env.ContentRootPath;
                Guid fileId = Guid.NewGuid();
                string newFilePath = Path.Combine(webRoot, "PDFGeneratedFiles");
                newFilePath = Path.Combine(newFilePath, fileId + ".pdf");
                string saveToFullPath = newFilePath;
                
                if (File.Exists(saveToFullPath)) File.Delete(saveToFullPath);                
                doc.Save(saveToFullPath);
                // close pdf document
                doc.Close();
                
                var result = File.ReadAllBytes(saveToFullPath);                
                // delete file after getting results 
                if (File.Exists(saveToFullPath)) File.Delete(saveToFullPath);

                return result;
            }
            catch (Exception) { return Array.Empty<byte>(); }
        }
    }
}
