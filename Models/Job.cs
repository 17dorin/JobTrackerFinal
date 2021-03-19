using System;
using System.Collections.Generic;
using FinalProject.Helpers;

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

        public static Job ToJob(Result raw)
        {
            Job j = new Job();
            j.Company = TextHelper.RemoveHTML(raw.company.display_name);
            j.Position = TextHelper.RemoveHTML(raw.title);
            j.Contact = "None";
            j.Method = "Through Job Ad";
            j.DateOfApplication = DateTime.Now;
            j.Link = raw.redirect_url;
            j.FollowUp = null;
            j.CompanySite = null;
            j.Responded = false;
            j.Notes = "None";

            return j;

        }
        public virtual AspNetUser User { get; set; }
    }

    public class Rootobject
    {
        public float mean { get; set; }
        public Result[] results { get; set; }
        public int count { get; set; }
        public string __CLASS__ { get; set; }
    }

    public class Result
    {
        public Location location { get; set; }
        public string title { get; set; }
        public DateTime created { get; set; }
        public Category category { get; set; }
        public string adref { get; set; }
        public Company company { get; set; }
        public string id { get; set; }
        public string __CLASS__ { get; set; }
        public float longitude { get; set; }
        public string salary_is_predicted { get; set; }
        public string description { get; set; }
        public float latitude { get; set; }
        public string redirect_url { get; set; }
        public string contract_time { get; set; }
    }

    public class Location
    {
        public string __CLASS__ { get; set; }
        public string[] area { get; set; }
        public string display_name { get; set; }
    }

    public class Category
    {
        public string tag { get; set; }
        public string label { get; set; }
        public string __CLASS__ { get; set; }
    }

    public class Company
    {
        public string __CLASS__ { get; set; }
        public string display_name { get; set; }
    }



}
