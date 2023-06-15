using ClinicSystemTest.Data;
using ClinicSystemTest.Entities;
using ClinicSystemTest.Enum;
using ClinicSystemTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            var appointments = context.Appointments
                .Include(d=> d.Doctor)
                .Include(P=>P.Patient)
                .ToList();

            return View(appointments);
        }


        #region Roles

        #region  CreateRole

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost] // post new role
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
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
            catch(Exception ex)
            {
                return View(ex.Message.ToString());
            }

        }
        #endregion

        #region List users with roles

        //get all users with their roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (userId.IsNullOrEmpty())//ceck it is not null
                {
                    return RedirectToAction("Index");
                }
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
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }

        }
        #endregion

        #region Assigning Role
        [HttpGet]
        //view user and select a role for the user
        public async Task<IActionResult> AssigningRole(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var roles = await roleManager.Roles.ToListAsync();
                var viewModel = new UserRolesViewModel // view user and list all roles that are created
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
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //Post role for user
        public async Task<IActionResult> AssigningRole(UserRolesViewModel model)
        {
            try
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
                return RedirectToAction("GetRoles"); //or  return RedirectToAction(nameof(GetRoles));
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }
        #endregion

        #endregion

        #region Create Department

        [HttpGet]
        public IActionResult ListDepartments()
        {
            var departments = context.Departments.ToList();

            return View(departments);
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateDepartment(Department department)
        {
            try
            {
                context.Departments.Add(department);
                context.SaveChanges();
                return RedirectToAction("ListDepartments");
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }

        }
        #endregion

        #region Doctor

        [HttpGet]
        public IActionResult ListDoctors()
        {
            var doctors = context.Doctors
                .Include(d=>d.Department)
                .Include(u=>u.DoctorUser)
                .ToList();

            return View(doctors);
        }

        #region Add Doctor
        [HttpGet]
        //Add new doctor 
        public IActionResult AddDoctor()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (userId.IsNullOrEmpty())//ceck it is not null
                {
                    return RedirectToAction("Index");
                }
                AddDoctorViewModel addDoctorViewModel = new AddDoctorViewModel();
                addDoctorViewModel.doctor = new Doctor();


                List<SelectListItem> users = userManager.Users //get all users
                    .OrderBy(m => m.UserName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.UserName
                    }).ToList();

                List<SelectListItem> departments = context.Departments //get all departments
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
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(AddDoctorViewModel model)
        {
            try
            {
                var user = userManager.GetUserId(User);
                Doctor doctor = new Doctor()
                {
                    Name = model.doctor.Name,
                    Description = model.doctor.Description,
                    DepartmentId = model.doctor.DepartmentId,
                    DoctorUserId = model.doctor.DoctorUserId
                };

                context.Doctors.Add(doctor);
                context.SaveChanges();
                return RedirectToAction("ListDoctors");
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }
        #endregion
        
        #region Delete Doctor
        public IActionResult DeleteDoctor(int id)
        {
            try
            {
                var doctor = context.Doctors.FirstOrDefault(a => a.Id == id);
                context.Doctors.Remove(doctor);
                context.SaveChanges();
                return RedirectToAction("ListDoctors");
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }
        #endregion

        #endregion

        #region Add Appointment
        [HttpGet]
        public IActionResult AddAppointment()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (userId.IsNullOrEmpty())//ceck it is not null
                {
                    return RedirectToAction("Index");
                }
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
            catch (Exception ex)
            {
                //ViewBag.Error = "هناك خطأ ما";
                return View(ex.Message.ToString());
            }

        }

        [HttpPost]
        public IActionResult AddAppointment(CreateAppointmentViewModel model)
        {
            try
            {
                var output = new SystemOutput();
                var countAppointment = context.Appointments //Check whether the doctor has more than five appointments a day
                    .Where(x => x.DoctorId == model.appointment.DoctorId)
                    .Where(d => d.AppointmentDate == model.appointment.AppointmentDate)
                    .ToList();
                if (countAppointment.Count >= 5)
                {
                    //output.ErrorCode = SystemOutput.ErrorCodes.MoreThanMax;
                    //ViewBag.AppointmentNum = "This doctor has reached the maximum appointment limit";
                    return RedirectToAction("ValdiationErrorr", "Home");

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
                output.ErrorCode = SystemOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(ex.Message.ToString());
            }
        }
        #endregion

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
