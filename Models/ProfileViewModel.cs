using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ProfileViewModel
    {
        public AspNetUser user { get; set; }

        public List<Skill> skills { get; set; }

        public ProfileViewModel(AspNetUser user, List<Skill> skills)
        {
            this.skills = skills;
            this.user = user;
        }
    }
}
