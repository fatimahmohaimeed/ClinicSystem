using ClinicSystemTest.Entities;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ClinicSystemTest.Models
{
    public class AddDoctorViewModel
    {
        // Add Doctor details
        //List Department
        //select user
        public Doctor doctor { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
