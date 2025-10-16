using Framework.Core.SharedServices.Entities;
using Framework.Core.SharedServices.Services;
using SelectPdf;
using System;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Services
{
    public interface IPDFGenerator
    {
        Task<(bool status, string fullPath)> URLToPDFAsync(Uri uri, string saveToFullPath = null, bool useAbsoluteUnlessOrgenal = false);
        (bool status, string fullPath) URLToPDF(Uri uri, string saveToFullPath = null, bool useAbsoluteUnlessOrgenal = false);
        (bool status, string fullPath) FullPathToPDF(string path, string saveToFullPath = null);
        Task<(bool status, string fullPath)> FullPathToPDFAsync(string path, string saveToFullPath = null);
        (bool status, byte[] fileStreamArray) URLToPDFStream(Uri uri);
        byte[] HtmlStringToPDF(string htmlString);
    }
}
