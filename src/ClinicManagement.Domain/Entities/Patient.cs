using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entities
{
    public class Patient
    {
        [Key]
        public int PatientID { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        [Required]
        [StringLength(15)]
        public string Phone { get; set; }
        [StringLength(40)]
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
        [Required]
        [StringLength(30)]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
    }
}
