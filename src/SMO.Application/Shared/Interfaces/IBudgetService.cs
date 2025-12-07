using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMO.Application.Shared.DTOs;

namespace SMO.Application.Shared.Interfaces
{
    /// <summary>
    /// Shared Budget Service - Used by all modules
    /// Financial Management (M5) owns this data, Performance (M1) reads only
    /// Updates every 4 hours per sync rules
    /// </summary>
    public interface IBudgetService
    {
        // Read operations (all modules can read)
        Task<ProgramBudgetSummaryDto> GetProgramBudgetAsync(int programId, int year);
        Task<InitiativeBudgetDto> GetInitiativeBudgetAsync(int initiativeId, int year);
        Task<List<BudgetAllocationDto>> GetBudgetAllocationsAsync(int programId);
        Task<decimal> GetUtilizationRateAsync(int programId, int year);
        Task<BudgetForecastDto> GetBudgetForecastAsync(int programId, int year);
        
        // Financial Management operations (M5 only)
        Task<ProgramBudgetSummaryDto> CreateBudgetAsync(ProgramBudgetSummaryDto budget);
        Task<ProgramBudgetSummaryDto> UpdateBudgetAsync(int id, ProgramBudgetSummaryDto budget);
        Task<bool> RecordExpenditureAsync(int budgetId, decimal amount, string description);
        Task<bool> TransferBudgetAsync(int fromId, int toId, decimal amount);
        
        // Analysis operations
        Task<BudgetVarianceDto> CalculateVarianceAsync(int programId, int year, int? quarter = null);
        Task<CashFlowDto> GetCashFlowAnalysisAsync(int programId, int year);
        Task<List<BudgetAlertDto>> GetBudgetAlertsAsync(int programId);
    }

    public class InitiativeBudgetDto
    {
        public int InitiativeId { get; set; }
        public int Year { get; set; }
        public decimal ApprovedBudget { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal ActualCost { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal UtilizationRate { get; set; }
        public string Status { get; set; }
    }

    public class BudgetAllocationDto
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string AllocationType { get; set; } // Capital, Operational
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime AllocationDate { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class BudgetForecastDto
    {
        public int ProgramId { get; set; }
        public int Year { get; set; }
        public decimal CurrentUtilization { get; set; }
        public decimal ProjectedUtilization { get; set; }
        public decimal ExpectedOverrun { get; set; }
        public DateTime ForecastDate { get; set; }
        public string Assumptions { get; set; }
    }

    public class BudgetVarianceDto
    {
        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string VarianceType { get; set; } // Favorable, Unfavorable
        public string Explanation { get; set; }
    }

    public class CashFlowDto
    {
        public int ProgramId { get; set; }
        public int Year { get; set; }
        public List<MonthlyFlowDto> MonthlyFlows { get; set; }
        public decimal TotalInflow { get; set; }
        public decimal TotalOutflow { get; set; }
        public decimal NetCashFlow { get; set; }
    }

    public class MonthlyFlowDto
    {
        public int Month { get; set; }
        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public decimal NetFlow { get; set; }
    }

    public class BudgetAlertDto
    {
        public string AlertType { get; set; } // OverBudget, UnderUtilized, Unusual
        public string Severity { get; set; } // High, Medium, Low
        public string Message { get; set; }
        public decimal ThresholdValue { get; set; }
        public decimal ActualValue { get; set; }
        public DateTime AlertDate { get; set; }
    }
}
