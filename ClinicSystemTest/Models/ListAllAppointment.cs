using ClinicSystemTest.Entities;

namespace ClinicSystemTest.Models
{
    public class ListAllAppointment
    {
        public bool IsReserved { get; set; }

        public int AppId { get; set; }
        public DateTime Data { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string Name { get; set; }



    }
}
