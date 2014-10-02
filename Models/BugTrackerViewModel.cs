using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Splatter.Models
{
    public class BTAssignUserViewModel
    {
        public string Role { get; set; }
        public List <BTUserViewModel> AssignUser;
    }
        
    public class BTUserViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string AspNetUserId { get; set; } 
    }

    public class BugTrackerDBContext : DbContext
    {
        public DbSet<BTUserViewModel> BTUsers { get; set; }
    }
}