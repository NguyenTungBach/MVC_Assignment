using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssignmentMVC.Models
{
    public class User : IdentityUser
    {
        public string IdentityCard { get; set; }
        public int Status { get; set; }
    }
}