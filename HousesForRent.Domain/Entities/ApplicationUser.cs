using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Name and surname")]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


