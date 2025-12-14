using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Shared
{
    /// <summary>
    /// Shared Risk Entity - Used by both Performance Management and Vision 2030 modules
    /// This is a SHARED RESOURCE that can be linked to multiple modules
    /// </summary>
    public class Risk : AuditableEntity
    {
        public string Code { get; set; } // RISK-001, RISK-002, etc.
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        
        // Risk Assessment
        public string Category { get; set; } // Technical, Financial, Operational, Strategic, Compliance
        public string Probability { get; set; } // VeryLow, Low, Medium, High, VeryHigh
        public string Impact { get; set; } // VeryLow, Low, Medium, High, VeryHigh
        public decimal RiskScore { get; set; } // Calculated from Probability x Impact
        
        // Risk Management
        public string Status { get; set; } // Open, Mitigating, Closed, Escalated
        public string MitigationPlan { get; set; }
        public string MitigationPlanAr { get; set; }
        public string ContingencyPlan { get; set; }
        
        // Ownership
        public string Owner { get; set; }
        public string EscalatedTo { get; set; }
        
        // Dates
        public DateTime IdentifiedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        
        // Module Tracking
        public string OriginModule { get; set; } // Performance, Vision2030, Shared
        
        // Navigation Properties - Links to both modules
        public virtual ICollection<ProgramRisk> ProgramRisks { get; set; }
        public virtual ICollection<InitiativeRisk> InitiativeRisks { get; set; }
        public virtual ICollection<PerformanceRisk> PerformanceRisks { get; set; }
        
        public Risk()
        {
            ProgramRisks = new HashSet<ProgramRisk>();
            InitiativeRisks = new HashSet<InitiativeRisk>();
            PerformanceRisks = new HashSet<PerformanceRisk>();
            Status = "Open";
            IdentifiedDate = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Calculates risk score based on probability and impact
        /// </summary>
        public void CalculateRiskScore()
        {
            var probValue = GetNumericValue(Probability);
            var impactValue = GetNumericValue(Impact);
            RiskScore = probValue * impactValue;
        }
        
        private decimal GetNumericValue(string level)
        {
            return level?.ToLower() switch
            {
                "verylow" => 1,
                "low" => 2,
                "medium" => 3,
                "high" => 4,
                "veryhigh" => 5,
                _ => 3 // Default to medium
            };
        }
    }
}
