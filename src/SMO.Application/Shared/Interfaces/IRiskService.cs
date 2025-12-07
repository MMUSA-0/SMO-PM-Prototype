using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared Risk Service - Used by all modules
    /// Risk Management (M4) owns this data, Performance (M1) reads only
    /// </summary>
    public interface IRiskService
    {
        // Read operations (all modules can read)
        Task<List<ProgramRiskSummaryDto>> GetProgramRisksAsync(int programId);
        Task<List<ProgramRiskSummaryDto>> GetTopProgramRisksAsync(int programId, int count = 5);
        Task<RiskDetailsDto> GetRiskByIdAsync(int riskId);
        Task<List<ProgramRiskSummaryDto>> GetCriticalRisksAsync();
        Task<List<ProgramRiskSummaryDto>> GetRisksByCategoryAsync(string category);
        
        // Risk Management operations (M4 only)
        Task<RiskDetailsDto> CreateRiskAsync(RiskDetailsDto risk);
        Task<RiskDetailsDto> UpdateRiskAsync(int id, RiskDetailsDto risk);
        Task<bool> UpdateRiskStatusAsync(int id, string status);
        Task<bool> DeleteRiskAsync(int id);
        
        // Risk assessment operations
        Task<int> CalculateRiskScoreAsync(int probability, int impact);
        Task<string> DetermineRiskLevelAsync(int riskScore);
        Task<RiskMatrixDto> GetRiskMatrixAsync(int programId);
        
        // Mitigation tracking
        Task<List<RiskMitigationDto>> GetMitigationPlansAsync(int riskId);
        Task<bool> UpdateMitigationProgressAsync(int mitigationId, int progress);
    }

    public class RiskDetailsDto
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string RiskCode { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Category { get; set; }
        public int Probability { get; set; }
        public int Impact { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public string CurrentStatus { get; set; }
        public string MitigationStrategyAr { get; set; }
        public string MitigationStrategyEn { get; set; }
        public string Owner { get; set; }
        public DateTime IdentifiedDate { get; set; }
        public DateTime? TargetResolutionDate { get; set; }
        public bool IsTopRisk { get; set; }
    }

    public class RiskMatrixDto
    {
        public int ProgramId { get; set; }
        public List<RiskMatrixItemDto> Risks { get; set; }
        public int[,] Matrix { get; set; } // 5x5 matrix
    }

    public class RiskMatrixItemDto
    {
        public int RiskId { get; set; }
        public string RiskCode { get; set; }
        public int Probability { get; set; }
        public int Impact { get; set; }
        public string Color { get; set; } // Red, Orange, Yellow, Green
    }

    public class RiskMitigationDto
    {
        public int Id { get; set; }
        public int RiskId { get; set; }
        public string ActionAr { get; set; }
        public string ActionEn { get; set; }
        public string ResponsibleParty { get; set; }
        public DateTime PlannedDate { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; }
    }
}
