using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Core;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// KPI management API controller
    /// Uses EXISTING KPI entity from Domain.Entities.Core
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class KPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<KPIController> _logger;

        public KPIController(IUnitOfWork unitOfWork, ILogger<KPIController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all KPIs with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KPI>>> GetKPIs(
            [FromQuery] int? programId = null,
            [FromQuery] int? initiativeId = null,
            [FromQuery] string category = null)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                
                // Build predicate
                Expression<Func<KPI, bool>> predicate = k => k.IsActive;
                
                var kpis = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderBy(k => k.Category).ThenBy(k => k.NameEn),
                    includes: new List<Expression<Func<KPI, object>>>
                    {
                        k => k.Program,
                        k => k.Initiative
                    },
                    disableTracking: true
                );

                // Apply additional filters
                if (programId.HasValue)
                {
                    kpis = kpis.Where(k => k.ProgramId == programId.Value).ToList();
                }
                
                if (initiativeId.HasValue)
                {
                    kpis = kpis.Where(k => k.InitiativeId == initiativeId.Value).ToList();
                }
                
                if (!string.IsNullOrEmpty(category))
                {
                    kpis = kpis.Where(k => k.Category == category).ToList();
                }

                _logger.LogInformation($"Retrieved {kpis.Count} KPIs");
                return Ok(kpis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPIs");
                return StatusCode(500, new { error = "An error occurred while retrieving KPIs" });
            }
        }

        /// <summary>
        /// Get KPI by ID with related data
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<KPI>> GetKPI(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                var kpis = await repo.GetAsync(
                    predicate: k => k.Id == id && k.IsActive,
                    includes: new List<Expression<Func<KPI, object>>>
                    {
                        k => k.Program,
                        k => k.Initiative,
                        k => k.Values,
                        k => k.Targets
                    },
                    disableTracking: false
                );

                var kpi = kpis.FirstOrDefault();
                if (kpi == null)
                {
                    return NotFound(new { error = $"KPI with ID {id} not found" });
                }

                // Calculate current achievement
                if (kpi.Values != null && kpi.Values.Any())
                {
                    var latestValue = kpi.Values.OrderByDescending(v => v.PeriodDate).FirstOrDefault();
                    if (latestValue != null && kpi.Target2030.HasValue)
                    {
                        var achievement = (latestValue.ActualValue / kpi.Target2030.Value) * 100;
                        // Add achievement to response
                    }
                }

                return Ok(kpi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving KPI {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the KPI" });
            }
        }

        /// <summary>
        /// Create new KPI
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<KPI>> CreateKPI([FromBody] KPI kpi)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repo = _unitOfWork.Repository<KPI>();
                
                // Set audit fields
                kpi.CreatedOn = DateTime.UtcNow;
                kpi.CreatedBy = User.Identity?.Name ?? "System";
                kpi.IsActive = true;

                // Generate code if not provided
                if (string.IsNullOrEmpty(kpi.Code))
                {
                    var prefix = kpi.InitiativeId.HasValue ? "INIT" : "PRG";
                    var count = (await repo.GetAsync(predicate: k => k.IsActive)).Count;
                    kpi.Code = $"{prefix}.KPI.{count + 1:D3}";
                }

                await repo.InsertAsync(kpi, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created new KPI: {kpi.NameEn} (ID: {kpi.Id})");

                return CreatedAtAction(nameof(GetKPI), new { id = kpi.Id }, kpi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating KPI");
                return StatusCode(500, new { error = "An error occurred while creating the KPI" });
            }
        }

        /// <summary>
        /// Update existing KPI
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKPI(int id, [FromBody] KPI kpi)
        {
            try
            {
                if (id != kpi.Id)
                {
                    return BadRequest(new { error = "ID mismatch" });
                }

                var repo = _unitOfWork.Repository<KPI>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: k => k.Id == id && k.IsActive
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"KPI with ID {id} not found" });
                }

                // Update fields
                existing.Code = kpi.Code;
                existing.NameAr = kpi.NameAr;
                existing.NameEn = kpi.NameEn;
                existing.DescriptionAr = kpi.DescriptionAr;
                existing.DescriptionEn = kpi.DescriptionEn;
                existing.Category = kpi.Category;
                existing.Type = kpi.Type;
                existing.Unit = kpi.Unit;
                existing.MeasurementFrequency = kpi.MeasurementFrequency;
                existing.CalculationMethod = kpi.CalculationMethod;
                existing.DataSource = kpi.DataSource;
                existing.Owner = kpi.Owner;
                existing.Polarity = kpi.Polarity;
                existing.BaselineValue = kpi.BaselineValue;
                existing.BaselineDate = kpi.BaselineDate;
                existing.Target2030 = kpi.Target2030;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated KPI: {kpi.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating KPI {id}");
                return StatusCode(500, new { error = "An error occurred while updating the KPI" });
            }
        }

        /// <summary>
        /// Delete KPI (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKPI(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                var kpi = await repo.GetFirstOrDefaultAsync(
                    predicate: k => k.Id == id && k.IsActive,
                    includes: new List<Expression<Func<KPI, object>>>
                    {
                        k => k.Values,
                        k => k.Targets
                    }
                );

                if (kpi == null)
                {
                    return NotFound(new { error = $"KPI with ID {id} not found" });
                }

                // Soft delete
                kpi.IsActive = false;
                kpi.UpdatedOn = DateTime.UtcNow;
                kpi.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(kpi);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Deleted KPI: {kpi.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting KPI {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the KPI" });
            }
        }

        /// <summary>
        /// Get KPI values (historical data)
        /// </summary>
        [HttpGet("{id}/values")]
        public async Task<ActionResult<IEnumerable<KPIValue>>> GetKPIValues(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPIValue>();
                var values = await repo.GetAsync(
                    predicate: v => v.KPIId == id,
                    orderBy: q => q.OrderByDescending(v => v.PeriodDate),
                    disableTracking: true
                );

                return Ok(values);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving values for KPI {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving KPI values" });
            }
        }

        /// <summary>
        /// Add KPI value
        /// </summary>
        [HttpPost("{id}/values")]
        public async Task<ActionResult<KPIValue>> AddKPIValue(int id, [FromBody] KPIValue value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify KPI exists
                var kpiRepo = _unitOfWork.Repository<KPI>();
                var kpi = await kpiRepo.GetFirstOrDefaultAsync(
                    predicate: k => k.Id == id && k.IsActive
                );

                if (kpi == null)
                {
                    return NotFound(new { error = $"KPI with ID {id} not found" });
                }

                value.KPIId = id;
                value.CreatedOn = DateTime.UtcNow;
                value.CreatedBy = User.Identity?.Name ?? "System";

                // Calculate achievement percentage if target exists
                if (kpi.Target2030.HasValue && kpi.Target2030.Value > 0)
                {
                    value.AchievementPercentage = (value.ActualValue / kpi.Target2030.Value) * 100;
                }

                var repo = _unitOfWork.Repository<KPIValue>();
                await repo.InsertAsync(value, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Added value for KPI {id}: {value.ActualValue}");

                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding value for KPI {id}");
                return StatusCode(500, new { error = "An error occurred while adding the KPI value" });
            }
        }

        /// <summary>
        /// Get KPI targets
        /// </summary>
        [HttpGet("{id}/targets")]
        public async Task<ActionResult<IEnumerable<KPITarget>>> GetKPITargets(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPITarget>();
                var targets = await repo.GetAsync(
                    predicate: t => t.KPIId == id,
                    orderBy: q => q.OrderBy(t => t.Year),
                    disableTracking: true
                );

                return Ok(targets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving targets for KPI {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving KPI targets" });
            }
        }

        /// <summary>
        /// Set KPI target
        /// </summary>
        [HttpPost("{id}/targets")]
        public async Task<ActionResult<KPITarget>> SetKPITarget(int id, [FromBody] KPITarget target)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                target.KPIId = id;
                target.CreatedOn = DateTime.UtcNow;
                target.CreatedBy = User.Identity?.Name ?? "System";

                var repo = _unitOfWork.Repository<KPITarget>();
                
                // Check if target for this year already exists
                var existingTarget = await repo.GetFirstOrDefaultAsync(
                    predicate: t => t.KPIId == id && t.Year == target.Year
                );

                if (existingTarget != null)
                {
                    // Update existing target
                    existingTarget.TargetValue = target.TargetValue;
                    existingTarget.UpdatedOn = DateTime.UtcNow;
                    existingTarget.UpdatedBy = User.Identity?.Name ?? "System";
                    
                    repo.Update(existingTarget);
                    await _unitOfWork.SaveChangesAsync();
                    
                    _logger.LogInformation($"Updated target for KPI {id}, Year {target.Year}");
                    return Ok(existingTarget);
                }
                else
                {
                    // Create new target
                    await repo.InsertAsync(target, autoSave: false);
                    await _unitOfWork.SaveChangesAsync();
                    
                    _logger.LogInformation($"Created target for KPI {id}, Year {target.Year}");
                    return Ok(target);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting target for KPI {id}");
                return StatusCode(500, new { error = "An error occurred while setting the KPI target" });
            }
        }

        /// <summary>
        /// Get KPI statistics (dashboard data)
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetKPIStatistics()
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                var allKPIs = await repo.GetAsync(
                    predicate: k => k.IsActive,
                    includes: new List<Expression<Func<KPI, object>>>
                    {
                        k => k.Values
                    },
                    disableTracking: true
                );

                var statistics = new
                {
                    total = allKPIs.Count,
                    byCategory = allKPIs.GroupBy(k => k.Category).Select(g => new { category = g.Key, count = g.Count() }),
                    byType = allKPIs.GroupBy(k => k.Type).Select(g => new { type = g.Key, count = g.Count() }),
                    byFrequency = allKPIs.GroupBy(k => k.MeasurementFrequency).Select(g => new { frequency = g.Key, count = g.Count() }),
                    withValues = allKPIs.Count(k => k.Values != null && k.Values.Any()),
                    withoutValues = allKPIs.Count(k => k.Values == null || !k.Values.Any())
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KPI statistics");
                return StatusCode(500, new { error = "An error occurred while retrieving KPI statistics" });
            }
        }
    }
}


