using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string Name { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Your last name has too many characters", MinimumLength = 6)]
        public string Surname { get; set; }
        [Required]
        [Range(16, 75, ErrorMessage = "You do not fit to age")]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string Country { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string City { get; set; }
    }
}
