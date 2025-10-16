using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Rectangle = iTextSharp.text.Rectangle;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using Framework.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.ComponentModel.DataAnnotations.Schema;
using Framework.Core.Globalization;

namespace Framework.Core.Drawing
{
    /// <summary>
    /// The PDFHelper class provides helper methods to perform various operations on PDF documents.
    /// This class uses iTextSharp package version 5.5.13.3
    /// </summary>
    public class PDFHelper
    {
        public PDFHelper()
        {
        }

        /// <summary>
        /// Method used to PDF document from List<Item>
        /// Created By: Mahmoud Salah Elwakeil
        /// Created On: 30-05-2023
        /// </summary>
        /// <param name="data">List of Items to generate PDF for</param>
        /// <param name="documentTitleText">Optional documentTitleText</param>
        /// <returns>result pdf document as byte[]</returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> GeneratePdf<T>(List<T> data, string documentTitleText = null)
        {
            byte[] result = null;

            var variables = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var tahomaFontFile = Path.Combine(variables, "Tahoma.ttf");

            // Creating document object
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);

                BaseFont baseFont = BaseFont.CreateFont(tahomaFontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                var headerFont = new iTextSharp.text.Font(baseFont, 13, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var bodyFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
                var direction = CultureHelper.IsArabic ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;

                Document doc = new Document(rec);
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                writer.RunDirection = direction;

                doc.Open();

                // Creating paragraph for header
                if (!string.IsNullOrEmpty(documentTitleText))
                {
                    //BaseFont bfntHead = BaseFont.CreateFont(tahomaFontFile, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    //iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 13, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                    Paragraph title = new Paragraph();
                    title.Alignment = Element.ALIGN_CENTER;
                    title.PaddingTop = 150;
                    title.Add(new Chunk(documentTitleText, headerFont));
                    title.Add(new Chunk("\n")); // Add a newline after the title
                    doc.Add(title);
                }
                // Adding a line
                //Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 5)));
                //doc.Add(p);

                // Adding PdfPTable
                PdfPTable table = new PdfPTable(typeof(T).GetProperties().Length);
                table.RunDirection = direction;
                table.DefaultCell.Border = 2;
                table.TotalWidth = 500;

                // Add table headers
                foreach (var property in typeof(T).GetProperties())
                {
                    string cellText = string.Empty;
                    //var notMappedAttribute = property.GetCustomAttributes(typeof(NotMappedAttribute), true).SingleOrDefault();
                    //if (notMappedAttribute != null)
                    //    continue; // Scip this column if has attribute [NotMapped]
                    var attribute = property.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                    if (attribute != null)
                    {
                        DisplayAttribute authAttr = attribute as DisplayAttribute;
                        try
                        {
                            cellText = authAttr.GetName();
                        }
                        catch (Exception ex)
                        {
                            cellText = property.Name;
                        }
                    }
                    else
                    {
                        cellText = property.Name;
                    }

                    PdfPCell cell = new PdfPCell();
                    cell.Phrase = new Phrase(cellText, headerFont);
                    cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#C8C8C8"));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 5;
                    cell.RunDirection = direction;
                    table.AddCell(cell);
                }


                // Writing table data
                foreach (var item in data)
                {
                    foreach (var property in typeof(T).GetProperties())
                    {
                        //var notMappedAttribute = property.GetCustomAttributes(typeof(NotMappedAttribute), true).SingleOrDefault();
                        //if (notMappedAttribute != null)
                        //    continue; // Scip this column if has attribute [NotMapped]

                        var value = property.GetValue(item);
                        var phrase = new Phrase(value != null ? value.ToString() : "", bodyFont);
                        var cell = new PdfPCell(phrase)
                        {
                            RunDirection = direction
                        };
                        table.AddCell(cell);
                    }
                }

                doc.Add(table);
                doc.Close();

                result = ms.ToArray();
            }
            return result;
        }

        public async Task<byte[]> GeneratePdfObjectDetails<T>(T data, string documentTitleText = null)
        {
            byte[] result = null;

            var variables = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var tahomaFontFile = Path.Combine(variables, "Tahoma.ttf");

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);

