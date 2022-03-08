using AssignmentMVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssignmentMVC.Data
{
    public class MyIdentityDbContext: IdentityDbContext<User>
    {
        public MyIdentityDbContext() : base("ConnectionStringAssignment")
        {

        }
    }
}