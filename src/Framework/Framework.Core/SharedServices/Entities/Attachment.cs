using Framework.Core.Data;
using System;

namespace Framework.Core.SharedServices.Entities
{
    #region usings

    #endregion

    public partial class Attachment : FullAuditedEntityBase<Guid>
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string? FilePath { get; set; }
        public string ContentType { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int AttachmentTypeId { get; set; }
        public byte[]? Thumbnail { get; set; }

        public AttachmentType AttachmentType { get; set; }
        public byte[]? FileContent { get; set; }

        public bool IsTransferred { get; set; }
    }

}


