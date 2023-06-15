using ClinicSystemTest.Data;
using ClinicSystemTest.Entities;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult Index()
        {
            try
            {
                var user = userManager.GetUserId(User);// if user login
                var pationtLogIn = context.Patients.FirstOrDefault(a => a.PatientUserId == user);

                if (pationtLogIn == null)
                {
                    return RedirectToAction("AddProfile");
                }

                var appointments = context.Appointments.Where(p => p.PatientId == null || p.PatientId == pationtLogIn.Id).ToList();//

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
                        Name = context.Doctors.FirstOrDefault(n => n.Id == apoin.DoctorId).Name,
                        IsReserved = (apoin.PatientId == pationtLogIn.Id)

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
