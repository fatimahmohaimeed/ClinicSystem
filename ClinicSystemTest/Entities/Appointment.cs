using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;

namespace ClinicSystemTest.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please enter appointment date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy }")]//{0:dd-MMMM-yyyy } \\{0:dd-MM-yyyy }
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Please enter appointment time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime AppointmentTime { get; set; }

        [UIHint("Currency")]
        public decimal AppointmentPrice { get; set; }



        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("Patient")]
        public int? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

    }
}
