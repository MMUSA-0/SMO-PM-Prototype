// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebNotification.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.SharedServices.Entities
{
    #region usings

    #endregion

    public class WebNotification : FullAuditedEntityBase<Guid>
    {
        public WebNotification()
        {
            WebNotificationUsers = new HashSet<WebNotificationUser>();
        }

        public int ApplicationId { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string MessageContentAr { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string MessageContentEn { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string Url { get; set; }

        public virtual ICollection<WebNotificationUser> WebNotificationUsers { get; set; }
    }
}