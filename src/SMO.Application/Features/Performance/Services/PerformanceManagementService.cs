using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SMO.Application.Features.Performance.DTOs;
using SMO.Domain.Entities.Performance;
using SMO.Domain.Interfaces;

namespace SMO.Application.Features.Performance.Services
{
    public interface IPerformanceManagementService
    {
        // Macroeconomic Indicators
        Task<List<MacroeconomicIndicatorDto>> GetMacroeconomicIndicatorsAsync();
        Task<MacroeconomicIndicatorDto> GetMacroeconomicIndicatorByIdAsync(int id);
        Task<bool> UpdateMEIValueAsync(UpdateMEIValueRequest request);
        
        // Program Performance
        Task<List<ProgramPerformanceDto>> GetProgramPerformancesAsync(int? year = null, int? quarter = null);
        Task<ProgramPerformanceDto> GetProgramPerformanceByIdAsync(int id);
        Task<ProgramPerformanceDto> CreateProgramPerformanceAsync(ProgramPerformanceDto dto);
        Task<bool> UpdateProgramPerformanceAsync(int id, ProgramPerformanceDto dto);
        Task<bool> SubmitProgramPerformanceForApprovalAsync(int id);
        
        // Performance Calculations
        Task<decimal> CalculatePerformanceScoreAsync(int indicatorId, decimal actualValue);
        Task<bool> RecalculateAllPerformanceScoresAsync();
        
        // Reporting
        Task<byte[]> GenerateQuarterlyReportAsync(int programId, int year, int quarter);
        Task<PerformanceDashboardDto> GetPerformanceDashboardAsync();
    }

    public class PerformanceManagementService : IPerformanceManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PerformanceManagementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MacroeconomicIndicatorDto>> GetMacroeconomicIndicatorsAsync()
        {
            var indicators = await _unitOfWork.Repository<MacroeconomicIndicator>()
                .GetAll()
                .Include(m => m.Values.Where(v => v.Year == DateTime.Now.Year))
                .ToListAsync();

            var dtos = _mapper.Map<List<MacroeconomicIndicatorDto>>(indicators);
            
            // Calculate current values and performance scores
            foreach (var dto in dtos)
            {
                var latestValue = indicators
                    .FirstOrDefault(i => i.Id == dto.Id)?
                    .Values?
                    .OrderByDescending(v => v.Year)
                    .ThenByDescending(v => v.Quarter)
                    .FirstOrDefault();

                if (latestValue != null)
                {
                    dto.CurrentValue = latestValue.ActualValue;
                    dto.PerformanceScore = latestValue.PerformanceScore;
                    dto.Status = DetermineStatus(latestValue.PerformanceScore);
                }
            }

            return dtos;
        }

        public async Task<MacroeconomicIndicatorDto> GetMacroeconomicIndicatorByIdAsync(int id)
        {
            var indicator = await _unitOfWork.Repository<MacroeconomicIndicator>()
                .GetAll()
                .Include(m => m.Values)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (indicator == null)
                return null;

            var dto = _mapper.Map<MacroeconomicIndicatorDto>(indicator);
            dto.RecentValues = _mapper.Map<List<MacroeconomicIndicatorValueDto>>(
                indicator.Values.OrderByDescending(v => v.Year).Take(10)
            );

            return dto;
        }

