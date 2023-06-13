namespace ClinicSystemTest.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
