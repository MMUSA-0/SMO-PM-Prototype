using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Identity.Migrations
{
    /// <summary>
    /// Seeds SMO-specific roles into the system
    /// </summary>
    public partial class SeedSMORoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var roles = new[]
            {
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 10, 
                    Name = "ExecutiveOffice",
                    NormalizedName = "EXECUTIVEOFFICE",
                    DisplayNameEn = "Executive Office", 
                    DisplayNameAr = "المكتب التنفيذي",
                    DescriptionEn = "Executive Office with full administrative privileges",
                    DescriptionAr = "المكتب التنفيذي مع كامل الصلاحيات الإدارية",
                    RoleGroup = "Executive",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 11, 
                    Name = "VRO",
                    NormalizedName = "VRO",
                    DisplayNameEn = "Vision Realization Office", 
                    DisplayNameAr = "مكتب تحقيق الرؤية",
                    DescriptionEn = "Vision Realization Office for strategic oversight",
                    DescriptionAr = "مكتب تحقيق الرؤية للإشراف الاستراتيجي",
                    RoleGroup = "Strategic",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 12, 
                    Name = "VRP",
                    NormalizedName = "VRP",
                    DisplayNameEn = "Vision Realization Program", 
                    DisplayNameAr = "برنامج تحقيق الرؤية",
                    DescriptionEn = "Vision Realization Program management",
                    DescriptionAr = "إدارة برنامج تحقيق الرؤية",
                    RoleGroup = "Program",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 13, 
                    Name = "SMO",
                    NormalizedName = "SMO",
                    DisplayNameEn = "Strategic Management Office", 
                    DisplayNameAr = "مكتب الإدارة الاستراتيجية",
                    DescriptionEn = "Strategic Management Office for performance management",
                    DescriptionAr = "مكتب الإدارة الاستراتيجية لإدارة الأداء",
                    RoleGroup = "Management",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 14, 
                    Name = "ADAA",
                    NormalizedName = "ADAA",
                    DisplayNameEn = "National Center for Performance Management (ADAA)", 
                    DisplayNameAr = "المركز الوطني لقياس الأداء (أداء)",
                    DescriptionEn = "National Center for Government Performance Management",
                    DescriptionAr = "المركز الوطني لقياس أداء الأجهزة العامة",
                    RoleGroup = "Performance",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 15, 
                    Name = "ProgramOwner",
                    NormalizedName = "PROGRAMOWNER",
                    DisplayNameEn = "Program Owner", 
                    DisplayNameAr = "مالك البرنامج",
                    DescriptionEn = "Owner of specific Vision 2030 programs",
                    DescriptionAr = "مالك برامج رؤية 2030 المحددة",
                    RoleGroup = "Program",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 16, 
                    Name = "InitiativeOwner",
                    NormalizedName = "INITIATIVEOWNER",
                    DisplayNameEn = "Initiative Owner", 
                    DisplayNameAr = "مالك المبادرة",
                    DescriptionEn = "Owner of specific initiatives",
                    DescriptionAr = "مالك المبادرات المحددة",
                    RoleGroup = "Initiative",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 17, 
                    Name = "PerformanceManager",
                    NormalizedName = "PERFORMANCEMANAGER",
                    DisplayNameEn = "Performance Manager", 
                    DisplayNameAr = "مدير الأداء",
                    DescriptionEn = "Manages performance evaluations and KPIs",
                    DescriptionAr = "يدير تقييمات الأداء ومؤشرات الأداء الرئيسية",
                    RoleGroup = "Performance",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                },
                new { 
                    Id = Guid.NewGuid(), 
                    Code = 18, 
                    Name = "DataAnalyst",
                    NormalizedName = "DATAANALYST",
                    DisplayNameEn = "Data Analyst", 
                    DisplayNameAr = "محلل البيانات",
                    DescriptionEn = "Analyzes performance data and generates reports",
                    DescriptionAr = "يحلل بيانات الأداء وينشئ التقارير",
                    RoleGroup = "Analytics",
                    IsDefault = false,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                }
            };

            foreach (var role in roles)
            {
                migrationBuilder.InsertData(
                    table: "Roles",
                    columns: new[] { 
                        "Id", "Code", "Name", "NormalizedName", 
                        "DisplayNameEn", "DisplayNameAr", 
                        "DescriptionEn", "DescriptionAr", 
                        "RoleGroup", "IsDefault", "CreatedBy", "CreatedOn",
                        "ConcurrencyStamp"
                    },
                    values: new object[] { 
                        role.Id, role.Code, role.Name, role.NormalizedName, 
                        role.DisplayNameEn, role.DisplayNameAr, 
                        role.DescriptionEn, role.DescriptionAr, 
                        role.RoleGroup, role.IsDefault, role.CreatedBy, role.CreatedOn,
                        Guid.NewGuid().ToString()
                    }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded roles
            migrationBuilder.Sql(@"
                DELETE FROM Roles 
                WHERE Name IN ('ExecutiveOffice', 'VRO', 'VRP', 'SMO', 'ADAA', 
                              'ProgramOwner', 'InitiativeOwner', 'PerformanceManager', 'DataAnalyst')
            ");
        }
    }
}


