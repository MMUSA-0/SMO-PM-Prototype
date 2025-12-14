using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMO.Domain.Common;

namespace SMO.Domain.Entities.EmployeePerformance
{
    /// <summary>
    /// Employee entity for Performance Management Module
    /// </summary>
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; }

        [StringLength(200)]
        public string FullNameAr { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }

        public DateTime JoinDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation Properties
        public virtual ICollection<Employee> Subordinates { get; set; }
        public virtual ICollection<PerformanceGoal> PerformanceGoals { get; set; }
        public virtual ICollection<PerformanceReview> PerformanceReviews { get; set; }
        public virtual ICollection<PerformanceRating> PerformanceRatings { get; set; }

        public Employee()
        {
            Subordinates = new HashSet<Employee>();
            PerformanceGoals = new HashSet<PerformanceGoal>();
            PerformanceReviews = new HashSet<PerformanceReview>();
            PerformanceRatings = new HashSet<PerformanceRating>();
        }
    }
}