        public async Task<bool> UpdateMEIValueAsync(UpdateMEIValueRequest request)
        {
            var indicator = await _unitOfWork.Repository<MacroeconomicIndicator>()
                .GetByIdAsync(request.IndicatorId);

            if (indicator == null)
                return false;

            var value = await _unitOfWork.Repository<MacroeconomicIndicatorValue>()
                .GetAll()
                .FirstOrDefaultAsync(v => 
                    v.MacroeconomicIndicatorId == request.IndicatorId &&
                    v.Year == request.Year &&
                    v.Quarter == request.Quarter &&
                    v.Month == request.Month);

            if (value == null)
            {
                value = new MacroeconomicIndicatorValue
                {
                    MacroeconomicIndicatorId = request.IndicatorId,
                    Year = request.Year,
                    Quarter = request.Quarter,
                    Month = request.Month
                };
                await _unitOfWork.Repository<MacroeconomicIndicatorValue>().AddAsync(value);
            }

            value.ActualValue = request.ActualValue;
            value.ForecastValue = request.ForecastValue;
            value.Description = request.Description;
            value.LastCalculationDate = DateTime.Now;

            // Calculate performance score
            if (request.ActualValue.HasValue && indicator.BaselineValue.HasValue && indicator.TargetValue2030.HasValue)
            {
                value.PerformanceScore = CalculatePerformance(
                    request.ActualValue.Value,
                    indicator.BaselineValue.Value,
                    indicator.TargetValue2030.Value,
                    indicator.Polarity
                );
                value.Status = DetermineStatus(value.PerformanceScore);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProgramPerformanceDto>> GetProgramPerformancesAsync(int? year = null, int? quarter = null)
        {
            var query = _unitOfWork.Repository<ProgramPerformance>()
                .GetAll()
                .Include(p => p.Program)
                .Include(p => p.KPIValues)
                .Include(p => p.Achievements)
                .Include(p => p.Risks)
                .Include(p => p.Budgets)
                .AsQueryable();

            if (year.HasValue)
                query = query.Where(p => p.Year == year.Value);

            if (quarter.HasValue)
                query = query.Where(p => p.Quarter == quarter.Value);

            var performances = await query.ToListAsync();
            return _mapper.Map<List<ProgramPerformanceDto>>(performances);
        }

        public async Task<ProgramPerformanceDto> GetProgramPerformanceByIdAsync(int id)
        {
            var performance = await _unitOfWork.Repository<ProgramPerformance>()
                .GetAll()
                .Include(p => p.Program)
                .Include(p => p.KPIValues)
                    .ThenInclude(k => k.KPI)
                .Include(p => p.Achievements)
                .Include(p => p.Risks)
                .Include(p => p.Budgets)
                .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<ProgramPerformanceDto>(performance);
        }

        public async Task<ProgramPerformanceDto> CreateProgramPerformanceAsync(ProgramPerformanceDto dto)
        {
            var performance = _mapper.Map<ProgramPerformance>(dto);
            performance.Status = "Draft";
            performance.CreatedDate = DateTime.Now;

            await _unitOfWork.Repository<ProgramPerformance>().AddAsync(performance);
            await _unitOfWork.SaveChangesAsync();

            return await GetProgramPerformanceByIdAsync(performance.Id);
        }

        public async Task<bool> UpdateProgramPerformanceAsync(int id, ProgramPerformanceDto dto)
        {
            var performance = await _unitOfWork.Repository<ProgramPerformance>()
                .GetByIdAsync(id);

            if (performance == null || performance.Status == "Verified")
                return false;

            _mapper.Map(dto, performance);
            performance.ModifiedDate = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitProgramPerformanceForApprovalAsync(int id)
        {
            var performance = await _unitOfWork.Repository<ProgramPerformance>()
                .GetByIdAsync(id);

            if (performance == null || performance.Status != "Draft")
                return false;

            performance.Status = "UnderReview";
            performance.SubmissionDate = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculatePerformanceScoreAsync(int indicatorId, decimal actualValue)
        {
            var indicator = await _unitOfWork.Repository<MacroeconomicIndicator>()
                .GetByIdAsync(indicatorId);

            if (indicator == null || !indicator.BaselineValue.HasValue || !indicator.TargetValue2030.HasValue)
                return 0;

            return CalculatePerformance(
                actualValue,
                indicator.BaselineValue.Value,
                indicator.TargetValue2030.Value,
                indicator.Polarity
            );
        }

        public async Task<bool> RecalculateAllPerformanceScoresAsync()
        {
            var indicators = await _unitOfWork.Repository<MacroeconomicIndicator>()
                .GetAll()
                .Include(m => m.Values)
                .ToListAsync();

            foreach (var indicator in indicators)
            {
                foreach (var value in indicator.Values.Where(v => v.ActualValue.HasValue))
                {
                    if (indicator.BaselineValue.HasValue && indicator.TargetValue2030.HasValue)
                    {
                        value.PerformanceScore = CalculatePerformance(
                            value.ActualValue.Value,
                            indicator.BaselineValue.Value,
                            indicator.TargetValue2030.Value,
                            indicator.Polarity
                        );
                        value.Status = DetermineStatus(value.PerformanceScore);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> GenerateQuarterlyReportAsync(int programId, int year, int quarter)
        {
            // Implementation for generating Excel/PDF reports
            // This would integrate with EPPlus or iText libraries
            throw new NotImplementedException("Report generation will be implemented with reporting libraries");
        }

        public async Task<PerformanceDashboardDto> GetPerformanceDashboardAsync()
        {
            var dashboard = new PerformanceDashboardDto();

            // Get MEI summary
            var meiValues = await _unitOfWork.Repository<MacroeconomicIndicatorValue>()
                .GetAll()
                .Where(v => v.Year == DateTime.Now.Year)
                .ToListAsync();

            dashboard.TotalMEIIndicators = await _unitOfWork.Repository<MacroeconomicIndicator>().CountAsync();
            dashboard.MEIOnTrack = meiValues.Count(v => v.Status == "Green");
            dashboard.MEIAtRisk = meiValues.Count(v => v.Status == "Yellow");
            dashboard.MEIOffTrack = meiValues.Count(v => v.Status == "Red");

            // Get program performance summary
            var programPerformances = await _unitOfWork.Repository<ProgramPerformance>()
                .GetAll()
                .Where(p => p.Year == DateTime.Now.Year)
                .ToListAsync();

            dashboard.TotalPrograms = programPerformances.Select(p => p.ProgramId).Distinct().Count();
            dashboard.ProgramsOnTrack = programPerformances.Count(p => p.OverallPerformance >= 90);
            dashboard.ProgramsAtRisk = programPerformances.Count(p => p.OverallPerformance >= 70 && p.OverallPerformance < 90);
            dashboard.ProgramsOffTrack = programPerformances.Count(p => p.OverallPerformance < 70);

            return dashboard;
        }

        private decimal CalculatePerformance(decimal actualValue, decimal baselineValue, decimal targetValue, string polarity)
        {
            if (targetValue == baselineValue)
                return 100;

            decimal performance;
            if (polarity == "Increasing")
            {
                performance = ((actualValue - baselineValue) / (targetValue - baselineValue)) * 100;
            }
            else // Decreasing
            {
                performance = ((baselineValue - actualValue) / (baselineValue - targetValue)) * 100;
            }

            return Math.Max(0, Math.Min(100, performance));
        }

        private string DetermineStatus(decimal? performanceScore)
        {
            if (!performanceScore.HasValue)
                return "Gray";

            if (performanceScore >= 90)
                return "Green";
            else if (performanceScore >= 70)
                return "Yellow";
            else
                return "Red";
        }
    }

    public class PerformanceDashboardDto
    {
        public int TotalMEIIndicators { get; set; }
        public int MEIOnTrack { get; set; }
        public int MEIAtRisk { get; set; }
        public int MEIOffTrack { get; set; }
        public int TotalPrograms { get; set; }
        public int ProgramsOnTrack { get; set; }
        public int ProgramsAtRisk { get; set; }
        public int ProgramsOffTrack { get; set; }
    }
}
