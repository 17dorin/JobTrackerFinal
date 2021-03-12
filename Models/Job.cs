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

    public class Rootobject
    {
        public Result[] results { get; set; }
        public int count { get; set; }
        public float mean { get; set; }
        public string __CLASS__ { get; set; }
    }

    public class Result
    {
        public string title { get; set; }
        public string contract_time { get; set; }
        public Category category { get; set; }
        public Location location { get; set; }
        public Company company { get; set; }
        public string id { get; set; }
        public string __CLASS__ { get; set; }
        public float longitude { get; set; }
        public string salary_is_predicted { get; set; }
        public string redirect_url { get; set; }
        public DateTime created { get; set; }
        public string adref { get; set; }
        public float latitude { get; set; }
        public string description { get; set; }
    }

    public class Category
    {
        public string __CLASS__ { get; set; }
        public string tag { get; set; }
        public string label { get; set; }
    }

    public class Location
    {
        public string[] area { get; set; }
        public string __CLASS__ { get; set; }
        public string display_name { get; set; }
    }

    public class Company
    {
        public string display_name { get; set; }
        public string __CLASS__ { get; set; }
    }

}
