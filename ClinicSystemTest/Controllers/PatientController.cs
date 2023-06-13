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


        public PatientController(UserManager<IdentityUser> userManager,  ClinicSystemTestContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> Index1()
        {
            var appointments = context.Appointments;


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
                    //PatientId = apoin.PatientId,
                    IsReserved = context.Patients.Any(a => a.Id == apoin.PatientId)
                });
            }
            return View(appointmentList);

        }




        public async Task<IActionResult> book(int id, bool isReserved)
        {
            var user = userManager.GetUserId(User);
            if (user == null)
            {
                return RedirectToAction("Index1");
            }
            if (isReserved == false)
            {
                var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
                appointm.PatientId = pation.Id;
                context.SaveChanges();
            }
            else
            {
                var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
                var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
                appointm.PatientId = pation.Id;
                context.Remove(appointm.PatientId);
                context.SaveChanges();
            }

            return RedirectToAction("Index1");
        }





        //public async Task<IActionResult> book(int id)
        //{
        //    var user = userManager.GetUserId(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
        //    var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
        //    appointm.PatientId = pation.Id;
        //    context.SaveChanges();

        //    return RedirectToAction("Index");
        //}


        // -------------------------------------------------------------------------------------------------
        //public async Task<IActionResult> Index1()
        //{
        //    List<Appointment> appointments = context.Appointments
        //   .Include(c => c.Doctor)
        //   .Include(c => c.Patient)
        //   .ToList();
        //    return View(appointments);
        //}

        //public async Task<IActionResult> book(int id)
        //{
        //    var user = userManager.GetUserId(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var appointm = context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
        //    var pation = context.Patients.FirstOrDefault(x => x.PatientUserId == user);
        //    appointm.PatientId = pation.Id;
        //    context.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        // -------------------------------------------------------------------------------------------------









        /*public async Task<IActionResult> Index1(int id, bool isReserved, Appointment appointment)
        {
            var user = userManager.GetUserId(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var appointm = context.Appointments.FirstOrDefaultAsync(a=>a.AppointmentId == id);
            var pation = context.Patients.FirstOrDefaultAsync(x => x.PatientUserId == user);



                if (pation != null && appointm != null)
                {
                    Appointment patient = new Appointment()
                    {
                        AppointmentId = id,
                        AppointmentDate = appointment.AppointmentDate,
                        AppointmentTime = appointment.AppointmentTime,
                        AppointmentPrice = appointment.AppointmentPrice,
                        DoctorId = appointment.DoctorId,
                        PatientId = pation.Id

                    };
                    context.Appointments.Add(patient);
                    context.SaveChanges();

                }

            return RedirectToAction("Index");
        }*/



        [HttpGet]
        public IActionResult AddProfile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProfile(Patient model)
        {
            var user = userManager.GetUserId(User);
            if (user == null)
            {
                return RedirectToAction("/Account/Register");
            }
            else
            {
                Patient patient = new Patient()
                {
                    Name = model.Name,
                    PatientUserId =user,
                };
                context.Patients.Add(patient);
                context.SaveChanges();
                return RedirectToAction("index", "Home");
            }

        }



        //public IActionResult ListAppointmentforPatient()
        //{
        //    var userId = userManager.GetUserId(User);  //user id in session
        //    var user = userManager.Users.FirstOrDefault().Id;
        //    var profiles = context.Profiles;


        //    //var Profiles = _context.Profiles.Where(a => a.ProfileUserId ==  userId);

        //    List<ListUsers> usersList = new List<ListUsers>();
        //    foreach (var pro in profiles)
        //    {
        //        if (pro.ProfileUserId == userId)
        //            continue;


        //        var follwerUser = new ListUsers
        //        {
        //            //Id = _userManager.Users.FirstOrDefault().Id,
        //            //Id =_context.Profiles.FirstOrDefault(a=>a.ProfileUserId == user),
        //            Id = pro.ProfileUserId,
        //            IsFollower = _context.Followers.Any(a => a.UserId == userId && a.FollowerUserId == pro.ProfileUserId),
        //            UserName = pro.UserName,
        //            ImagePath = pro.PhotoPath,
        //        };

        //        usersList.Add(follwerUser);
        //    }
        //    return View(usersList);
        //}







    }
}
