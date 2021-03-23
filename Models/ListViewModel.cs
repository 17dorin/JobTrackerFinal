using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ListViewModel
    {
        public List<Job> AllJobs { get; set; }
        public List<Job> NeedsResponse { get; set; }
        public List<Job> PastFollowUp { get; set; }
    }
}
