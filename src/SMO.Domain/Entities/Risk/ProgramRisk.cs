using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Risk
{
    /// <summary>
    /// Program Risk - مخاطر البرنامج (UC-12 BRD)
    /// Owned by M4 (Risk Management), Read-only for M1 (Performance)
    /// </summary>
    public class ProgramRisk : AuditableEntity
    {
        public int ProgramId { get; set; }
        
        // UC-12 BRD Fields (Lines 1772-1787)
        public string RiskCode { get; set; } // VRP_X_XXX_### format
        public string RiskName { get; set; } // اسم المخاطرة
        public bool IsTop5 { get; set; } // هل هي من أفضل خمسة
        public string RiskLevelType { get; set; } // رؤية/برنامج/مبادرة
        public int? InitiativeId { get; set; } // If risk is at initiative level
        public string SubmittingEntity { get; set; } // الجهة المقدمة للطلب
        public string ResponsibleEntity { get; set; } // الجهة المعنية
        
        // Risk Assessment
        public int Probability { get; set; } // الاحتمالية (1-5)
        public int Impact { get; set; } // التأثير (1-5)
        public int RiskScore { get; set; } // درجة المخاطرة (auto-calculated)
        
        // Mitigation Details
        public string MitigationStatus { get; set; } // حالة إجراء التخفيف
        public int MitigationActionsCount { get; set; } // عدد إجراءات التخفيف
        public decimal MitigationCompletionPercentage { get; set; } // النسبة المئوية للإجراءات المغلقة
        public string MitigationActions { get; set; } // إجراءات التخفيف
        
        // Time Tracking
        public int RiskYear { get; set; } // السنة
        public string RiskQuarter { get; set; } // الربع (Q1, Q2, Q3, Q4)
        public DateTime IdentifiedDate { get; set; }
        public DateTime? TargetResolutionDate { get; set; }
        public DateTime? ActualResolutionDate { get; set; }
        
        // Status and Classification
        public string RiskStatus { get; set; } // حالة المخاطرة (مفتوح/مغلق)
        public string Category { get; set; } // Strategic, Operational, Financial, Technical, Compliance
        public string RiskLevel { get; set; } // Critical, High, Medium, Low (based on score)
        
        // Additional Details
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Owner { get; set; }
        public string TriggerEvent { get; set; }
        public string ResidualRiskLevel { get; set; } // After mitigation
        public string EscalationReason { get; set; } // سبب التصعيد
        public DateTime? EscalationDate { get; set; }
        public bool IsArchived { get; set; } // للأرشفة بدلاً من الحذف النهائي
        
        // Legacy fields for compatibility
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string MitigationStrategyAr { get; set; }
        public string MitigationStrategyEn { get; set; }
        
        // Navigation Properties
        public virtual Core.VisionProgram Program { get; set; }
        public virtual Core.Initiative Initiative { get; set; }
    }
}
