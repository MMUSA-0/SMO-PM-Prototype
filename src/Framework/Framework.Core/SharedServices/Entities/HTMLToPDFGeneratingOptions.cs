using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Entities
{
    public class HTMLToPDFGeneratingOptions
    {
        public string HTML { get; set; }
        public string HeaderHtml { get; set; } = null;
        public string FooterHtml { get; set; } = null;
        public PdfPageSize PageSize { get; set; } = PdfPageSize.A4;
        public PdfPageOrientation PageOrientation { get; set; } = PdfPageOrientation.Portrait;
        public HtmlToPdfPageFitMode AutoFitWidth { get; set; } = HtmlToPdfPageFitMode.AutoFit;
        public HtmlToPdfPageFitMode AutoFitHeight { get; set; } = HtmlToPdfPageFitMode.NoAdjustment;
        public int WebPageHeight { get; set; } = 0;
        public bool WebPageFixedSize { get; set; } = false;
        public string saveToFullPath { get; set; } = null;
    }
}