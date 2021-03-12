using System;
using System.Collections.Generic;

#nullable disable

namespace FinalProject.Models
{
    public partial class UserSkill
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? SkillId { get; set; }

        public virtual Skill Skill { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
