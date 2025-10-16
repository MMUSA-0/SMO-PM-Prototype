using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Helpers
{
    public static class AttachmentMimeType
    {
        // Canonical MIME constants (reuse these anywhere)
        public const string ImageJpeg = "image/jpeg";
        public const string ImagePng = "image/png";
        public const string ImageTiff = "image/tiff";
        public const string ImageGif = "image/gif";
        public const string ImageBmp = "image/bmp";

        public const string AudioMpeg = "audio/mpeg";   // .mp3
        public const string AudioMp3 = "audio/mp3";    // alt
        public const string AudioWav = "audio/wav";
        public const string AudioXWav = "audio/x-wav";  // alt
        public const string VideoMp4 = "video/mp4";

        public const string TextPlain = "text/plain";
        public const string AppRtf = "application/rtf";
        public const string TextRtf = "text/rtf";     // alt
        public const string AppPdf = "application/pdf";
        public const string TextCsv = "text/csv";
        public const string AppCsv = "application/csv"; // alt

        public const string ExcelXls = "application/vnd.ms-excel"; // .xls, .xlt
        public const string ExcelXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string ExcelXltx = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";

        public const string WordDoc = "application/msword"; // .doc, .dot
        public const string WordDocx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string WordDotx = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";

        public const string Ppt = "application/vnd.ms-powerpoint";
        public const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
        public const string Ppsx = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
        public const string Sldx = "application/vnd.openxmlformats-officedocument.presentationml.slide";

        public const string Zip = "application/zip";
        public const string ZipWin = "application/x-zip-compressed"; // alt
        public const string Rar = "application/vnd.rar";
        public const string RarX = "application/x-rar-compressed"; // alt

        // Private backing set (fast lookups, not modifiable from outside)
        private static readonly HashSet<string> _allowedSet = new(StringComparer.OrdinalIgnoreCase)
    {
        ImageJpeg, ImagePng, ImageTiff, ImageGif, ImageBmp,
        AudioMpeg, AudioMp3, AudioWav, AudioXWav, VideoMp4,
        TextPlain, AppRtf, TextRtf, AppPdf, TextCsv, AppCsv,
        ExcelXls, ExcelXlsx, ExcelXltx,
        WordDoc, WordDocx, WordDotx,
        Ppt, Pptx, Ppsx, Sldx,
        Zip, ZipWin, Rar, RarX
    };

        // Public read-only view for enumeration (cannot be mutated externally)
        public static IReadOnlyCollection<string> AllowedMimeTypes { get; } =
            _allowedSet.ToArray(); // snapshot; or use: Array.AsReadOnly(_allowedSet.ToArray())

        // Convenience check
        public static bool IsAllowed(string? contentType) =>
            !string.IsNullOrWhiteSpace(contentType) && _allowedSet.Contains(contentType);
    }
}
