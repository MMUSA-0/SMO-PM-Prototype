using System;
using System.Collections.Generic;

namespace SMO.Application.Shared.DTOs
{
    /// <summary>
    /// Shared KPI DTO - Used across all modules
    /// </summary>
    public class KPIDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public string MeasurementFrequency { get; set; }
        public string CalculationMethod { get; set; }
        public string DataSource { get; set; }
        public string Owner { get; set; }
        public string Polarity { get; set; }
        public decimal? BaselineValue { get; set; }
        public DateTime? BaselineDate { get; set; }
        public decimal? Target2030 { get; set; }
        public bool IsActive { get; set; }
        
        // Related entities
        public int? ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int? InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        
        // Current performance (calculated)
        public decimal? CurrentValue { get; set; }
        public decimal? AchievementRate { get; set; }
        public string Status { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class KPIValueDto
    {
        public int Id { get; set; }
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? ForecastValue { get; set; }
        public decimal? AchievementRate { get; set; }
        public string Status { get; set; }
        public string PerformanceDriver { get; set; }
        public string PerformanceBarrier { get; set; }
        public string BriefExplanation { get; set; }
        public string DataEntrySource { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public string LastSyncSource { get; set; }
    }

    public class UpdateKPIValueRequest
    {
        public int KPIId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal ActualValue { get; set; }
        public string PerformanceDriver { get; set; }
        public string PerformanceBarrier { get; set; }
        public string BriefExplanation { get; set; }
        public string UpdateSource { get; set; } // M1-Performance or M3-Program
    }

    public class PerformanceScoreDto
    {
        public int KPIId { get; set; }
        public decimal Score { get; set; }
        public string Status { get; set; } // Red, Yellow, Green
        public string Trend { get; set; } // Improving, Stable, Declining
        public decimal VarianceFromTarget { get; set; }
        public string CalculationMethod { get; set; }
    }

    public class SyncResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime SyncTimestamp { get; set; }
        public string SourceModule { get; set; }
        public string TargetModule { get; set; }
        public object SyncedData { get; set; }
    }

    public class SyncConflictDto
    {
        public int Id { get; set; }
        public int KPIId { get; set; }
        public string ConflictType { get; set; }
        public DateTime ConflictTime { get; set; }
        public object SourceValue { get; set; }
        public object TargetValue { get; set; }
        public string SourceModule { get; set; }
        public string TargetModule { get; set; }
        public string SuggestedResolution { get; set; }
    }
}
