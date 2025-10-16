// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentType.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;

namespace Framework.Core.SharedServices.Entities
{
    #region usings

    #endregion

    public class AttachmentType : FullAuditedEntityBase<int>
    {
        public AttachmentType()
        {
            Attachments = new HashSet<Attachment>();
        }

        [StringLength(250)]
        [MaxLength(250)]
        public string NameAr { get; set; }
        [StringLength(250)]
        [MaxLength(250)]
        public string NameEn { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string Code { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string AllowedFilesExtension { get; set; }

        public int? ImageMaxHeight { get; set; }

        public int? ImageMaxWidth { get; set; }

        public bool IsImage { get; set; }

        public bool IsMandatory { get; set; }

        public int MaxSizeInMegabytes { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }


    public enum AttachmentTypes
    {
        GeneralFileAttachment = 1, // All of extensions below
        FacilitatorImg = 2, // .jpg,.jpeg,.png
        FacilitatorVideo = 3, // .bmp,.mp4,.wav
    }
}