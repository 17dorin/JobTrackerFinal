using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string Biography { get; set; }
        public string GitHub { get; set; }
        public string LinkedIn { get; set; }
        public bool? IsEmployer { get; set; }
        public bool? IsPrivate { get; set; } 

        public List<Skill> skills { get; set; }

        public ProfileViewModel(AspNetUser user, List<Skill> skills)
        {
            this.UserId = user.Id;
            this.skills = skills;
            this.Email = user.Email;
            this.PhoneNumber = user.PhoneNumber;
            this.Location = user.Location;
            this.Biography = user.Biography;
            this.GitHub = user.GitHub;
            this.LinkedIn = user.LinkedIn;
            this.IsEmployer = user.IsEmployer;
            this.IsPrivate = user.IsPrivate;
        }
    }
}
