using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entities
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        [StringLength(30)]
        public string Phone { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
        [StringLength(50)]
        public string Address { get; set; }
        public int Salary { get; set; }
        [StringLength(30)]
        public string Qualification { get; set; }
        [StringLength(30)]
        public string Designation { get; set; }
    }
}
