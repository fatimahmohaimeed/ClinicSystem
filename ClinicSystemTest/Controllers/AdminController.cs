using ClinicSystemTest.Data;
using ClinicSystemTest.Entities;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace ClinicSystemTest.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        readonly ClinicSystemTestContext context;


        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ClinicSystemTestContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            var appointments = context.Appointments.ToList();

            return View(appointments);
        }


        #region Roles
        #region  CreateRole


        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost] // post new role
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // saves role AspNetRoles table
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles", "Admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region List users with roles
        public async Task<IActionResult> GetRoles()
        {
            var users = userManager.Users;

            List<ListViewModel> usersList = new List<ListViewModel>();
            foreach (var user in users)
            {
                usersList.Add(new ListViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = (List<string>)userManager.GetRolesAsync(user).Result
                });
            }

            return View(usersList);
        }
        #endregion

        #region Assigning Role
        [HttpGet]
        public async Task<IActionResult> AssigningRole(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = userManager.IsInRoleAsync(user, role.Name).Result

                }).ToList()
            };
            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //  [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> AssigningRole(UserRolesViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                    await userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)//role is alredy assigned => no && role is selected => yes == Add ROLE
                    await userManager.AddToRoleAsync(user, role.RoleName);
            }
            return RedirectToAction("GetRoles"); //or  return RedirectToAction(nameof(Index));
        }
        #endregion

        #endregion



        #region Create Department
        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateDepartment(Department department)
        {
            context.Departments.Add(department);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region Add Doctor

        [HttpGet]
        public IActionResult AddDoctor()
        {

            //var userId = userManager.GetUserId(User);
            //if (userId.IsNullOrEmpty())//ceck it is not null
            //{
            //    return RedirectToAction("Index");
            //}
            AddDoctorViewModel addDoctorViewModel = new AddDoctorViewModel();
            addDoctorViewModel.doctor = new Doctor();


            List<SelectListItem> users = userManager.Users
                .OrderBy(m => m.UserName)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.UserName
                }).ToList();

            List<SelectListItem> departments = context.Departments
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                }).ToList();
            addDoctorViewModel.Users = users;
            addDoctorViewModel.Departments = departments;

            return View(addDoctorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(AddDoctorViewModel model)
        {
            
            var user = userManager.GetUserId(User);
            Doctor doctor = new Doctor()
            {
                Name = model.doctor.Name,
                Description = model.doctor.Description,
                DepartmentId = model.doctor.DepartmentId,
                DoctorUserId = model.doctor.DoctorUserId//doesn't return ID //but if change to DoctorUserId = user => return id user in session
            };

            context.Doctors.Add(doctor);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Add Patient

        #endregion

        #region Add Appointment
        [HttpGet]
        public IActionResult AddAppointment()
        {
            //var userId = userManager.GetUserId(User);
            //if (userId.IsNullOrEmpty())//ceck it is not null
            //{
            //    return RedirectToAction("Index");
            //}
            CreateAppointmentViewModel createAppointment = new CreateAppointmentViewModel();
            createAppointment.appointment = new Appointment();



            List<SelectListItem> doctors = context.Doctors
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                }).ToList();
            createAppointment.Doctors = doctors;

            return View(createAppointment);
        }


        public IActionResult AddAppointment(CreateAppointmentViewModel model)
        {
            //var oneReservation = context.Appointments.Where(x => x.DoctorId == model.appointment.DoctorId).Where(d => d.AppointmentDate == model.appointment.AppointmentDate).Where(p => p.PatientId == model.appointment.PatientId).FirstOrDefault();
            //if (oneReservation != null)
            //{
            //    ViewBag.AppointmentNum = "One Reservation per patient with the same doctor on the same day";

            //}
            var countAppointment = context.Appointments.Where(x => x.DoctorId == model.appointment.DoctorId).Where(d => d.AppointmentDate == model.appointment.AppointmentDate).ToList();
            if (countAppointment.Count >= 5)
            {
                //ViewBag.AppointmentNum = "This doctor has reached the maximum appointment limit";

            }
            else
            {
                var user = userManager.GetUserId(User);
                Appointment appointment = new Appointment()
                {
                    AppointmentDate = model.appointment.AppointmentDate,
                    AppointmentTime = model.appointment.AppointmentTime,
                    AppointmentPrice = model.appointment.AppointmentPrice,
                    DoctorId = model.appointment.DoctorId
                };

                context.Appointments.Add(appointment);
                context.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        #endregion

    }
}
