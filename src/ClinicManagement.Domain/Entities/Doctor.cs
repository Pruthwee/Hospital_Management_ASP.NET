using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorID { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        [StringLength(30)]
        public string Email { get; set; }
        [Required]
        [StringLength(30)]
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public int DeptNo { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
        [StringLength(30)]
        public string Address { get; set; }
        public int Experience { get; set; }
        public decimal Salary { get; set; }
        public decimal ChargesPerVisit { get; set; }
        [StringLength(30)]
        public string Specialization { get; set; }
        [StringLength(30)]
        public string Qualification { get; set; }
        public int Status { get; set; } = 1;
    }
}
