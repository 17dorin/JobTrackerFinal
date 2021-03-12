using System;
using System.Collections.Generic;

#nullable disable

namespace FinalProject.Models
{
    public partial class Skill
    {
        public Skill()
        {
            UserSkills = new HashSet<UserSkill>();
        }

        public int Id { get; set; }
        public string Skill1 { get; set; }

        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
