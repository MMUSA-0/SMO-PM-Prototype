using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Core;
using SMO.Domain.Entities.Risk;
using SMO.Domain.Entities.Financial;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// Vision Program Controller - Manages Vision 2030 Programs
    /// Uses EXISTING VisionProgram entity from Domain.Entities.Core
    /// FIXED version that properly uses VisionProgram entity
    /// </summary>
    [ApiController]
    [Route("api/vision-program")]
    public class VisionProgramController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VisionProgramController> _logger;

        public VisionProgramController(IUnitOfWork unitOfWork, ILogger<VisionProgramController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all vision programs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVisionPrograms(
            [FromQuery] string status = null,
            [FromQuery] string ministry = null)
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                
                Expression<Func<VisionProgram, bool>> predicate = p => !p.IsDeleted;
                
                if (!string.IsNullOrEmpty(status))
                {
                    predicate = p => !p.IsDeleted && p.Status == status;
                }
                
                if (!string.IsNullOrEmpty(ministry))
                {
                    predicate = p => !p.IsDeleted && (p.MinistryEn.Contains(ministry) || p.MinistryAr.Contains(ministry));
                }

                var programs = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderBy(p => p.StartDate).ThenBy(p => p.NameEn),
                    includes: new List<Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives,
                        p => p.KPIs
                    },
                    disableTracking: true
                );

                var programsWithStats = programs.Select(p => new
                {
                    id = p.Id,
                    code = p.Code,
                    nameAr = p.NameAr,
                    nameEn = p.NameEn,
                    descriptionAr = p.DescriptionAr,
                    descriptionEn = p.DescriptionEn,
                    visionAlignment = p.VisionAlignment,
                    startDate = p.StartDate,
                    endDate = p.EndDate,
                    status = p.Status,
                    programOwner = p.ProgramOwner,
                    ministryAr = p.MinistryAr,
                    ministryEn = p.MinistryEn,
                    totalBudget = p.TotalBudget,
                    logoUrl = p.LogoUrl,
                    initiativeCount = p.Initiatives?.Count(i => !i.IsDeleted) ?? 0,
                    kpiCount = p.KPIs?.Count(k => k.IsActive) ?? 0,
                    createdOn = p.CreatedOn,
                    createdBy = p.CreatedBy
                });

                _logger.LogInformation($"Retrieved {programs.Count} vision programs");
                return Ok(programsWithStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vision programs");
                return StatusCode(500, new { error = "An error occurred while retrieving vision programs" });
            }
        }

        /// <summary>
        /// Get vision program by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVisionProgram(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var programs = await repo.GetAsync(
                    predicate: p => p.Id == id && !p.IsDeleted,
                    includes: new List<Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives,
                        p => p.KPIs,
                        p => p.Risks,
                        p => p.Budgets
                    },
                    disableTracking: false
                );

                var program = programs.FirstOrDefault();
                if (program == null)
                {
                    _logger.LogWarning($"Vision program not found: {id}");
                    return NotFound(new { error = $"Vision program with ID {id} not found" });
                }

                // Calculate comprehensive statistics
                var programDetails = new
                {
                    program = new
                    {
                        id = program.Id,
                        code = program.Code,
                        nameAr = program.NameAr,
                        nameEn = program.NameEn,
                        descriptionAr = program.DescriptionAr,
                        descriptionEn = program.DescriptionEn,
                        visionAlignment = program.VisionAlignment,
                        startDate = program.StartDate,
                        endDate = program.EndDate,
                        status = program.Status,
                        programOwner = program.ProgramOwner,
                        ministryAr = program.MinistryAr,
                        ministryEn = program.MinistryEn,
                        totalBudget = program.TotalBudget,
                        logoUrl = program.LogoUrl
                    },
                    statistics = new
                    {
                        initiatives = new
                        {
                            total = program.Initiatives?.Count(i => !i.IsDeleted) ?? 0,
                            active = program.Initiatives?.Count(i => !i.IsDeleted && i.Status == "InProgress") ?? 0,
                            completed = program.Initiatives?.Count(i => !i.IsDeleted && i.Status == "Completed") ?? 0,
                            notStarted = program.Initiatives?.Count(i => !i.IsDeleted && i.Status == "NotStarted") ?? 0,
                            delayed = program.Initiatives?.Count(i => !i.IsDeleted && i.Status == "Delayed") ?? 0
                        },
                        kpis = new
                        {
                            total = program.KPIs?.Count(k => k.IsActive) ?? 0,
                            onTarget = program.KPIs?.Count(k => k.IsActive && k.Status == "OnTarget") ?? 0,
                            atRisk = program.KPIs?.Count(k => k.IsActive && k.Status == "AtRisk") ?? 0,
                            offTarget = program.KPIs?.Count(k => k.IsActive && k.Status == "OffTarget") ?? 0
                        },
                        risks = new
                        {
                            total = program.Risks?.Count(r => !r.IsArchived) ?? 0,
                            critical = program.Risks?.Count(r => !r.IsArchived && r.RiskLevel == "Critical") ?? 0,
                            high = program.Risks?.Count(r => !r.IsArchived && r.RiskLevel == "High") ?? 0,
                            medium = program.Risks?.Count(r => !r.IsArchived && r.RiskLevel == "Medium") ?? 0,
                            low = program.Risks?.Count(r => !r.IsArchived && r.RiskLevel == "Low") ?? 0
                        },
                        budget = new
                        {
                            total = program.TotalBudget ?? 0,
                            allocated = program.Budgets?.Sum(b => b.AllocatedAmount) ?? 0,
                            spent = program.Budgets?.Sum(b => b.SpentAmount) ?? 0,
                            remaining = (program.TotalBudget ?? 0) - (program.Budgets?.Sum(b => b.SpentAmount) ?? 0)
                        }
                    }
                };

                _logger.LogInformation($"Retrieved vision program: {program.NameEn}");
                return Ok(programDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vision program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the vision program" });
            }
        }

        /// <summary>
        /// Create new vision program
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VisionProgram>> CreateVisionProgram([FromBody] VisionProgram program)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repo = _unitOfWork.Repository<VisionProgram>();
                
                // Set audit fields
                program.CreatedOn = DateTime.UtcNow;
                program.CreatedBy = User.Identity?.Name ?? "System";
                program.Status = string.IsNullOrEmpty(program.Status) ? "NotStarted" : program.Status;
                
                // Initialize collections
                if (program.KPIs == null) program.KPIs = new HashSet<KPI>();
                if (program.Initiatives == null) program.Initiatives = new HashSet<Initiative>();
                if (program.Risks == null) program.Risks = new HashSet<ProgramRisk>();
                if (program.Budgets == null) program.Budgets = new HashSet<ProgramBudget>();

                await repo.InsertAsync(program, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created new vision program: {program.NameEn} (ID: {program.Id})");

                return CreatedAtAction(nameof(GetVisionProgram), new { id = program.Id }, program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vision program");
                return StatusCode(500, new { error = "An error occurred while creating the vision program" });
            }
        }

        /// <summary>
        /// Update existing vision program
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisionProgram(int id, [FromBody] VisionProgram program)
        {
            try
            {
                if (id != program.Id)
                {
                    return BadRequest(new { error = "ID mismatch" });
                }

                var repo = _unitOfWork.Repository<VisionProgram>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"Vision program with ID {id} not found" });
                }

                // Update fields
                existing.Code = program.Code;
                existing.NameAr = program.NameAr;
                existing.NameEn = program.NameEn;
                existing.DescriptionAr = program.DescriptionAr;
                existing.DescriptionEn = program.DescriptionEn;
                existing.VisionAlignment = program.VisionAlignment;
                existing.StartDate = program.StartDate;
                existing.EndDate = program.EndDate;
                existing.Status = program.Status;
                existing.ProgramOwner = program.ProgramOwner;
                existing.MinistryAr = program.MinistryAr;
                existing.MinistryEn = program.MinistryEn;
                existing.TotalBudget = program.TotalBudget;
                existing.LogoUrl = program.LogoUrl;
                existing.PillarId = program.PillarId;
                existing.StrategicObjectiveId = program.StrategicObjectiveId;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated vision program: {program.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vision program {id}");
                return StatusCode(500, new { error = "An error occurred while updating the vision program" });
            }
        }

        /// <summary>
        /// Delete vision program (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisionProgram(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var program = await repo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id,
                    includes: new List<Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives,
                        p => p.KPIs
                    }
                );

                if (program == null)
                {
                    return NotFound(new { error = $"Vision program with ID {id} not found" });
                }

                // Check for dependencies
                if (program.Initiatives.Any(i => !i.IsDeleted))
                {
                    return BadRequest(new { error = "Cannot delete program with active initiatives. Please deactivate all initiatives first." });
                }

                // Soft delete
                program.IsDeleted = true;
                program.Status = "Cancelled";
                program.UpdatedOn = DateTime.UtcNow;
                program.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(program);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Deleted vision program: {program.NameEn} (ID: {id})");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting vision program {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the vision program" });
            }
        }

        /// <summary>
        /// Get initiatives for a vision program
        /// </summary>
        [HttpGet("{id}/initiatives")]
        public async Task<ActionResult<IEnumerable<Initiative>>> GetProgramInitiatives(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<Initiative>();
                var initiatives = await repo.GetAsync(
                    predicate: i => i.ProgramId == id && !i.IsDeleted,
                    orderBy: q => q.OrderBy(i => i.Priority).ThenBy(i => i.NameEn),
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
        /// Get KPIs for a vision program
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
                    includes: new List<Expression<Func<KPI, object>>>
                    {
                        k => k.KPITargets,
                        k => k.KPIValues
                    },
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

        /// <summary>
        /// Get risks for a vision program
        /// </summary>
        [HttpGet("{id}/risks")]
        public async Task<ActionResult<IEnumerable<ProgramRisk>>> GetProgramRisks(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<ProgramRisk>();
                var risks = await repo.GetAsync(
                    predicate: r => r.ProgramId == id && !r.IsArchived,
                    orderBy: q => q.OrderByDescending(r => r.RiskScore).ThenBy(r => r.RiskCode),
                    disableTracking: true
                );

                return Ok(risks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving risks for program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving risks" });
            }
        }

        /// <summary>
        /// Get budget details for a vision program
        /// </summary>
        [HttpGet("{id}/budget")]
        public async Task<ActionResult<object>> GetProgramBudget(int id)
        {
            try
            {
                var programRepo = _unitOfWork.Repository<VisionProgram>();
                var budgetRepo = _unitOfWork.Repository<ProgramBudget>();

                var program = await programRepo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && !p.IsDeleted
                );

                if (program == null)
                {
                    return NotFound(new { error = $"Vision program with ID {id} not found" });
                }

                var budgets = await budgetRepo.GetAsync(
                    predicate: b => b.VisionProgram.Id == id,
                    orderBy: q => q.OrderBy(b => b.FiscalYear).ThenBy(b => b.Quarter),
                    disableTracking: true
                );

                var budgetSummary = new
                {
                    programId = id,
                    totalBudget = program.TotalBudget,
                    allocatedAmount = budgets.Sum(b => b.AllocatedAmount),
                    spentAmount = budgets.Sum(b => b.SpentAmount),
                    remainingAmount = program.TotalBudget - budgets.Sum(b => b.SpentAmount),
                    utilizationRate = program.TotalBudget > 0 
                        ? (budgets.Sum(b => b.SpentAmount) / program.TotalBudget.Value * 100) 
                        : 0,
                    budgetDetails = budgets
                };

                return Ok(budgetSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving budget for program {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving budget information" });
            }
        }

        /// <summary>
        /// Get vision programs dashboard statistics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboard()
        {
            try
            {
                var repo = _unitOfWork.Repository<VisionProgram>();
                var programs = await repo.GetAsync(
                    predicate: p => !p.IsDeleted,
                    includes: new List<Expression<Func<VisionProgram, object>>>
                    {
                        p => p.Initiatives,
                        p => p.KPIs,
                        p => p.Risks
                    },
                    disableTracking: true
                );

                var dashboard = new
                {
                    summary = new
                    {
                        totalPrograms = programs.Count,
                        activePrograms = programs.Count(p => p.Status == "Active"),
                        completedPrograms = programs.Count(p => p.Status == "Completed"),
                        notStartedPrograms = programs.Count(p => p.Status == "NotStarted"),
                        suspendedPrograms = programs.Count(p => p.Status == "Suspended")
                    },
                    initiatives = new
                    {
                        total = programs.SelectMany(p => p.Initiatives).Count(i => !i.IsDeleted),
                        inProgress = programs.SelectMany(p => p.Initiatives).Count(i => !i.IsDeleted && i.Status == "InProgress"),
                        completed = programs.SelectMany(p => p.Initiatives).Count(i => !i.IsDeleted && i.Status == "Completed")
                    },
                    risks = new
                    {
                        total = programs.SelectMany(p => p.Risks).Count(r => !r.IsArchived),
                        critical = programs.SelectMany(p => p.Risks).Count(r => !r.IsArchived && r.RiskLevel == "Critical"),
                        high = programs.SelectMany(p => p.Risks).Count(r => !r.IsArchived && r.RiskLevel == "High")
                    },
                    budget = new
                    {
                        totalAllocated = programs.Sum(p => p.TotalBudget ?? 0),
                        programsWithBudget = programs.Count(p => p.TotalBudget > 0)
                    }
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, new { error = "An error occurred while retrieving dashboard statistics" });
            }
        }
    }
}
