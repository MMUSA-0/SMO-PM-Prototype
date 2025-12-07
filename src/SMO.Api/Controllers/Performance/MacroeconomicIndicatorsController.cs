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
    public class MacroeconomicIndicatorsController : ControllerBase
    {
        private readonly IPerformanceManagementService _performanceService;

        public MacroeconomicIndicatorsController(IPerformanceManagementService performanceService)
        {
            _performanceService = performanceService;
        }

        /// <summary>
        /// Get all macroeconomic indicators
        /// UC-01: إدارة مؤشرات الاقتصاد الكلي
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "PerformanceManager,Executive,DataEntry")]
        public async Task<ActionResult<List<MacroeconomicIndicatorDto>>> GetMacroeconomicIndicators()
        {
            var indicators = await _performanceService.GetMacroeconomicIndicatorsAsync();
            return Ok(indicators);
        }

        /// <summary>
        /// Get macroeconomic indicator by ID with historical values
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "PerformanceManager,Executive,DataEntry")]
        public async Task<ActionResult<MacroeconomicIndicatorDto>> GetMacroeconomicIndicator(int id)
        {
            var indicator = await _performanceService.GetMacroeconomicIndicatorByIdAsync(id);
            if (indicator == null)
                return NotFound();

            return Ok(indicator);
        }

        /// <summary>
        /// Update MEI actual/forecast values
        /// </summary>
        [HttpPost("{id}/values")]
        [Authorize(Roles = "PerformanceManager,DataEntry")]
        public async Task<ActionResult> UpdateMEIValue(int id, [FromBody] UpdateMEIValueRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.IndicatorId = id;
            var result = await _performanceService.UpdateMEIValueAsync(request);
            
            if (!result)
                return NotFound();

            return Ok(new { message = "تم تحديث القيمة بنجاح" });
        }

        /// <summary>
        /// Calculate performance score for a given value
        /// </summary>
        [HttpPost("{id}/calculate")]
        [Authorize(Roles = "PerformanceManager")]
        public async Task<ActionResult<decimal>> CalculatePerformance(int id, [FromBody] decimal actualValue)
        {
            var score = await _performanceService.CalculatePerformanceScoreAsync(id, actualValue);
            return Ok(new { performanceScore = score });
        }

        /// <summary>
        /// Export MEI data to Excel
        /// </summary>
        [HttpGet("export")]
        [Authorize(Roles = "PerformanceManager,Executive")]
        public async Task<IActionResult> ExportToExcel()
        {
            // Implementation would use EPPlus to generate Excel file
            var indicators = await _performanceService.GetMacroeconomicIndicatorsAsync();
            
            // For now, return JSON (Excel generation to be implemented)
            return Ok(indicators);
        }
    }
}
