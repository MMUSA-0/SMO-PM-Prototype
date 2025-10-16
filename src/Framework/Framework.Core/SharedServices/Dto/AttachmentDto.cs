using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Dto
{
    public class AttachmentDto
    {
        public Guid? AttachmentId { get; set; }
        public Guid? entityAttachmentId { get { return AttachmentId; } }

        public string? FileName { get; set; }
        public string? Extension { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public int? AttachmentTypeId { get; set; }
        public byte[]? Thumbnail { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public bool IsNew { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public double? Size { get; set; }
        public bool? CanDelete { get; set; }

    }

    public class UploadedAttachmentDto
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
