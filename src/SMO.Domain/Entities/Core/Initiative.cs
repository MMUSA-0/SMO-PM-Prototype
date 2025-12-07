using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// Strategic Initiative - المبادرة الاستراتيجية
    /// Core entity shared across modules (M3: Program Management owns this)
    /// </summary>
    public class Initiative : AuditableEntity
    {
        public string Code { get; set; } // INIT.001, INIT.002, etc.
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int ProgramId { get; set; }
        public string Status { get; set; } // NotStarted, InProgress, Completed, OnHold, Cancelled
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public int ProgressPercentage { get; set; }
        public string Owner { get; set; }
        public string Priority { get; set; } // Critical, High, Medium, Low
        public string Scope { get; set; }
        public string ExpectedOutcome { get; set; }
        
        // Navigation Properties
        public virtual VisionProgram Program { get; set; }
        public virtual ICollection<InitiativeMilestone> Milestones { get; set; }
        public virtual ICollection<KPI> KPIs { get; set; }
        public virtual ICollection<InitiativeRisk> Risks { get; set; }
        
        public Initiative()
        {
            Milestones = new HashSet<InitiativeMilestone>();
            KPIs = new HashSet<KPI>();
            Risks = new HashSet<InitiativeRisk>();
        }
    }
}
