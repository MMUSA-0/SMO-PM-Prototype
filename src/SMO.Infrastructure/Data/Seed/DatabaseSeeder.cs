using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMO.Domain.Entities.Core;
using SMO.Domain.Entities.Shared;

namespace SMO.Infrastructure.Data.Seed
{
    /// <summary>
    /// Seeds initial data into the SMO database
    /// Includes sample Vision 2030 programs, initiatives, and shared resources
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Apply any pending migrations
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await _context.Database.MigrateAsync();
                }

                // Seed data
                await SeedVisionProgramsAsync();
                await SeedInitiativesAsync();
                await SeedSharedRisksAsync();
                await SeedSharedMilestonesAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private async Task SeedVisionProgramsAsync()
        {
            if (await _context.VisionPrograms.AnyAsync())
            {
                _logger.LogInformation("Vision Programs already exist, skipping seed");
                return;
            }

            var programs = new List<VisionProgram>
            {
                new VisionProgram
                {
                    Code = "QOL",
                    NameAr = "برنامج جودة الحياة",
                    NameEn = "Quality of Life Program",
                    DescriptionAr = "تحسين جودة الحياة للمقيمين والزوار",
                    DescriptionEn = "Improving the quality of life for residents and visitors",
                    VisionAlignment = "Vibrant Society",
                    StartDate = new DateTime(2018, 1, 1),
                    EndDate = new DateTime(2030, 12, 31),
                    Status = "Active",
                    ProgramOwner = "Ministry of Culture",
                    MinistryAr = "وزارة الثقافة",
                    MinistryEn = "Ministry of Culture",
                    TotalBudget = 1000000000,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new VisionProgram
                {
                    Code = "PIF",
                    NameAr = "برنامج صندوق الاستثمارات العامة",
                    NameEn = "Public Investment Fund Program",
                    DescriptionAr = "تعزيز موقع صندوق الاستثمارات العامة",
                    DescriptionEn = "Strengthening the Public Investment Fund's position",
                    VisionAlignment = "Thriving Economy",
                    StartDate = new DateTime(2017, 1, 1),
                    EndDate = new DateTime(2030, 12, 31),
                    Status = "Active",
                    ProgramOwner = "Public Investment Fund",
                    MinistryAr = "صندوق الاستثمارات العامة",
                    MinistryEn = "Public Investment Fund",
                    TotalBudget = 2000000000,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new VisionProgram
                {
                    Code = "NTP",
                    NameAr = "برنامج التحول الوطني",
                    NameEn = "National Transformation Program",
                    DescriptionAr = "تحقيق التميز الحكومي التشغيلي",
                    DescriptionEn = "Achieving governmental operational excellence",
                    VisionAlignment = "Ambitious Nation",
                    StartDate = new DateTime(2016, 6, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    Status = "Active",
                    ProgramOwner = "Ministry of Economy and Planning",
                    MinistryAr = "وزارة الاقتصاد والتخطيط",
                    MinistryEn = "Ministry of Economy and Planning",
                    TotalBudget = 1500000000,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                }
            };

            await _context.VisionPrograms.AddRangeAsync(programs);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {programs.Count} Vision Programs");
        }

        private async Task SeedInitiativesAsync()
        {
            if (await _context.Initiatives.AnyAsync())
            {
                _logger.LogInformation("Initiatives already exist, skipping seed");
                return;
            }

            var qolProgram = await _context.VisionPrograms.FirstOrDefaultAsync(p => p.Code == "QOL");
            if (qolProgram == null) return;

            var initiatives = new List<Initiative>
            {
                new Initiative
                {
                    Code = "INIT-001",
                    NameAr = "تطوير المتاحف الوطنية",
                    NameEn = "National Museums Development",
                    DescriptionAr = "تطوير وتحديث المتاحف الوطنية",
                    DescriptionEn = "Developing and modernizing national museums",
                    ProgramId = qolProgram.Id,
                    Status = "InProgress",
                    PlannedStartDate = new DateTime(2024, 1, 1),
                    PlannedEndDate = new DateTime(2025, 12, 31),
                    Budget = 50000000,
                    ProgressPercentage = 35,
                    Owner = "Ministry of Culture",
                    Priority = "High",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new Initiative
                {
                    Code = "INIT-002",
                    NameAr = "برنامج الرياضة المجتمعية",
                    NameEn = "Community Sports Program",
                    DescriptionAr = "تعزيز الرياضة على مستوى المجتمع",
                    DescriptionEn = "Promoting sports at community level",
                    ProgramId = qolProgram.Id,
                    Status = "InProgress",
                    PlannedStartDate = new DateTime(2023, 6, 1),
                    PlannedEndDate = new DateTime(2025, 6, 30),
                    Budget = 75000000,
                    ProgressPercentage = 60,
                    Owner = "Ministry of Sport",
                    Priority = "Medium",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                }
            };

            await _context.Initiatives.AddRangeAsync(initiatives);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {initiatives.Count} Initiatives");
        }

        private async Task SeedSharedRisksAsync()
        {
            if (await _context.Risks.AnyAsync())
            {
                _logger.LogInformation("Shared Risks already exist, skipping seed");
                return;
            }

            var risks = new List<Risk>
            {
                new Risk
                {
                    Code = "RISK-001",
                    Title = "Budget Overrun Risk",
                    TitleAr = "مخاطر تجاوز الميزانية",
                    Description = "Risk of exceeding allocated budget due to scope changes",
                    DescriptionAr = "مخاطر تجاوز الميزانية المخصصة بسبب تغييرات النطاق",
                    Category = "Financial",
                    Probability = "Medium",
                    Impact = "High",
                    Status = "Open",
                    MitigationPlan = "Regular budget reviews and change control process",
                    Owner = "Finance Department",
                    OriginModule = "Vision2030",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new Risk
                {
                    Code = "RISK-002",
                    Title = "Resource Availability",
                    TitleAr = "توفر الموارد",
                    Description = "Risk of key resources not being available when needed",
                    DescriptionAr = "مخاطر عدم توفر الموارد الرئيسية عند الحاجة",
                    Category = "Operational",
                    Probability = "Low",
                    Impact = "Medium",
                    Status = "Mitigating",
                    MitigationPlan = "Early resource planning and backup resource identification",
                    Owner = "HR Department",
                    OriginModule = "Shared",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                }
            };

            foreach (var risk in risks)
            {
                risk.CalculateRiskScore();
            }

            await _context.Risks.AddRangeAsync(risks);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {risks.Count} Shared Risks");
        }

        private async Task SeedSharedMilestonesAsync()
        {
            if (await _context.Milestones.AnyAsync())
            {
                _logger.LogInformation("Shared Milestones already exist, skipping seed");
                return;
            }

            var milestones = new List<Milestone>
            {
                new Milestone
                {
                    Title = "Q4 2024 - Platform Launch",
                    TitleAr = "الربع الرابع 2024 - إطلاق المنصة",
                    Description = "Launch of the unified SMO digital platform",
                    DescriptionAr = "إطلاق منصة SMO الرقمية الموحدة",
                    PlannedDate = new DateTime(2024, 12, 31),
                    Status = "InProgress",
                    Progress = 75,
                    IsCritical = true,
                    Owner = "IT Department",
                    OriginModule = "Shared",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new Milestone
                {
                    Title = "Mid-Year Review 2025",
                    TitleAr = "مراجعة منتصف العام 2025",
                    Description = "Comprehensive mid-year performance review",
                    DescriptionAr = "مراجعة شاملة للأداء في منتصف العام",
                    PlannedDate = new DateTime(2025, 6, 30),
                    Status = "Pending",
                    Progress = 0,
                    IsCritical = false,
                    Owner = "Performance Management",
                    OriginModule = "Performance",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                }
            };

            await _context.Milestones.AddRangeAsync(milestones);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {milestones.Count} Shared Milestones");
        }
    }
}
