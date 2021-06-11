using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ReportAspNetCore.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }
}
