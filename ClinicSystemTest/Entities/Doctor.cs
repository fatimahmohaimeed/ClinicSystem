using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicSystemTest.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public List<Appointment>? Appointments { get; set; }

        [ForeignKey("IdentityUser")]
        public string DoctorUserId { get; set; }
        public virtual IdentityUser DoctorUser { get; set; }

    }
}