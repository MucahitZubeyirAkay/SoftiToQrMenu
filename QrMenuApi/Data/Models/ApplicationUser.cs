using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100, MinimumLength = 2)]
        public override string UserName { get; set; } = "";


        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = "";


        [StringLength(50, MinimumLength = 1)]
        public string SurName { get; set; } = "";


        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        public override string Email { get; set; } = "";


        [Phone]
        [StringLength(30)]
        public override string? PhoneNumber { get; set; }


        public DateTime RegisterDate { get; set; }


        public byte StateId { get; set; }

        public int CompanyId { get; set; }
 


        public State? State { get; set; }

        public Company? Company { get; set; }
    }
}
