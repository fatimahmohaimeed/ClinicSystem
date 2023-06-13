using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicSystemTest.Entities
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Appointment>? Appointments { get; set; }

        [ForeignKey("IdentityUser")]
        public string PatientUserId { get; set; }
        public virtual IdentityUser PatientUser { get; set; }

    }
}
