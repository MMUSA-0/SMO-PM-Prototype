// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.SharedServices.Entities
{
    #region usings

    #endregion

    public class Log
    {
        public Guid Id { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string Host { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string MachineName { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string Url { get; set; }
        public DateTime Date { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string Thread { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string LogLevel { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string Logger { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string UserAgent { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string UserName { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string Message { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        public string Exception { get; set; }
        [StringLength(1000)]
        [MaxLength(1000)]
        //public string HttpMethod { get; set; }
        public string CallSite { get; set; }
    }
}