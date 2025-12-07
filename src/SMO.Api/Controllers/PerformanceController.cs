using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMO.Application.Features.Performance.Services;
using SMO.Application.Features.Performance.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// Performance Management API Controller
    /// Aggregates data from multiple modules for performance views
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceAggregatorService _aggregatorService;

        public PerformanceController(IPerformanceAggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }

        /// <summary>
        /// UC-01: Get Executive Performance Dashboard
        /// Aggregates data from all modules for executive view
        /// </summary>
        [HttpGet("dashboard/executive")]
        public async Task<IActionResult> GetExecutiveDashboard([FromQuery] int? year, [FromQuery] int? quarter)
        {
            try
            {
                var dashboard = await _aggregatorService.GetExecutiveDashboardAsync(year, quarter);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load dashboard", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-01: Get Macroeconomic Indicators
        /// </summary>
        [HttpGet("mei")]
        public async Task<IActionResult> GetMacroeconomicIndicators()
        {
            try
            {
                var indicators = await _aggregatorService.GetMacroeconomicIndicatorsAsync();
                return Ok(indicators);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load MEI", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-01: Update MEI value (BR-09)
        /// </summary>
        [HttpPut("mei/{id}")]
        [Authorize(Roles = "Admin,PerformanceManager")]
        public async Task<IActionResult> UpdateMEI(int id, [FromBody] UpdateMEIDto dto)
        {
            try
            {
                // BR-10: Validate update frequency
                var result = await _aggregatorService.UpdateMEIAsync(id, dto);
                if (!result.Success)
                {
                    return BadRequest(new { error = result.Message });
                }
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_MEI", $"MEI {id} updated", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update MEI", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-02: Get Strategic Objectives
        /// </summary>
        [HttpGet("strategic-objectives")]
        public async Task<IActionResult> GetStrategicObjectives([FromQuery] int? level)
        {
            try
            {
                var objectives = await _aggregatorService.GetStrategicObjectivesAsync(level);
                return Ok(objectives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load objectives", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-03: Get Programs List
        /// </summary>
        [HttpGet("programs")]
        public async Task<IActionResult> GetPrograms([FromQuery] string status = null)
        {
            try
            {
                var programs = await _aggregatorService.GetProgramsAsync(status);
                return Ok(programs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load programs", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-04: Generate Quarterly Report (BR-43 to BR-48)
        /// </summary>
        [HttpPost("programs/{programId}/quarterly-report")]
        [Authorize(Roles = "ProgramManager,Admin")]
        public async Task<IActionResult> GenerateQuarterlyReport(int programId, [FromBody] GenerateReportDto dto)
        {
            try
            {
                // BR-43: Validate all sections included
                // BR-44: Check data accuracy
                // BR-45: Validate KPI calculations
                // BR-46: Include achievements
                // BR-47: Include risks
                // BR-48: Include support requests
                
                var report = await _aggregatorService.GenerateQuarterlyReportAsync(programId, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("GENERATE_REPORT", $"Quarterly report for program {programId}", dto);
                
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to generate report", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-05 to UC-13: Program Data Entry Wizard
        /// </summary>
        [HttpPost("programs/{programId}/wizard-data")]
        [Authorize(Roles = "ProgramManager,Admin")]
        public async Task<IActionResult> SaveProgramWizardData(int programId, [FromBody] ProgramWizardDto dto)
        {
            try
            {
                // Validate each step data based on BR-17 to BR-42
                var validationResult = ValidateProgramWizardData(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors });
                }

                var result = await _aggregatorService.SaveProgramWizardDataAsync(programId, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_PROGRAM", $"Program {programId} wizard data saved", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to save program data", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-14: Get Initiatives List
        /// </summary>
        [HttpGet("initiatives")]
        public async Task<IActionResult> GetInitiatives(
            [FromQuery] int? programId,
            [FromQuery] string status,
            [FromQuery] bool? isPivotal)
        {
            try
            {
                var initiatives = await _aggregatorService.GetInitiativesAsync(programId, status, isPivotal);
                return Ok(initiatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load initiatives", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-15: Get Initiative Details
        /// </summary>
        [HttpGet("initiatives/{id}")]
        public async Task<IActionResult> GetInitiativeDetails(int id)
        {
            try
            {
                var initiative = await _aggregatorService.GetInitiativeDetailsAsync(id);
                if (initiative == null)
                {
                    return NotFound(new { error = "Initiative not found" });
                }
                return Ok(initiative);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load initiative", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-16: Update Initiative Progress (BR-49 to BR-51)
        /// </summary>
        [HttpPut("initiatives/{id}/progress")]
        [Authorize(Roles = "InitiativeManager,ProgramManager,Admin")]
        public async Task<IActionResult> UpdateInitiativeProgress(int id, [FromBody] UpdateProgressDto dto)
        {
            try
            {
                // BR-49: Validate progress percentage
                if (dto.Progress < 0 || dto.Progress > 100)
                {
                    return BadRequest(new { error = "Progress must be between 0 and 100" });
                }

                // BR-50: Calculate based on milestones
                // BR-51: Auto-update status
                var result = await _aggregatorService.UpdateInitiativeProgressAsync(id, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_PROGRESS", $"Initiative {id} progress updated to {dto.Progress}%", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update progress", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-17: Update Milestones (BR-52 to BR-54)
        /// </summary>
        [HttpPut("initiatives/{initiativeId}/milestones/{milestoneId}")]
        [Authorize(Roles = "InitiativeManager,ProgramManager,Admin")]
        public async Task<IActionResult> UpdateMilestone(int initiativeId, int milestoneId, [FromBody] UpdateMilestoneDto dto)
        {
            try
            {
                // BR-52: Track planned vs actual dates
                // BR-53: Update status
                // BR-54: Link to deliverables
                var result = await _aggregatorService.UpdateMilestoneAsync(initiativeId, milestoneId, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_MILESTONE", $"Milestone {milestoneId} updated", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update milestone", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-18: Update KPIs (BR-55 to BR-57)
        /// </summary>
        [HttpPut("kpis/{id}/value")]
        [Authorize(Roles = "KPIManager,Admin")]
        public async Task<IActionResult> UpdateKPIValue(int id, [FromBody] UpdateKPIValueDto dto)
        {
            try
            {
                // BR-55: Actual vs forecast values
                // BR-56: Quarterly/annual updates
                // BR-57: Auto-calculate variances
                var result = await _aggregatorService.UpdateKPIValueAsync(id, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_KPI", $"KPI {id} value updated", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update KPI", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-21: Add Achievement (BR-58, BR-59)
        /// </summary>
        [HttpPost("programs/{programId}/achievements")]
        [Authorize(Roles = "ProgramManager,Admin")]
        public async Task<IActionResult> AddAchievement(int programId, [FromBody] AddAchievementDto dto)
        {
            try
            {
                // BR-58: Track major achievements
                // BR-59: Link to KPIs/initiatives
                var result = await _aggregatorService.AddAchievementAsync(programId, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("ADD_ACHIEVEMENT", $"Achievement added to program {programId}", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to add achievement", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-22: Request Support (BR-89, BR-90)
        /// </summary>
        [HttpPost("support-requests")]
        [Authorize]
        public async Task<IActionResult> CreateSupportRequest([FromBody] CreateSupportRequestDto dto)
        {
            try
            {
                // BR-89: Multiple support types
                // BR-90: Prioritization system
                var request = await _aggregatorService.CreateSupportRequestAsync(dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("CREATE_SUPPORT_REQUEST", $"Support request created", dto);
                
                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to create support request", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-23: Update Risks (BR-82 to BR-85)
        /// </summary>
        [HttpPut("risks/{id}")]
        [Authorize(Roles = "RiskManager,ProgramManager,Admin")]
        public async Task<IActionResult> UpdateRisk(int id, [FromBody] UpdateRiskDto dto)
        {
            try
            {
                // BR-82: Risk assessment matrix
                // BR-83: Mitigation plans
                // BR-84: Impact analysis
                // BR-85: Escalation procedures
                var result = await _aggregatorService.UpdateRiskAsync(id, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_RISK", $"Risk {id} updated", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update risk", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-24: Approval Workflows (BR-86 to BR-88)
        /// </summary>
        [HttpGet("approvals/pending")]
        [Authorize]
        public async Task<IActionResult> GetPendingApprovals()
        {
            try
            {
                var approvals = await _aggregatorService.GetPendingApprovalsAsync();
                return Ok(approvals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load approvals", details = ex.Message });
            }
        }

        [HttpPost("approvals/{id}/approve")]
        [Authorize(Roles = "Approver,Admin")]
        public async Task<IActionResult> ApproveRequest(int id, [FromBody] ApprovalDto dto)
        {
            try
            {
                // BR-86: Multi-level approval
                // BR-87: Comments and attachments
                // BR-88: Email notifications
                var result = await _aggregatorService.ApproveRequestAsync(id, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("APPROVE_REQUEST", $"Request {id} approved", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to approve request", details = ex.Message });
            }
        }

        /// <summary>
        /// UC-26: KPI Thresholds Configuration (BR-06 to BR-08)
        /// </summary>
        [HttpGet("thresholds")]
        [Authorize]
        public async Task<IActionResult> GetKPIThresholds([FromQuery] int? kpiId)
        {
            try
            {
                var thresholds = await _aggregatorService.GetKPIThresholdsAsync(kpiId);
                return Ok(thresholds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load thresholds", details = ex.Message });
            }
        }

        [HttpPut("thresholds/{id}")]
        [Authorize(Roles = "Admin,PerformanceManager")]
        public async Task<IActionResult> UpdateThreshold(int id, [FromBody] UpdateThresholdDto dto)
        {
            try
            {
                // BR-06: Three-level threshold (Green/Yellow/Red)
                // BR-07: Polarity configuration
                // BR-08: Customizable ranges
                var result = await _aggregatorService.UpdateThresholdAsync(id, dto);
                
                // BR-91: Audit trail
                await LogAuditAsync("UPDATE_THRESHOLD", $"Threshold {id} updated", dto);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to update threshold", details = ex.Message });
            }
        }

        /// <summary>
        /// Data Synchronization Status (BR-60 to BR-67)
        /// </summary>
        [HttpGet("sync/status")]
        [Authorize(Roles = "Admin,SystemMonitor")]
        public async Task<IActionResult> GetSyncStatus()
        {
            try
            {
                var status = await _aggregatorService.GetSyncStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get sync status", details = ex.Message });
            }
        }

        [HttpPost("sync/manual/{systemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TriggerManualSync(string systemId)
        {
            try
            {
                // BR-64: Real-time sync monitoring
                // BR-65: Error handling and retry
                // BR-66: Conflict resolution
                // BR-67: Performance optimization
                var result = await _aggregatorService.TriggerManualSyncAsync(systemId);
                
                // BR-91: Audit trail
                await LogAuditAsync("MANUAL_SYNC", $"Manual sync triggered for {systemId}", null);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to trigger sync", details = ex.Message });
            }
        }

        /// <summary>
        /// Audit Logs (BR-91 to BR-93)
        /// </summary>
        [HttpGet("audit-logs")]
        [Authorize(Roles = "Admin,Auditor")]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string action,
            [FromQuery] string user,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // BR-91: Track all critical operations
                // BR-92: User, timestamp, changes
                // BR-93: Export capability
                var logs = await _aggregatorService.GetAuditLogsAsync(from, to, action, user, page, pageSize);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to load audit logs", details = ex.Message });
            }
        }

        #region Helper Methods

        private ValidationResult ValidateProgramWizardData(ProgramWizardDto dto)
        {
            var result = new ValidationResult { IsValid = true, Errors = new List<string>() };

            // BR-17 to BR-42: Validate each step
            if (string.IsNullOrEmpty(dto.Overview?.Description))
            {
                result.Errors.Add("Program description is required");
                result.IsValid = false;
            }

            if (dto.KPIs == null || dto.KPIs.Count == 0)
            {
                result.Errors.Add("At least one KPI is required");
                result.IsValid = false;
            }

            // Additional validations...

            return result;
        }

        private async Task LogAuditAsync(string action, string description, object data)
        {
            // Implementation would use audit service
            // This is placeholder for BR-91 compliance
            await Task.CompletedTask;
        }

        #endregion

        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; }
        }
    }
}
