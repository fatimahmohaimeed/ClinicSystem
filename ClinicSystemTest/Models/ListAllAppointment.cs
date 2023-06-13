using ClinicSystemTest.Entities;

namespace ClinicSystemTest.Models
{
    public class ListAllAppointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string DoctorName { get; set; }
        



        public Appointment Appointment { get; set; }

        public Doctor doctor { get; set; }


    }
}
