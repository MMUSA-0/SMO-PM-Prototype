using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// Vision Realization Program (VRP) - البرنامج التنفيذي
    /// Core entity shared across modules (M3: Program Management owns this)
    /// </summary>
    public class VisionProgram : AuditableEntity
    {
        public string Code { get; set; } // VRP.001, VRP.002, etc.
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string VisionAlignment { get; set; } // How it aligns with Vision 2030
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // Active, Suspended, Completed, Cancelled, NotStarted
        public string ProgramOwner { get; set; }
        public string MinistryAr { get; set; }
        public string MinistryEn { get; set; }
        public decimal? TotalBudget { get; set; }
        public string LogoUrl { get; set; }
        
        // Relations
        public int PillarId { get; set; }
        public int? StrategicObjectiveId { get; set; }
        
        // Navigation Properties
        public virtual ICollection<KPI> KPIs { get; set; }
        public virtual ICollection<Initiative> Initiatives { get; set; }
        public virtual ICollection<ProgramRisk> Risks { get; set; }
        public virtual ICollection<ProgramBudget> Budgets { get; set; }
        
        public VisionProgram()
        {
            KPIs = new HashSet<KPI>();
            Initiatives = new HashSet<Initiative>();
            Risks = new HashSet<ProgramRisk>();
            Budgets = new HashSet<ProgramBudget>();
        }
    }
}
