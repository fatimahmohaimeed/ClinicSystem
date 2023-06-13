using ClinicSystemTest.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicSystemTest.Models
{
    public class CreateAppointmentViewModel
    {
        public Appointment appointment { get; set; }

        public IEnumerable<SelectListItem> Doctors { get; set; }

    }
}
