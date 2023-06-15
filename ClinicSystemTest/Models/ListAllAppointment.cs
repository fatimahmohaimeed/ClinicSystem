using ClinicSystemTest.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClinicSystemTest.Models
{
    public class ListAllAppointment
    {
        public bool IsReserved { get; set; }

        public int AppId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy }")]
        public DateTime Data { get; set; }

        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string Name { get; set; }



    }
}
