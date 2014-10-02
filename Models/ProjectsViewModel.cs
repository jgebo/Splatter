using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Splatter.Models
{
    public class ProjectsViewModel
    {
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public int ProjectId { get; set; }
    }
    public class ProjectUsersViewModel
    {
        public int ProjectId { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = " Project Users")]
        public System.Web.Mvc.MultiSelectList ProjectUsers { get; set; }

        public string[] SelectedUsers { get; set; }

    }
}