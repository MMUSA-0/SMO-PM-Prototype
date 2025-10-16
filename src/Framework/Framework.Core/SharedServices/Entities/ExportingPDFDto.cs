using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Entities
{
    public class ExportingPDFReplacementItems
    {

        public static string HtmlTitle = "{{html-title}}";
        public static string SectionLink = "{{section-link}}";
        public static string SectionImg = "{{section-img}}";
        public static string SectionTitle = "{{section-title}}";
        public static string SectionFlag = "{{section-flag}}";
        public static string TableTotalTitle = "{{table-total-title}}";
        public static string TableTotalCounter = "{{table-total-counter}}";
        public static string TableColumnsNames = "{{table-columns-name}}";
        public static string TableRowsValue = "{{table-rows-value}}";
        public static string TableWatermark = "{{table-watermark}}";
        public static string TableDate = "{{table-date}}";
    }

    public class ExportingPDFDto
    {
        public string HtmlTitle { get; set; }
        public string SectionLink { get; set; } 
        public string SectionImg { get; set; }
        public string SectionTitle { get; set; }
        public string SectionFlag { get; set; }
        public string TableTotalTitle { get; set; }
        public string TableTotalCounter { get; set; }
        public string TableColumnsNames { get; set; }
        public string TableRowsValue { get; set; }
        public string TableWatermark { get; set; }
        public DateTime TableDate { get; set; }
    }
}
