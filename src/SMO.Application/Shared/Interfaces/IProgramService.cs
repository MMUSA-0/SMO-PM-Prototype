using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared Program Service - Used by all modules
    /// Program Management (M3) owns this data, others read
    /// </summary>
    public interface IProgramService
    {
        // Read operations (all modules can read)
        Task<ProgramDto> GetProgramAsync(int id);
        Task<List<ProgramDto>> GetAllProgramsAsync();
        Task<List<ProgramDto>> GetActiveProgramsAsync();
        Task<ProgramDto> GetProgramByCodeAsync(string code);
        Task<List<ProgramDto>> GetProgramsByPillarAsync(int pillarId);
        Task<List<ProgramDto>> GetProgramsByStatusAsync(string status);
        
        // Program Management operations (M3 only)
        Task<ProgramDto> CreateProgramAsync(ProgramDto program);
        Task<ProgramDto> UpdateProgramAsync(int id, ProgramDto program);
        Task<bool> UpdateProgramStatusAsync(int id, string status);
        Task<bool> DeleteProgramAsync(int id);
        
        // Related data operations
        Task<List<InitiativeDto>> GetProgramInitiativesAsync(int programId);
        Task<List<KPIDto>> GetProgramKPIsAsync(int programId);
        Task<ProgramStatisticsDto> GetProgramStatisticsAsync(int programId);
        
        // Performance data (read-only from M1)
        Task<ProgramPerformanceSummaryDto> GetPerformanceSummaryAsync(int programId, int year, int quarter);
        
        // Budget data (read-only from M5)
        Task<ProgramBudgetSummaryDto> GetBudgetSummaryAsync(int programId, int year);
        
        // Risk data (read-only from M4)
        Task<List<ProgramRiskSummaryDto>> GetTopRisksAsync(int programId, int count = 5);
    }
}