                BaseFont baseFont = BaseFont.CreateFont(tahomaFontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                var headerFont = new iTextSharp.text.Font(baseFont, 13, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var bodyFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
                var direction = CultureHelper.IsArabic ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;

                Document doc = new Document(rec);
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                writer.RunDirection = direction;
                doc.Open();

                if (!string.IsNullOrEmpty(documentTitleText))
                {
                    Paragraph title = new Paragraph();
                    title.Alignment = Element.ALIGN_CENTER;
                    title.PaddingTop = 150;
                    title.Add(new Chunk(documentTitleText, headerFont));
                    title.Add(new Chunk("\n"));
                    writer.RunDirection = direction;
                    doc.Add(title);
                }

                foreach (var property in typeof(T).GetProperties())
                {
                    var value = property.GetValue(data);
                    var propertyName = property.Name;
                    
                    var bidiLine = new BidiLine();

                    //bidiLine.AddChunk(new Chunk($"{propertyName}: ", headerFont));
                    //bidiLine.AddChunk(new Chunk(value != null ? value.ToString() : "", bodyFont));
                    //bidiLine.RunDirection = direction;
                    //bidiLine.AddChunk(new Chunk("\n"));
                    //doc.Add(bidiLine.CreatePdfPCell());

                    var line = new Paragraph();
                    line.Alignment = Element.ALIGN_LEFT;// CultureHelper.IsArabic ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                    line.Add(new Chunk($"{propertyName}: ", headerFont));
                    line.Add(new Chunk(value != null ? value.ToString() : "", bodyFont));
                    line.Add(new Chunk("\n"));
                    doc.Add(line);
                }

                doc.Close();

                result = ms.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Method used to add a watermark to a specified PDF document
        /// Created By: Mahmoud Salah Elwakeil
        /// Created On: 30-05-2023
        /// </summary>
        /// <param name="fileContent">PDF fileContent as byte[]</param>
        /// <param name="watermarkText">Text to be added as watermark</param>
        /// <param name="font">optional System.Drawing.Font (if not passed ==> default is tahoma)</param>
        /// <param name="fontColor">optional System.Drawing.Color (if not passed ==> default is Gray)</param>
        /// <param name="fontSize">optional font size for watermarkText (default is 50)</param>
        /// <param name="textRotationAngle">optional textRotationAngle for watermarkText (default is 45)</param>
        /// <returns>PDF document as byte[] </returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> AddWatermark(byte[] fileContent, string watermarkText, int fontSize = 50, int textRotationAngle = 45)
        {
            byte[] resultByteArray = null;
            try
            {
                var variables = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                var tahomaFontFile = Path.Combine(variables, "Tahoma.ttf");
                int defaultEmSize = 15;
                FontStyle defaultFontStyle = FontStyle.Bold;

                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new PdfReader(fileContent))
                    using (var stamper = new PdfStamper(reader, memoryStream))
                    {
                        var pages = reader.NumberOfPages;
                        for (var i = 1; i <= pages; i++)
                        {
                            var dc = stamper.GetOverContent(i);
                            BaseFont baseFont = BaseFont.CreateFont(tahomaFontFile, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            AddWaterMarkText(dc, watermarkText, baseFont, fontSize, textRotationAngle, BaseColor.DARK_GRAY, reader.GetPageSizeWithRotation(i));
                        }
                        stamper.Close();
                    }
                    resultByteArray = memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while attempting to read file, The error is - " + ex.ToString());
            }
            return resultByteArray;
        }

        private void AddWaterMarkText(PdfContentByte pdfData, string watermarkText, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize)
        {
            var gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            pdfData.SaveState();
            pdfData.SetGState(gstate);
            pdfData.SetColorFill(color);
            pdfData.BeginText();
            pdfData.SetFontAndSize(font, fontSize);

            var x = (realPageSize.Right + realPageSize.Left) / 2;
            var y = (realPageSize.Bottom + realPageSize.Top) / 2;
            pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText ?? string.Empty, x, y, angle);
            pdfData.EndText();
            pdfData.RestoreState();
        }

    }
}
