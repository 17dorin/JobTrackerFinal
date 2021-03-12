using System;
using System.Collections.Generic;

#nullable disable

namespace FinalProject.Models
{
    public partial class Job
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Contact { get; set; }
        public string Method { get; set; }
        public DateTime? DateOfApplication { get; set; }
        public string Link { get; set; }
        public DateTime? FollowUp { get; set; }
        public string CompanySite { get; set; }
        public bool? Responded { get; set; }
        public string Notes { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
