using ClinicSystemTest.Data;
using ClinicSystemTest.Entities;
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
        
        public IActionResult Index()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (userId.IsNullOrEmpty())//ceck it is not null
                {
                    return RedirectToAction("Index");
                }
                var doctor = context.Doctors.FirstOrDefault(d => d.DoctorUserId == userId);
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == doctor.Id)
                    .Include(d=>d.Doctor)
                    .Include(p=>p.Patient)
                    .ToList();
                return View(appointments);
            }catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }

        }

        #region Delete Appointmen
        public IActionResult DeleteAppointment(int id)
        {
            try
            {
                var appointment = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                context.Appointments.Remove(appointment);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }

        #endregion

    }
}
