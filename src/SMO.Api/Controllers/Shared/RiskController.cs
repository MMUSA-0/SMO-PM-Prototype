using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Risk;
using SMO.Domain.Entities.Core;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SMO.Api.Controllers.Shared
{
    /// <summary>
    /// SHARED Risk Controller - Used by BOTH Performance Management and Vision 2030 Modules
    /// Manages risks that can affect:
    /// - Performance Goals and Employee Development (Performance Module)
    /// - Strategic Programs and Initiatives (Vision 2030 Module)
    /// Based on UC-12 BRD specifications
    /// </summary>
    [ApiController]
    [Route("api/shared/[controller]")]
    public class RiskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RiskController> _logger;

        public RiskController(IUnitOfWork unitOfWork, ILogger<RiskController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Combined Risk Operations

        /// <summary>
        /// Get all risks across both Program and Initiative levels with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllRisks(
            [FromQuery] string module = "All",
            [FromQuery] string level = "All", // Program, Initiative, or All
            [FromQuery] string status = null,
            [FromQuery] bool isTop5Only = false,
            [FromQuery] string category = null)
        {
            try
            {
                var programRisks = new List<ProgramRisk>();
                var initiativeRisks = new List<InitiativeRisk>();

                // Get Program Risks
                if (level == "All" || level == "Program")
                {
                    var programRepo = _unitOfWork.Repository<ProgramRisk>();
                    Expression<Func<ProgramRisk, bool>> programPredicate = r => !r.IsArchived;
                    
                    if (!string.IsNullOrEmpty(status))
                        programPredicate = r => !r.IsArchived && r.RiskStatus == status;
                    
                    if (isTop5Only)
                        programPredicate = r => !r.IsArchived && r.IsTop5;
                    
                    if (!string.IsNullOrEmpty(category))
                        programPredicate = r => !r.IsArchived && r.Category == category;

                    var pRisks = await programRepo.GetAsync(
                        predicate: programPredicate,
                        orderBy: q => q.OrderByDescending(r => r.RiskScore).ThenBy(r => r.RiskCode),
                        includes: new List<Expression<Func<ProgramRisk, object>>>
                        {
                            r => r.Program
                        },
                        disableTracking: true
                    );
                    
                    programRisks.AddRange(pRisks);
                }

                // Get Initiative Risks
                if (level == "All" || level == "Initiative")
                {
                    var initiativeRepo = _unitOfWork.Repository<InitiativeRisk>();
                    Expression<Func<InitiativeRisk, bool>> initiativePredicate = r => !r.IsArchived;
                    
                    if (!string.IsNullOrEmpty(status))
                        initiativePredicate = r => !r.IsArchived && r.RiskStatus == status;
                    
                    if (isTop5Only)
                        initiativePredicate = r => !r.IsArchived && r.IsTop5;
                    
                    if (!string.IsNullOrEmpty(category))
                        initiativePredicate = r => !r.IsArchived && r.Category == category;

                    var iRisks = await initiativeRepo.GetAsync(
                        predicate: initiativePredicate,
                        orderBy: q => q.OrderByDescending(r => r.RiskScore).ThenBy(r => r.RiskCode),
                        includes: new List<Expression<Func<InitiativeRisk, object>>>
                        {
                            r => r.Initiative,
                            r => r.Program
                        },
                        disableTracking: true
                    );
                    
                    initiativeRisks.AddRange(iRisks);
                }

                // Combine and format response
                var response = new
                {
                    totalRisks = programRisks.Count + initiativeRisks.Count,
                    criticalCount = programRisks.Count(r => r.RiskLevel == "Critical") + 
                                   initiativeRisks.Count(r => r.RiskLevel == "Critical"),
                    highCount = programRisks.Count(r => r.RiskLevel == "High") + 
                               initiativeRisks.Count(r => r.RiskLevel == "High"),
                    programRisks = programRisks.Select(r => new
                    {
                        id = r.Id,
                        type = "Program",
                        riskCode = r.RiskCode,
                        riskName = r.RiskName,
                        programName = r.Program?.NameEn,
                        riskScore = r.RiskScore,
                        riskLevel = r.RiskLevel,
                        isTop5 = r.IsTop5,
                        status = r.RiskStatus,
                        mitigationStatus = r.MitigationStatus,
                        owner = r.Owner
                    }),
                    initiativeRisks = initiativeRisks.Select(r => new
                    {
                        id = r.Id,
                        type = "Initiative",
                        riskCode = r.RiskCode,
                        riskName = r.RiskName,
                        initiativeName = r.Initiative?.NameEn,
                        programName = r.Program?.NameEn,
                        riskScore = r.RiskScore,
                        riskLevel = r.RiskLevel,
                        isTop5 = r.IsTop5,
                        status = r.RiskStatus,
                        mitigationStatus = r.MitigationStatus,
                        owner = r.Owner
                    })
                };

                _logger.LogInformation($"Retrieved {response.totalRisks} risks (Module: {module}, Level: {level})");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risks");
                return StatusCode(500, new { error = "An error occurred while retrieving risks" });
            }
        }

        /// <summary>
        /// Get Top 5 risks across the platform
        /// </summary>
        [HttpGet("top5")]
        public async Task<ActionResult<object>> GetTop5Risks()
        {
            try
            {
                var programRepo = _unitOfWork.Repository<ProgramRisk>();
                var initiativeRepo = _unitOfWork.Repository<InitiativeRisk>();

                var programRisks = await programRepo.GetAsync(
                    predicate: r => r.IsTop5 && !r.IsArchived && r.RiskStatus == "Open",
                    orderBy: q => q.OrderByDescending(r => r.RiskScore),
                    includes: new List<Expression<Func<ProgramRisk, object>>>
                    {
                        r => r.Program
                    },
                    disableTracking: true
                );

                var initiativeRisks = await initiativeRepo.GetAsync(
                    predicate: r => r.IsTop5 && !r.IsArchived && r.RiskStatus == "Open",
                    orderBy: q => q.OrderByDescending(r => r.RiskScore),
                    includes: new List<Expression<Func<InitiativeRisk, object>>>
                    {
                        r => r.Initiative,
                        r => r.Program
                    },
                    disableTracking: true
                );

                var allRisks = programRisks.Select(r => new
                {
                    id = r.Id,
                    type = "Program",
                    riskCode = r.RiskCode,
                    riskName = r.RiskName,
                    entityName = r.Program?.NameEn,
                    riskScore = r.RiskScore,
                    riskLevel = r.RiskLevel,
                    probability = r.Probability,
                    impact = r.Impact,
                    mitigationStatus = r.MitigationStatus,
                    mitigationCompletion = r.MitigationCompletionPercentage,
                    owner = r.Owner,
                    responsibleEntity = r.ResponsibleEntity
                })
                .Concat(initiativeRisks.Select(r => new
                {
                    id = r.Id,
                    type = "Initiative",
                    riskCode = r.RiskCode,
                    riskName = r.RiskName,
                    entityName = r.Initiative?.NameEn,
                    riskScore = r.RiskScore,
                    riskLevel = r.RiskLevel,
                    probability = r.Probability,
                    impact = r.Impact,
                    mitigationStatus = r.MitigationStatus,
                    mitigationCompletion = r.MitigationCompletionPercentage,
                    owner = r.Owner,
                    responsibleEntity = r.ResponsibleEntity
                }))
                .OrderByDescending(r => r.riskScore)
                .Take(5);

                return Ok(allRisks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top 5 risks");
                return StatusCode(500, new { error = "An error occurred while retrieving top 5 risks" });
            }
        }

        #endregion

        #region Program Risk Operations

        /// <summary>
        /// Get all program risks
        /// </summary>
        [HttpGet("program")]
        public async Task<ActionResult<IEnumerable<ProgramRisk>>> GetProgramRisks(
            [FromQuery] int? programId = null,
            [FromQuery] string status = null)
        {
            try
            {
                var repo = _unitOfWork.Repository<ProgramRisk>();
                Expression<Func<ProgramRisk, bool>> predicate = r => !r.IsArchived;
                
                if (programId.HasValue)
                    predicate = r => !r.IsArchived && r.ProgramId == programId.Value;
                
                if (!string.IsNullOrEmpty(status))
                    predicate = r => !r.IsArchived && r.RiskStatus == status;

                var risks = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderByDescending(r => r.RiskScore).ThenBy(r => r.RiskCode),
                    includes: new List<Expression<Func<ProgramRisk, object>>>
                    {
                        r => r.Program,
                        r => r.Initiative
                    },
                    disableTracking: true
                );

                _logger.LogInformation($"Retrieved {risks.Count} program risks");
                return Ok(risks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving program risks");
                return StatusCode(500, new { error = "An error occurred while retrieving program risks" });
            }
        }

        /// <summary>
        /// Get program risk by ID
        /// </summary>
        [HttpGet("program/{id}")]
        public async Task<ActionResult<ProgramRisk>> GetProgramRisk(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<ProgramRisk>();
                var risk = await repo.GetFirstOrDefaultAsync(
                    predicate: r => r.Id == id,
                    includes: new List<Expression<Func<ProgramRisk, object>>>
                    {
                        r => r.Program,
                        r => r.Initiative
                    }
                );

                if (risk == null)
                {
                    return NotFound(new { error = $"Program risk with ID {id} not found" });
                }

                return Ok(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving program risk {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the program risk" });
            }
        }

        /// <summary>
        /// Create new program risk
        /// </summary>
        [HttpPost("program")]
        public async Task<ActionResult<ProgramRisk>> CreateProgramRisk([FromBody] ProgramRiskDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate program exists
                var programRepo = _unitOfWork.Repository<VisionProgram>();
                var programExists = await programRepo.GetFirstOrDefaultAsync(
                    predicate: p => p.Id == dto.ProgramId && p.IsDeleted == false
                );
                
                if (programExists == null)
                {
                    return BadRequest(new { error = "Invalid Program ID" });
                }

                var risk = new ProgramRisk
                {
                    ProgramId = dto.ProgramId,
                    InitiativeId = dto.InitiativeId,
                    RiskCode = GenerateRiskCode("Program", dto.ProgramId),
                    RiskName = dto.RiskName,
                    IsTop5 = dto.IsTop5,
                    RiskLevelType = "Program",
                    SubmittingEntity = dto.SubmittingEntity,
                    ResponsibleEntity = dto.ResponsibleEntity,
                    Probability = dto.Probability,
                    Impact = dto.Impact,
                    RiskScore = dto.Probability * dto.Impact, // Auto-calculate
                    RiskLevel = CalculateRiskLevel(dto.Probability * dto.Impact),
                    Category = dto.Category,
                    DescriptionAr = dto.DescriptionAr,
                    DescriptionEn = dto.DescriptionEn,
                    Owner = dto.Owner,
                    IdentifiedDate = dto.IdentifiedDate ?? DateTime.UtcNow,
                    TargetResolutionDate = dto.TargetResolutionDate,
                    RiskStatus = "Open",
                    MitigationStatus = "Not Started",
                    MitigationActionsCount = 0,
                    MitigationCompletionPercentage = 0,
                    RiskYear = DateTime.Now.Year,
                    RiskQuarter = GetCurrentQuarter(),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    IsArchived = false
                };

                var repo = _unitOfWork.Repository<ProgramRisk>();
                await repo.InsertAsync(risk, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created program risk: {risk.RiskCode} - affects BOTH modules");

                // Trigger notifications to both modules
                await NotifyModulesOfRiskCreation("Program", risk);

                return CreatedAtAction(nameof(GetProgramRisk), new { id = risk.Id }, risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating program risk");
                return StatusCode(500, new { error = "An error occurred while creating the program risk" });
            }
        }

        /// <summary>
        /// Update program risk
        /// </summary>
        [HttpPut("program/{id}")]
        public async Task<IActionResult> UpdateProgramRisk(int id, [FromBody] ProgramRiskDto dto)
        {
            try
            {
                var repo = _unitOfWork.Repository<ProgramRisk>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: r => r.Id == id
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"Program risk with ID {id} not found" });
                }

                // Update fields
                existing.RiskName = dto.RiskName ?? existing.RiskName;
                existing.IsTop5 = dto.IsTop5;
                existing.SubmittingEntity = dto.SubmittingEntity ?? existing.SubmittingEntity;
                existing.ResponsibleEntity = dto.ResponsibleEntity ?? existing.ResponsibleEntity;
                existing.Probability = dto.Probability;
                existing.Impact = dto.Impact;
                existing.RiskScore = dto.Probability * dto.Impact;
                existing.RiskLevel = CalculateRiskLevel(existing.RiskScore);
                existing.Category = dto.Category ?? existing.Category;
                existing.DescriptionAr = dto.DescriptionAr ?? existing.DescriptionAr;
                existing.DescriptionEn = dto.DescriptionEn ?? existing.DescriptionEn;
                existing.Owner = dto.Owner ?? existing.Owner;
                existing.TargetResolutionDate = dto.TargetResolutionDate ?? existing.TargetResolutionDate;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated program risk: {existing.RiskCode} - changes affect BOTH modules");

                // Notify both modules of update
                await NotifyModulesOfRiskUpdate("Program", existing);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating program risk {id}");
                return StatusCode(500, new { error = "An error occurred while updating the program risk" });
            }
        }

        #endregion

        #region Initiative Risk Operations

        /// <summary>
        /// Get all initiative risks
        /// </summary>
        [HttpGet("initiative")]
        public async Task<ActionResult<IEnumerable<InitiativeRisk>>> GetInitiativeRisks(
            [FromQuery] int? initiativeId = null,
            [FromQuery] int? programId = null,
            [FromQuery] string status = null)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeRisk>();
                Expression<Func<InitiativeRisk, bool>> predicate = r => !r.IsArchived;
                
                if (initiativeId.HasValue)
                    predicate = r => !r.IsArchived && r.InitiativeId == initiativeId.Value;
                
                if (programId.HasValue)
                    predicate = r => !r.IsArchived && r.ProgramId == programId.Value;
                
                if (!string.IsNullOrEmpty(status))
                    predicate = r => !r.IsArchived && r.RiskStatus == status;

                var risks = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderByDescending(r => r.RiskScore).ThenBy(r => r.RiskCode),
                    includes: new List<Expression<Func<InitiativeRisk, object>>>
                    {
                        r => r.Initiative,
                        r => r.Program
                    },
                    disableTracking: true
                );

                _logger.LogInformation($"Retrieved {risks.Count} initiative risks");
                return Ok(risks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving initiative risks");
                return StatusCode(500, new { error = "An error occurred while retrieving initiative risks" });
            }
        }

        /// <summary>
        /// Get initiative risk by ID
        /// </summary>
        [HttpGet("initiative/{id}")]
        public async Task<ActionResult<InitiativeRisk>> GetInitiativeRisk(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeRisk>();
                var risk = await repo.GetFirstOrDefaultAsync(
                    predicate: r => r.Id == id,
                    includes: new List<Expression<Func<InitiativeRisk, object>>>
                    {
                        r => r.Initiative,
                        r => r.Program
                    }
                );

                if (risk == null)
                {
                    return NotFound(new { error = $"Initiative risk with ID {id} not found" });
                }

                return Ok(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving initiative risk {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the initiative risk" });
            }
        }

        /// <summary>
        /// Create new initiative risk
        /// </summary>
        [HttpPost("initiative")]
        public async Task<ActionResult<InitiativeRisk>> CreateInitiativeRisk([FromBody] InitiativeRiskDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate initiative exists
                var initiativeRepo = _unitOfWork.Repository<Initiative>();
                var initiative = await initiativeRepo.GetFirstOrDefaultAsync(
                    predicate: i => i.Id == dto.InitiativeId && i.IsDeleted == false,
                    includes: new List<Expression<Func<Initiative, object>>>
                    {
                        i => i.Program
                    }
                );
                
                if (initiative == null)
                {
                    return BadRequest(new { error = "Invalid Initiative ID" });
                }

                var risk = new InitiativeRisk
                {
                    InitiativeId = dto.InitiativeId,
                    ProgramId = dto.ProgramId ?? initiative.ProgramId, // Use provided or get from initiative
                    RiskCode = GenerateRiskCode("Initiative", dto.InitiativeId),
                    RiskName = dto.RiskName,
                    IsTop5 = dto.IsTop5,
                    RiskLevelType = "Initiative",
                    SubmittingEntity = dto.SubmittingEntity,
                    ResponsibleEntity = dto.ResponsibleEntity,
                    Probability = dto.Probability,
                    Impact = dto.Impact,
                    RiskScore = dto.Probability * dto.Impact, // Auto-calculate
                    RiskLevel = CalculateRiskLevel(dto.Probability * dto.Impact),
                    Category = dto.Category,
                    DescriptionAr = dto.DescriptionAr,
                    DescriptionEn = dto.DescriptionEn,
                    Owner = dto.Owner,
                    IdentifiedDate = dto.IdentifiedDate ?? DateTime.UtcNow,
                    TargetResolutionDate = dto.TargetResolutionDate,
                    ContingencyPlan = dto.ContingencyPlan,
                    RiskStatus = "Open",
                    MitigationStatus = "Not Started",
                    MitigationActionsCount = 0,
                    MitigationCompletionPercentage = 0,
                    RiskYear = DateTime.Now.Year,
                    RiskQuarter = GetCurrentQuarter(),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    IsArchived = false
                };

                var repo = _unitOfWork.Repository<InitiativeRisk>();
                await repo.InsertAsync(risk, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created initiative risk: {risk.RiskCode} - affects BOTH modules");

                // Trigger notifications to both modules
                await NotifyModulesOfRiskCreation("Initiative", risk);

                return CreatedAtAction(nameof(GetInitiativeRisk), new { id = risk.Id }, risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating initiative risk");
                return StatusCode(500, new { error = "An error occurred while creating the initiative risk" });
            }
        }

        #endregion

        #region Risk Mitigation Operations

        /// <summary>
        /// Update mitigation for a risk
        /// </summary>
        [HttpPost("{type}/{id}/mitigation")]
        public async Task<IActionResult> UpdateMitigation(string type, int id, [FromBody] MitigationDto dto)
        {
            try
            {
                if (type.ToLower() == "program")
                {
                    var repo = _unitOfWork.Repository<ProgramRisk>();
                    var risk = await repo.GetFirstOrDefaultAsync(r => r.Id == id);
                    
                    if (risk == null)
                        return NotFound(new { error = $"Program risk with ID {id} not found" });

                    risk.MitigationStatus = dto.Status;
                    risk.MitigationActions = dto.Actions;
                    risk.MitigationActionsCount = dto.ActionsCount;
                    risk.MitigationCompletionPercentage = dto.CompletionPercentage;
                    risk.ResidualRiskLevel = CalculateResidualRisk(risk.RiskScore, dto.CompletionPercentage);
                    risk.UpdatedOn = DateTime.UtcNow;
                    risk.UpdatedBy = User.Identity?.Name ?? "System";

                    repo.Update(risk);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (type.ToLower() == "initiative")
                {
                    var repo = _unitOfWork.Repository<InitiativeRisk>();
                    var risk = await repo.GetFirstOrDefaultAsync(r => r.Id == id);
                    
                    if (risk == null)
                        return NotFound(new { error = $"Initiative risk with ID {id} not found" });

                    risk.MitigationStatus = dto.Status;
                    risk.MitigationActions = dto.Actions;
                    risk.MitigationActionsCount = dto.ActionsCount;
                    risk.MitigationCompletionPercentage = dto.CompletionPercentage;
                    risk.ResidualRiskLevel = CalculateResidualRisk(risk.RiskScore, dto.CompletionPercentage);
                    risk.ContingencyPlan = dto.ContingencyPlan;
                    risk.UpdatedOn = DateTime.UtcNow;
                    risk.UpdatedBy = User.Identity?.Name ?? "System";

                    repo.Update(risk);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    return BadRequest(new { error = "Invalid risk type. Use 'program' or 'initiative'" });
                }

                _logger.LogInformation($"Updated mitigation for {type} risk {id}");
                return Ok(new { message = "Mitigation updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating mitigation for {type} risk {id}");
                return StatusCode(500, new { error = "An error occurred while updating mitigation" });
            }
        }

        /// <summary>
        /// Close/resolve a risk
        /// </summary>
        [HttpPost("{type}/{id}/close")]
        public async Task<IActionResult> CloseRisk(string type, int id, [FromBody] RiskClosureDto dto)
        {
            try
            {
                if (type.ToLower() == "program")
                {
                    var repo = _unitOfWork.Repository<ProgramRisk>();
                    var risk = await repo.GetFirstOrDefaultAsync(r => r.Id == id);
                    
                    if (risk == null)
                        return NotFound(new { error = $"Program risk with ID {id} not found" });

                    risk.RiskStatus = "Closed";
                    risk.ActualResolutionDate = dto.ResolutionDate ?? DateTime.UtcNow;
                    risk.MitigationCompletionPercentage = 100;
                    risk.UpdatedOn = DateTime.UtcNow;
                    risk.UpdatedBy = User.Identity?.Name ?? "System";

                    repo.Update(risk);
                }
                else if (type.ToLower() == "initiative")
                {
                    var repo = _unitOfWork.Repository<InitiativeRisk>();
                    var risk = await repo.GetFirstOrDefaultAsync(r => r.Id == id);
                    
                    if (risk == null)
                        return NotFound(new { error = $"Initiative risk with ID {id} not found" });

                    risk.RiskStatus = "Closed";
                    risk.ActualResolutionDate = dto.ResolutionDate ?? DateTime.UtcNow;
                    risk.MitigationCompletionPercentage = 100;
                    risk.UpdatedOn = DateTime.UtcNow;
                    risk.UpdatedBy = User.Identity?.Name ?? "System";

                    repo.Update(risk);
                }
                else
                {
                    return BadRequest(new { error = "Invalid risk type" });
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Closed {type} risk {id} - notifying BOTH modules");
                
                return Ok(new { message = "Risk closed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error closing {type} risk {id}");
                return StatusCode(500, new { error = "An error occurred while closing the risk" });
            }
        }

        #endregion

        #region Risk Dashboard Operations

        /// <summary>
        /// Get risk dashboard statistics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetRiskDashboard()
        {
            try
            {
                var programRepo = _unitOfWork.Repository<ProgramRisk>();
                var initiativeRepo = _unitOfWork.Repository<InitiativeRisk>();

                var programRisks = await programRepo.GetAsync(
                    predicate: r => !r.IsArchived,
                    disableTracking: true
                );

                var initiativeRisks = await initiativeRepo.GetAsync(
                    predicate: r => !r.IsArchived,
                    disableTracking: true
                );

                var dashboard = new
                {
                    summary = new
                    {
                        totalRisks = programRisks.Count + initiativeRisks.Count,
                        openRisks = programRisks.Count(r => r.RiskStatus == "Open") + 
                                   initiativeRisks.Count(r => r.RiskStatus == "Open"),
                        closedRisks = programRisks.Count(r => r.RiskStatus == "Closed") + 
                                     initiativeRisks.Count(r => r.RiskStatus == "Closed"),
                        top5Risks = programRisks.Count(r => r.IsTop5) + 
                                   initiativeRisks.Count(r => r.IsTop5)
                    },
                    byLevel = new
                    {
                        critical = programRisks.Count(r => r.RiskLevel == "Critical") + 
                                  initiativeRisks.Count(r => r.RiskLevel == "Critical"),
                        high = programRisks.Count(r => r.RiskLevel == "High") + 
                              initiativeRisks.Count(r => r.RiskLevel == "High"),
                        medium = programRisks.Count(r => r.RiskLevel == "Medium") + 
                                initiativeRisks.Count(r => r.RiskLevel == "Medium"),
                        low = programRisks.Count(r => r.RiskLevel == "Low") + 
                             initiativeRisks.Count(r => r.RiskLevel == "Low")
                    },
                    byCategory = new
                    {
                        strategic = programRisks.Count(r => r.Category == "Strategic") + 
                                   initiativeRisks.Count(r => r.Category == "Strategic"),
                        operational = programRisks.Count(r => r.Category == "Operational") + 
                                     initiativeRisks.Count(r => r.Category == "Operational"),
                        financial = programRisks.Count(r => r.Category == "Financial") + 
                                   initiativeRisks.Count(r => r.Category == "Financial"),
                        technical = programRisks.Count(r => r.Category == "Technical") + 
                                   initiativeRisks.Count(r => r.Category == "Technical"),
                        compliance = programRisks.Count(r => r.Category == "Compliance") + 
                                    initiativeRisks.Count(r => r.Category == "Compliance")
                    },
                    mitigation = new
                    {
                        notStarted = programRisks.Count(r => r.MitigationStatus == "Not Started") + 
                                    initiativeRisks.Count(r => r.MitigationStatus == "Not Started"),
                        inProgress = programRisks.Count(r => r.MitigationStatus == "In Progress") + 
                                    initiativeRisks.Count(r => r.MitigationStatus == "In Progress"),
                        completed = programRisks.Count(r => r.MitigationStatus == "Completed") + 
                                   initiativeRisks.Count(r => r.MitigationStatus == "Completed")
                    },
                    trend = new
                    {
                        currentQuarter = GetCurrentQuarter(),
                        currentYear = DateTime.Now.Year,
                        newRisksThisQuarter = programRisks.Count(r => r.RiskQuarter == GetCurrentQuarter() && 
                                                                      r.RiskYear == DateTime.Now.Year) +
                                             initiativeRisks.Count(r => r.RiskQuarter == GetCurrentQuarter() && 
                                                                       r.RiskYear == DateTime.Now.Year)
                    }
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk dashboard");
                return StatusCode(500, new { error = "An error occurred while retrieving risk dashboard" });
            }
        }

        #endregion

        #region Helper Methods

        private string GenerateRiskCode(string type, int entityId)
        {
            // Format: VRP_X_XXX_### (as per UC-12 BRD)
            var prefix = type == "Program" ? "VRP_P" : "VRP_I";
            var timestamp = DateTime.Now.ToString("MMdd");
            var random = new Random().Next(100, 999);
            return $"{prefix}_{entityId:D3}_{random}";
        }

        private string CalculateRiskLevel(int riskScore)
        {
            if (riskScore >= 20) return "Critical";
            if (riskScore >= 15) return "High";
            if (riskScore >= 10) return "Medium";
            return "Low";
        }

        private string CalculateResidualRisk(int originalScore, decimal mitigationPercentage)
        {
            var residualScore = originalScore * (1 - (mitigationPercentage / 100));
            return CalculateRiskLevel((int)residualScore);
        }

        private string GetCurrentQuarter()
        {
            var month = DateTime.Now.Month;
            if (month <= 3) return "Q1";
            if (month <= 6) return "Q2";
            if (month <= 9) return "Q3";
            return "Q4";
        }

        private async Task NotifyModulesOfRiskCreation(string type, object risk)
        {
            // TODO: Implement notification logic
            _logger.LogInformation($"Notifying modules of new {type} risk creation");
            await Task.CompletedTask;
        }

        private async Task NotifyModulesOfRiskUpdate(string type, object risk)
        {
            // TODO: Implement notification logic
            _logger.LogInformation($"Notifying modules of {type} risk update");
            await Task.CompletedTask;
        }

        #endregion
    }

    #region DTOs

    public class ProgramRiskDto
    {
        public int ProgramId { get; set; }
        public int? InitiativeId { get; set; }
        public string RiskName { get; set; }
        public bool IsTop5 { get; set; }
        public string SubmittingEntity { get; set; }
        public string ResponsibleEntity { get; set; }
        public int Probability { get; set; } // 1-5
        public int Impact { get; set; } // 1-5
        public string Category { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Owner { get; set; }
        public DateTime? IdentifiedDate { get; set; }
        public DateTime? TargetResolutionDate { get; set; }
    }

    public class InitiativeRiskDto
    {
        public int InitiativeId { get; set; }
        public int? ProgramId { get; set; }
        public string RiskName { get; set; }
        public bool IsTop5 { get; set; }
        public string SubmittingEntity { get; set; }
        public string ResponsibleEntity { get; set; }
        public int Probability { get; set; } // 1-5
        public int Impact { get; set; } // 1-5
        public string Category { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Owner { get; set; }
        public DateTime? IdentifiedDate { get; set; }
        public DateTime? TargetResolutionDate { get; set; }
        public string ContingencyPlan { get; set; }
    }

    public class MitigationDto
    {
        public string Status { get; set; }
        public string Actions { get; set; }
        public int ActionsCount { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string ContingencyPlan { get; set; } // For initiative risks
    }

    public class RiskClosureDto
    {
        public DateTime? ResolutionDate { get; set; }
        public string ClosureNotes { get; set; }
    }

    #endregion
}
