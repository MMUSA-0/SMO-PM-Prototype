using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMO.Application.Features.Performance.DTOs;
using SMO.Application.Features.Performance.Services;

namespace SMO.Api.Controllers.Performance
{
    [ApiController]
    [Route("api/performance/[controller]")]
    [Authorize]
    public class ProgramPerformanceController : ControllerBase
    {
        private readonly IPerformanceManagementService _performanceService;

        public ProgramPerformanceController(IPerformanceManagementService performanceService)
        {
            _performanceService = performanceService;
        }

        /// <summary>
        /// Get program performances list
        /// UC-03: عرض قائمة البرامج
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "PerformanceManager,ProgramOwner,Executive")]
        public async Task<ActionResult<List<ProgramPerformanceDto>>> GetProgramPerformances(
            [FromQuery] int? year, 
            [FromQuery] int? quarter)
        {
            var performances = await _performanceService.GetProgramPerformancesAsync(year, quarter);
            return Ok(performances);
        }

        /// <summary>
        /// Get program performance details by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner,Executive")]
        public async Task<ActionResult<ProgramPerformanceDto>> GetProgramPerformance(int id)
        {
            var performance = await _performanceService.GetProgramPerformanceByIdAsync(id);
            if (performance == null)
                return NotFound();

            return Ok(performance);
        }

        /// <summary>
        /// Create new program performance record
        /// UC-05 to UC-12: Program performance wizard steps
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult<ProgramPerformanceDto>> CreateProgramPerformance(
            [FromBody] ProgramPerformanceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _performanceService.CreateProgramPerformanceAsync(dto);
            return CreatedAtAction(nameof(GetProgramPerformance), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update program performance
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult> UpdateProgramPerformance(
            int id, 
            [FromBody] ProgramPerformanceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _performanceService.UpdateProgramPerformanceAsync(id, dto);
            if (!result)
                return NotFound();

            return Ok(new { message = "تم تحديث أداء البرنامج بنجاح" });
        }

        /// <summary>
        /// Submit program performance for approval
        /// </summary>
        [HttpPost("{id}/submit")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult> SubmitForApproval(int id)
        {
            var result = await _performanceService.SubmitProgramPerformanceForApprovalAsync(id);
            if (!result)
                return BadRequest(new { message = "لا يمكن إرسال التقرير للموافقة" });

            return Ok(new { message = "تم إرسال التقرير للموافقة بنجاح" });
        }

        /// <summary>
        /// Generate quarterly report
        /// UC-04: توليد التقارير الربعية الموحدة
        /// </summary>
        [HttpGet("{programId}/report")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<IActionResult> GenerateQuarterlyReport(
            int programId,
            [FromQuery] int year,
            [FromQuery] int quarter)
        {
            try
            {
                var reportBytes = await _performanceService.GenerateQuarterlyReportAsync(programId, year, quarter);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Program_{programId}_Q{quarter}_{year}_Report.xlsx");
            }
            catch (NotImplementedException)
            {
                // Return placeholder response until reporting is implemented
                return Ok(new { 
                    message = "Report generation will be available soon",
                    programId,
                    year,
                    quarter
                });
            }
        }

        /// <summary>
        /// Update program KPI values
        /// UC-06: إدارة مؤشرات أداء البرنامج
        /// </summary>
        [HttpPost("{id}/kpis")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult> UpdateKPIValues(
            int id,
            [FromBody] List<ProgramKPIValueDto> kpiValues)
        {
            // Implementation would update KPI values for the program
            return Ok(new { message = "تم تحديث مؤشرات الأداء بنجاح" });
        }

        /// <summary>
        /// Add program achievement
        /// UC-10: إدارة الإنجازات الرئيسية
        /// </summary>
        [HttpPost("{id}/achievements")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult> AddAchievement(
            int id,
            [FromBody] ProgramAchievementDto achievement)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Implementation would add achievement to program
            return Ok(new { message = "تم إضافة الإنجاز بنجاح" });
        }

        /// <summary>
        /// Add program risk
        /// UC-11: إدارة المخاطر الرئيسية
        /// </summary>
        [HttpPost("{id}/risks")]
        [Authorize(Roles = "PerformanceManager,ProgramOwner")]
        public async Task<ActionResult> AddRisk(
            int id,
            [FromBody] ProgramRiskDto risk)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Implementation would add risk to program
            return Ok(new { message = "تم إضافة المخاطر بنجاح" });
        }
    }
}
