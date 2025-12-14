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
    /// Initiative management API controller
    /// Uses EXISTING Initiative entity from Domain.Entities.Core
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InitiativeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InitiativeController> _logger;

        public InitiativeController(IUnitOfWork unitOfWork, ILogger<InitiativeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all initiatives with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Initiative>>> GetInitiatives(
            [FromQuery] int? programId = null, 
            [FromQuery] string status = null)
        {
            try
            {
                var repo = _unitOfWork.Repository<Initiative>();
                
                // Build predicate
                Expression<Func<Initiative, bool>> predicate = i => true;
                
                if (programId.HasValue)
                {
                    predicate = i => i.ProgramId == programId.Value;
                }
                
                if (!string.IsNullOrEmpty(status))
                {
                    predicate = i => i.Status == status;
                }

                var initiatives = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderBy(i => i.Priority).ThenBy(i => i.NameEn),
                    includes: new List<Expression<Func<Initiative, object>>>
                    {
                        i => i.Program
                    },
                    disableTracking: true
                );

                _logger.LogInformation($"Retrieved {initiatives.Count} initiatives");
                return Ok(initiatives);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving initiatives");
                return StatusCode(500, new { error = "An error occurred while retrieving initiatives" });
            }
        }

        /// <summary>
        /// Get initiative by ID with related data
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Initiative>> GetInitiative(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<Initiative>();
                var initiatives = await repo.GetAsync(
                    predicate: i => i.Id == id,
                    includes: new List<Expression<Func<Initiative, object>>>
                    {
                        i => i.Program,
                        i => i.Milestones,
                        i => i.KPIs,
                        i => i.Risks
                    },
                    disableTracking: false
                );

                var initiative = initiatives.FirstOrDefault();
                if (initiative == null)
                {
                    return NotFound(new { error = $"Initiative with ID {id} not found" });
                }

                return Ok(initiative);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving initiative {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the initiative" });
            }
        }

        /// <summary>
        /// Create new initiative
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Initiative>> CreateInitiative([FromBody] Initiative initiative)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repo = _unitOfWork.Repository<Initiative>();
                
                // Set audit fields
                initiative.CreatedOn = DateTime.UtcNow;
                initiative.CreatedBy = User.Identity?.Name ?? "System";
                initiative.Status = string.IsNullOrEmpty(initiative.Status) ? "NotStarted" : initiative.Status;
                initiative.ProgressPercentage = 0;

                // Validate program exists
                var programRepo = _unitOfWork.Repository<VisionProgram>();
                var programExists = await programRepo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == initiative.ProgramId && p.IsDeleted == false
                );
                
                if (programExists == null)
                {
                    return BadRequest(new { error = "Invalid Program ID" });
                }

                await repo.InsertAsync(initiative, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created new initiative: {initiative.NameEn} (ID: {initiative.Id})");

                return CreatedAtAction(nameof(GetInitiative), new { id = initiative.Id }, initiative);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating initiative");
                return StatusCode(500, new { error = "An error occurred while creating the initiative" });
            }
        }

        /// <summary>
        /// Update existing initiative
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInitiative(int id, [FromBody] Initiative initiative)
        {
            try
            {
                if (id != initiative.Id)
                {
                    return BadRequest(new { error = "ID mismatch" });
                }

                var repo = _unitOfWork.Repository<Initiative>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: i => i.Id == id
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"Initiative with ID {id} not found" });
                }

                // Update fields
                existing.Code = initiative.Code;
                existing.NameAr = initiative.NameAr;
                existing.NameEn = initiative.NameEn;
                existing.DescriptionAr = initiative.DescriptionAr;
                existing.DescriptionEn = initiative.DescriptionEn;
                existing.Status = initiative.Status;
                existing.PlannedStartDate = initiative.PlannedStartDate;
                existing.PlannedEndDate = initiative.PlannedEndDate;
                existing.ActualStartDate = initiative.ActualStartDate;
                existing.ActualEndDate = initiative.ActualEndDate;
                existing.Budget = initiative.Budget;
                existing.ActualCost = initiative.ActualCost;
                existing.ProgressPercentage = initiative.ProgressPercentage;
                existing.Owner = initiative.Owner;
                existing.Priority = initiative.Priority;
                existing.Scope = initiative.Scope;
                existing.ExpectedOutcome = initiative.ExpectedOutcome;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated initiative: {initiative.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating initiative {id}");
                return StatusCode(500, new { error = "An error occurred while updating the initiative" });
            }
        }

        /// <summary>
        /// Delete initiative (soft delete if supported)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInitiative(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<Initiative>();
                var initiative = await repo.GetFirstOrDefaultAsync(
                    predicate: i => i.Id == id,
                    includes: new List<Expression<Func<Initiative, object>>>
                    {
                        i => i.Milestones,
                        i => i.KPIs
                    }
                );

                if (initiative == null)
                {
                    return NotFound(new { error = $"Initiative with ID {id} not found" });
                }

                // Check for dependencies
                if (initiative.Milestones.Any() || initiative.KPIs.Any())
                {
                    return BadRequest(new { error = "Cannot delete initiative with active milestones or KPIs" });
                }

                // Mark as deleted/cancelled
                initiative.Status = "Cancelled";
                initiative.UpdatedOn = DateTime.UtcNow;
                initiative.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(initiative);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Deleted initiative: {initiative.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting initiative {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the initiative" });
            }
        }

        /// <summary>
        /// Get milestones for an initiative
        /// </summary>
        [HttpGet("{id}/milestones")]
        public async Task<ActionResult<IEnumerable<InitiativeMilestone>>> GetInitiativeMilestones(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestones = await repo.GetAsync(
                    predicate: m => m.InitiativeId == id,
                    orderBy: q => q.OrderBy(m => m.PlannedDate),
                    disableTracking: true
                );

                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving milestones for initiative {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving milestones" });
            }
        }

        /// <summary>
        /// Create milestone for an initiative
        /// </summary>
        [HttpPost("{id}/milestones")]
        public async Task<ActionResult<InitiativeMilestone>> CreateMilestone(
            int id, 
            [FromBody] InitiativeMilestone milestone)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                milestone.InitiativeId = id;
                milestone.CreatedOn = DateTime.UtcNow;
                milestone.CreatedBy = User.Identity?.Name ?? "System";

                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                await repo.InsertAsync(milestone, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created milestone for initiative {id}");

                return Ok(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating milestone for initiative {id}");
                return StatusCode(500, new { error = "An error occurred while creating the milestone" });
            }
        }

        /// <summary>
        /// Get KPIs for an initiative
        /// </summary>
        [HttpGet("{id}/kpis")]
        public async Task<ActionResult<IEnumerable<KPI>>> GetInitiativeKPIs(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                var kpis = await repo.GetAsync(
                    predicate: k => k.InitiativeId == id && k.IsActive,
                    orderBy: q => q.OrderBy(k => k.NameEn),
                    disableTracking: true
                );

                return Ok(kpis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving KPIs for initiative {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving KPIs" });
            }
        }

        /// <summary>
        /// Get risks for an initiative
        /// </summary>
        [HttpGet("{id}/risks")]
        public async Task<ActionResult<IEnumerable<InitiativeRisk>>> GetInitiativeRisks(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeRisk>();
                var risks = await repo.GetAsync(
                    predicate: r => r.InitiativeId == id,
                    orderBy: q => q.OrderBy(r => r.Impact).ThenBy(r => r.Probability),
                    disableTracking: true
                );

                return Ok(risks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving risks for initiative {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving risks" });
            }
        }

        /// <summary>
        /// Update initiative progress
        /// </summary>
        [HttpPatch("{id}/progress")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] int progressPercentage)
        {
            try
            {
                if (progressPercentage < 0 || progressPercentage > 100)
                {
                    return BadRequest(new { error = "Progress must be between 0 and 100" });
                }

                var repo = _unitOfWork.Repository<Initiative>();
                var initiative = await repo.GetFirstOrDefaultAsync(
                    predicate: i => i.Id == id
                );

                if (initiative == null)
                {
                    return NotFound(new { error = $"Initiative with ID {id} not found" });
                }

                initiative.ProgressPercentage = progressPercentage;
                
                // Auto-update status based on progress
                if (progressPercentage == 0 && initiative.Status == "InProgress")
                {
                    initiative.Status = "NotStarted";
                }
                else if (progressPercentage > 0 && progressPercentage < 100)
                {
                    initiative.Status = "InProgress";
                }
                else if (progressPercentage == 100)
                {
                    initiative.Status = "Completed";
                    initiative.ActualEndDate = DateTime.UtcNow;
                }

                initiative.UpdatedOn = DateTime.UtcNow;
                initiative.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(initiative);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated progress for initiative {id}: {progressPercentage}%");

                return Ok(new { 
                    message = "Progress updated successfully", 
                    progress = progressPercentage,
                    status = initiative.Status 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating progress for initiative {id}");
                return StatusCode(500, new { error = "An error occurred while updating progress" });
            }
        }
    }
}


