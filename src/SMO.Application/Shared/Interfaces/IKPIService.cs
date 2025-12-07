using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared KPI Service - Used by all modules
    /// Program Management (M3) owns definition, Performance (M1) can update values
    /// Bi-directional sync for actual values
    /// </summary>
    public interface IKPIService
    {
        // Read operations (all modules can read)
        Task<KPIDto> GetKPIAsync(int id);
        Task<List<KPIDto>> GetKPIsByProgramAsync(int programId);
        Task<List<KPIDto>> GetKPIsByInitiativeAsync(int initiativeId);
        Task<List<KPIDto>> GetActiveKPIsAsync();
        
        // Definition operations (M3: Program Management only)
        Task<KPIDto> CreateKPIAsync(KPIDto kpi);
        Task<KPIDto> UpdateKPIDefinitionAsync(int id, KPIDto kpi);
        Task<bool> DeactivateKPIAsync(int id);
        
        // Value operations (M1: Performance & M3: Program - Bi-directional)
        Task<KPIValueDto> GetLatestValueAsync(int kpiId);
        Task<List<KPIValueDto>> GetValuesAsync(int kpiId, int year, int? quarter = null);
        Task<KPIValueDto> UpdateActualValueAsync(UpdateKPIValueRequest request);
        Task<KPIValueDto> UpdateForecastValueAsync(int kpiId, int year, int? quarter, decimal forecast);
        
        // Calculation operations (Shared logic)
        Task<decimal> CalculateAchievementRateAsync(int kpiId, decimal actualValue);
        Task<string> DetermineStatusAsync(int kpiId, decimal achievementRate);
        Task<PerformanceScoreDto> CalculatePerformanceScoreAsync(int kpiId);
        
        // Sync operations
        Task<SyncResultDto> SyncKPIValueAsync(int kpiId, string sourceModule);
        Task<List<SyncConflictDto>> GetSyncConflictsAsync(int kpiId);
        Task<bool> ResolveSyncConflictAsync(int conflictId, string resolution);
    }
}
