// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationType.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.SharedServices.Entities
{
    #region usings

    #endregion

    public sealed class NotificationType : FullAuditedEntityBase<int>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationType" /> class.
        /// </summary>
        public NotificationType()
        {
            this.NotificationTemplates = new HashSet<NotificationTemplate>();
        }

        [StringLength(250)]
        [MaxLength(250)]
        public string NameAr { get; set; }
        [StringLength(250)]
        [MaxLength(250)]
        public string NameEn { get; set; }

        /// <summary>
        ///     Gets or sets the notification templates.
        /// </summary>
        public ICollection<NotificationTemplate> NotificationTemplates { get; set; }
    }
}