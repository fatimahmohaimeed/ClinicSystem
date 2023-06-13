using Microsoft.Build.Framework;

namespace ClinicSystemTest.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}

