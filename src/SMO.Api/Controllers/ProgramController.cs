using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Core;  // Using EXISTING entities
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// Vision Program management API controller
    /// Uses EXISTING VisionProgram entity from Domain.Entities.Core
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProgramController> _logger;

        public ProgramController(IUnitOfWork unitOfWork, ILogger<ProgramController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all vision programs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisionProgram>>> GetPrograms()
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var programs = await repo.GetAsync(
                    predicate: p => p.IsDeleted == false,
                    orderBy: q => q.OrderBy(p => p.NameEn),
                    disableTracking: true
                );

                _logger.LogInformation($"Retrieved {programs.Count} programs");
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs");
                return StatusCode(500, new { error = "An error occurred while retrieving programs" });
            }
        }

        /// <summary>
        /// Get program by ID with initiatives
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VisionProgram>> GetProgram(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var programs = await repo.GetAsync(
                    predicate: p => p.Id == id && p.IsDeleted == false,
                    includes: new List<System.Linq.Expressions.Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives,
                        p => p.KPIs
                    },
                    disableTracking: false
                );

                var program = programs.FirstOrDefault();
                if (program == null)
                {
                    return NotFound(new { error = $"Program with ID {id} not found" });
                }

                return Ok(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the program" });
            }
        }

        /// <summary>
        /// Create new vision program
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VisionProgram>> CreateProgram([FromBody] VisionProgram program)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repo = _unitOfWork.Repository<VisionProgram>();
                
                // Set audit fields (handled by BaseDbContext but setting explicitly)
                program.CreatedOn = DateTime.UtcNow;
                program.CreatedBy = User.Identity?.Name ?? "System";
                program.IsDeleted = false;
                program.Status = string.IsNullOrEmpty(program.Status) ? "NotStarted" : program.Status;

                await repo.InsertAsync(program, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created new program: {program.NameEn} (ID: {program.Id})");

                return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating program");
                return StatusCode(500, new { error = "An error occurred while creating the program" });
            }
        }

        /// <summary>
        /// Update existing program
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] VisionProgram program)
        {
            try
            {
                if (id != program.Id)
                {
                    return BadRequest(new { error = "ID mismatch" });
                }

                var repo = _unitOfWork.Repository<VisionProgram>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.IsDeleted == false
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"Program with ID {id} not found" });
                }

                // Update fields
                existing.Code = program.Code;
                existing.NameAr = program.NameAr;
                existing.NameEn = program.NameEn;
                existing.DescriptionAr = program.DescriptionAr;
                existing.DescriptionEn = program.DescriptionEn;
                existing.Status = program.Status;
                existing.StartDate = program.StartDate;
                existing.EndDate = program.EndDate;
                existing.TotalBudget = program.TotalBudget;
                existing.ProgramOwner = program.ProgramOwner;
                existing.MinistryAr = program.MinistryAr;
                existing.MinistryEn = program.MinistryEn;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated program: {program.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating program {id}");
                return StatusCode(500, new { error = "An error occurred while updating the program" });
            }
        }

        /// <summary>
        /// Delete program (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var program = await repo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.IsDeleted == false,
                    includes: new List<System.Linq.Expressions.Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives
                    }
                );

                if (program == null)
                {
                    return NotFound(new { error = $"Program with ID {id} not found" });
                }

                // Check if program has active initiatives
                if (program.Initiatives.Any(i => i.Status != "Completed" && i.Status != "Cancelled"))
                {
                    return BadRequest(new { error = "Cannot delete program with active initiatives" });
                }

                // Soft delete
                program.IsDeleted = true;
                program.UpdatedOn = DateTime.UtcNow;
                program.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(program);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Deleted program: {program.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting program {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the program" });
            }
        }

        /// <summary>
        /// Get initiatives for a program
        /// </summary>
        [HttpGet("{id}/initiatives")]
        public async Task<ActionResult<IEnumerable<Initiative>>> GetProgramInitiatives(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<Initiative>();
                var initiatives = await repo.GetAsync(
                    predicate: i => i.ProgramId == id,
                    orderBy: q => q.OrderBy(i => i.NameEn),
                    disableTracking: true
                );

                return Ok(initiatives);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving initiatives for program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving initiatives" });
            }
        }

        /// <summary>
        /// Get KPIs for a program
        /// </summary>
        [HttpGet("{id}/kpis")]
        public async Task<ActionResult<IEnumerable<KPI>>> GetProgramKPIs(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<KPI>();
                var kpis = await repo.GetAsync(
                    predicate: k => k.ProgramId == id && k.IsActive,
                    orderBy: q => q.OrderBy(k => k.NameEn),
                    disableTracking: true
                );

                return Ok(kpis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving KPIs for program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving KPIs" });
            }
        }
    }
}
