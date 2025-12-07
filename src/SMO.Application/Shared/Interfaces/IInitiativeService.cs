using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared Initiative Service - Used by all modules
    /// Program Management (M3) owns this data, Performance (M1) can update progress
    /// Bi-directional sync for progress updates
    /// </summary>
    public interface IInitiativeService
    {
        // Read operations (all modules can read)
        Task<InitiativeDto> GetInitiativeAsync(int id);
        Task<List<InitiativeDto>> GetProgramInitiativesAsync(int programId);
        Task<List<InitiativeDto>> GetActiveInitiativesAsync();
        Task<InitiativeDto> GetInitiativeByCodeAsync(string code);
        Task<InitiativeStatisticsDto> GetInitiativeStatisticsAsync(int initiativeId);
        
        // Program Management operations (M3)
        Task<InitiativeDto> CreateInitiativeAsync(InitiativeDto initiative);
        Task<InitiativeDto> UpdateInitiativeAsync(int id, InitiativeDto initiative);
        Task<bool> UpdateInitiativeStatusAsync(int id, string status);
        Task<bool> DeleteInitiativeAsync(int id);
        
        // Progress operations (M1: Performance & M3: Program - Bi-directional)
        Task<bool> UpdateProgressAsync(int initiativeId, int progressPercentage, string updateSource);
        Task<InitiativeProgressDto> GetProgressDetailsAsync(int initiativeId);
        Task<List<InitiativeProgressHistoryDto>> GetProgressHistoryAsync(int initiativeId);
        
        // Milestone operations
        Task<List<InitiativeMilestoneDto>> GetInitiativeMilestonesAsync(int initiativeId);
        Task<bool> UpdateMilestoneProgressAsync(int milestoneId, int progress, string updateSource);
        
        // Output/Deliverable operations
        Task<List<InitiativeOutputDto>> GetInitiativeOutputsAsync(int initiativeId);
        Task<bool> MarkOutputCompletedAsync(int outputId, DateTime completionDate);
    }

    public class InitiativeStatisticsDto
    {
        public int InitiativeId { get; set; }
        public int OverallProgress { get; set; }
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public int DelayedMilestones { get; set; }
        public int TotalOutputs { get; set; }
        public int CompletedOutputs { get; set; }
        public int TotalKPIs { get; set; }
        public int KPIsOnTrack { get; set; }
        public decimal BudgetUtilization { get; set; }
        public int DaysRemaining { get; set; }
        public int ScheduleVariance { get; set; } // Days ahead/behind
    }

    public class InitiativeProgressDto
    {
        public int InitiativeId { get; set; }
        public int CurrentProgress { get; set; }
        public int PlannedProgress { get; set; }
        public int ProgressVariance { get; set; }
        public string Status { get; set; } // OnTrack, Delayed, Ahead
        public DateTime LastUpdated { get; set; }
        public string LastUpdateSource { get; set; }
        public string ProgressNotes { get; set; }
    }

    public class InitiativeProgressHistoryDto
    {
        public int Id { get; set; }
        public int InitiativeId { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime RecordedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdateSource { get; set; }
        public string Notes { get; set; }
    }

    public class InitiativeMilestoneDto
    {
        public int Id { get; set; }
        public int InitiativeId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; } // NotStarted, InProgress, Completed, Delayed
        public string Owner { get; set; }
        public bool IsCriticalPath { get; set; }
        public List<int> DependencyIds { get; set; }
    }

    public class InitiativeOutputDto
    {
        public int Id { get; set; }
        public int InitiativeId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Type { get; set; } // Document, System, Service, Product
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime PlannedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string Status { get; set; } // Pending, InProgress, Delivered
        public string QualityScore { get; set; }
        public string AcceptedBy { get; set; }
    }
}
