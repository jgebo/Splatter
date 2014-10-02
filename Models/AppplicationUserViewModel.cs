using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Splatter.Models
{
    public class AspNetRolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AspNetUserRolesViewModel
    {

        public string UserName { get; set; }
        public string Id { get; set; }
        public int UserId { get; set; }
        public string RolesId { get; set; }
        public string Name { get; set; }
        public System.Web.Mvc.MultiSelectList User { get; set; }
        public string SelectedUser { get; set; }
    }

    //public class RemoveUserViewModel
    //{
    //    public string IsActive { get; set; }
    //}
}