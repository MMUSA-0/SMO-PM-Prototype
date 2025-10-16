using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Framework.Core.Extensions
{
    public static class ExcelExtensions
    {
        public static MemoryStream ConvertListToExcel<T>(this List<T> query)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Result");

                //populate our Data
                if (query.Any())
                {
                    worksheet.Cells["A1"].LoadFromCollection(query, true, TableStyles.Medium2);
                }
                foreach (var item in worksheet.Cells)
                {
                    if (item.Text.ToLower().Contains("date"))
                    {
                        var rang = item.Address + ":" + item.Address;
                        if (item.Address.Contains("1"))
                        {
                            rang = item.Address.Split('1')[0] + ":" + item.Address.Split('1')[0];
                        }
                        worksheet.Cells[rang].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";

                    }
                }
                return new MemoryStream(package.GetAsByteArray());

            }
        }
    }
}
