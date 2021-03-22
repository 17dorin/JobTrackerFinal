using System;
using System.Collections.Generic;

#nullable disable

namespace FinalProject.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
