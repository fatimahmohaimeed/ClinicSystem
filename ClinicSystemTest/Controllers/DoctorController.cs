using ClinicSystemTest.Data;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace ClinicSystemTest.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class DoctorController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        readonly ClinicSystemTestContext context;


        public DoctorController(UserManager<IdentityUser> userManager, ClinicSystemTestContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        //public IActionResult Index()
        //{
        //    var user = userManager.GetUserId(User);
        //    return View();
        //}
        
        public IActionResult Index()
        {
            var userId = userManager.GetUserId(User);
            if (userId.IsNullOrEmpty())//ceck it is not null
            {
                return RedirectToAction("Index");
            }
            var appointments = context.Appointments;
            var doctors = context.Doctors.ToList();

            List<ListDoctorAppointment> doctorAppointmentlIST = new List<ListDoctorAppointment>();
            foreach (var appointment in appointments)
            {
                foreach(var doctor in doctors)
                {
                    var doctorAppointment = new ListDoctorAppointment
                    {
                        Id = appointment.AppointmentId,
                        Date = appointment.AppointmentDate,
                        IsAppointment = context.Appointments.Any(a => a.DoctorId == doctor.Id && doctor.DoctorUserId == userId),
                    };

                    doctorAppointmentlIST.Add(doctorAppointment);
                }
            }
            return View(doctorAppointmentlIST);
        }




    }
}
