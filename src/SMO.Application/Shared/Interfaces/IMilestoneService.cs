using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared Milestone Service - Used by all modules
    /// Program Management (M3) owns definition, Performance (M1) can update progress
    /// </summary>
    public interface IMilestoneService
    {
        // Read operations
        Task<InitiativeMilestoneDto> GetMilestoneAsync(int id);
        Task<List<InitiativeMilestoneDto>> GetInitiativeMilestonesAsync(int initiativeId);
        Task<List<InitiativeMilestoneDto>> GetUpcomingMilestonesAsync(int days = 30);
        Task<List<InitiativeMilestoneDto>> GetOverdueMilestonesAsync();
        Task<List<InitiativeMilestoneDto>> GetCriticalPathMilestonesAsync(int initiativeId);
        
        // Program Management operations (M3)
        Task<InitiativeMilestoneDto> CreateMilestoneAsync(InitiativeMilestoneDto milestone);
        Task<InitiativeMilestoneDto> UpdateMilestoneAsync(int id, InitiativeMilestoneDto milestone);
        Task<bool> DeleteMilestoneAsync(int id);
        
        // Progress operations (Bi-directional)
        Task<bool> UpdateMilestoneProgressAsync(int id, int progress, string updateSource);
        Task<bool> MarkMilestoneCompleteAsync(int id, DateTime completionDate, string completedBy);
        
        // Dependency management
        Task<bool> AddDependencyAsync(int milestoneId, int dependsOnId);
        Task<bool> RemoveDependencyAsync(int milestoneId, int dependsOnId);
        Task<List<MilestoneDependencyDto>> GetDependenciesAsync(int milestoneId);
        Task<bool> ValidateDependenciesAsync(int milestoneId);
        
        // Analysis
        Task<MilestoneHealthDto> GetMilestoneHealthAsync(int milestoneId);
        Task<CriticalPathDto> CalculateCriticalPathAsync(int initiativeId);
    }

    public class MilestoneDependencyDto
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public int DependsOnId { get; set; }
        public string DependsOnName { get; set; }
        public string DependencyType { get; set; } // FinishToStart, StartToStart, etc.
        public int LagDays { get; set; }
        public bool IsBlocking { get; set; }
    }

    public class MilestoneHealthDto
    {
        public int MilestoneId { get; set; }
        public string Status { get; set; } // OnTrack, AtRisk, Delayed, Completed
        public int DaysUntilDue { get; set; }
        public int ProgressPercentage { get; set; }
        public int ExpectedProgress { get; set; }
        public int ProgressVariance { get; set; }
        public List<string> Risks { get; set; }
        public List<string> Blockers { get; set; }
        public string Recommendation { get; set; }
    }

    public class CriticalPathDto
    {
        public int InitiativeId { get; set; }
        public List<int> CriticalMilestoneIds { get; set; }
        public int TotalDuration { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public int FloatDays { get; set; }
        public List<CriticalPathItemDto> PathItems { get; set; }
    }

    public class CriticalPathItemDto
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Float { get; set; }
        public bool IsOnCriticalPath { get; set; }
    }
}
