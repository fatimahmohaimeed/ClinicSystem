using ClinicSystemTest.Data;
using ClinicSystemTest.Entities;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace ClinicSystemTest.Controllers
{
    public class PatientController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        readonly ClinicSystemTestContext context;


        public PatientController(UserManager<IdentityUser> userManager, ClinicSystemTestContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var appointments = context.Appointments;
                //var doctor = context.Doctors.();

                List<ListAllAppointment> appointmentList = new List<ListAllAppointment>();
                foreach (var apoin in appointments)
                {
                    appointmentList.Add(new ListAllAppointment
                    {
                        AppId = apoin.AppointmentId,
                        Data = apoin.AppointmentDate,
                        Time = apoin.AppointmentTime,
                        Price = apoin.AppointmentPrice,
                        DoctorId = apoin.DoctorId,
                        //Name = context.Doctors.Where(d => d.Id == apoin.DoctorId).FirstOrDefault(n=>n.Name == doctor.Name),
                        IsReserved = context.Patients.Any(a => a.Id == apoin.PatientId)
                    });
                }
                return View(appointmentList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        #region Add reservation

        public IActionResult book(int id, bool isReserved)
        {
            try
            {
                var user = userManager.GetUserId(User);// if user login
                if (user == null)
                {
                    return RedirectToAction("Index");
                }

                var pation1 = context.Patients.FirstOrDefault(x => x.PatientUserId == user); //Checks if (patient's id, doctor's id, day) has been booked more than once
                var appointment = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == appointment.DoctorId)
                    .Where(d => d.AppointmentTime == appointment.AppointmentTime)
                    .Where(p => p.PatientId == pation1.Id).ToList();

                if (isReserved == false)// add appointment
                {
                    if (appointments.Count != 0)
                    {
                        return RedirectToAction("ValdiationErrorr", "Home");
                    }
                    else
                    {

                        var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                        var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
                        appointm.PatientId = pation.Id;
                        context.SaveChanges();
                    }
                }
                else
                { // remove appointment
                    var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                    var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
                    appointm.PatientId = null;
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion


        #region Add Profilr

        [HttpGet]
        public IActionResult AddProfile()
        {
            try
            {
                var user = userManager.GetUserId(User);
                if (user == null)
                {
                    return RedirectToAction("Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddProfile(Patient model)
        {
            try
            {
                var user = userManager.GetUserId(User);
                if (user == null)
                {
                    return RedirectToAction("Home");
                }
                else
                {
                    Patient patient = new Patient()
                    {
                        Name = model.Name,
                        PatientUserId = user,
                    };
                    context.Patients.Add(patient);
                    context.SaveChanges();
                    return RedirectToAction("Index", "Patient");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        #endregion

    }
}
