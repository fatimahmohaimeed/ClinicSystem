using ClinicSystemTest.Entities;

namespace ClinicSystemTest.Models
{
    public class ListDoctorAppointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public int? PatientId { get; set; }

        public decimal Price { get; set; }

        public List<Appointment> appointments { get; set; }
        public bool IsAppointment { get; set; }
    }
}
