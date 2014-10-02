using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Splatter.Controllers
{
    public class UserRolesController : Controller
    {
        public ApplicationDbContext AspDb = new ApplicationDbContext();
        public ApplicationDbContext db = new ApplicationDbContext();
        public BugTracker BtDb = new BugTracker();

        // GET: Assigned User in Roles
        [Authorize(Roles = "Administrator")]
        public ActionResult AssignUsers(string roleId)
        {
            //locate the role in the DB
            var role = AspDb.Roles.Find(roleId);
            
            // instantiate the viewmodel
            var model = new UserRolesViewModel();

            //add role Id and Name to the Model 
            model.RoleId = role.Id;
            model.RoleName = role.Name;
            
            // instantiate the user list that is part of the view model
            List<BTUser> userlist = new List<BTUser>();
            
            // loop through the system users and, as long as the user is NOT already in the role, add the user to the list
            foreach (var user in AspDb.Users)
            {
                if(!user.IsInRole(model.RoleName))
                {
                   var tempuser = BtDb.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.Id);
                   userlist.Add(tempuser);     
                }
            } 
            
            // instiate the MuliSelectList object (in the model) using the newly built list
            // with the appropriate value and display parameters ( Hint: see whiteboard)
            model.Users = new MultiSelectList(userlist.OrderBy(u => u.DisplayName), "AspNetUserId", "DisplayName", null);
                
            // send the model to the view

            return View(model);
        }

        // Post: Assigned User in Roles
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers(UserRolesViewModel model)
        {
           //ApplicationDbContext db = new ApplicationDbContext();
          
            if (ModelState.IsValid )
           {
               if (model.SelectedUsers != null)
               {
                   foreach (string id in model.SelectedUsers)
                   {
                        //locate the user int the database (AspNetUsers)
                        var user = AspDb.Users.Find(id);

                        if (model.RoleName == "Unassigned")
                        {
                            // see if user is in a role  
                            var roles = db.Roles.ToList();
                            foreach (var item in roles)
                            {
                                if (user.IsInRole(item.Name))
                                {
                                    user.RemoveUserFromRole(item.Name);
                                }
                            }
                            // add the user to the role
                            user.AddUserToRole(model.RoleName);
                        }
                        else
                        {
                            // add the user to the role
                            user.AddUserToRole(model.RoleName);
                            // if the user is in the "Unassigned" role, remove them from that role
                            if (user.IsInRole("Unassigned"))
                            {
                                user.RemoveUserFromRole("Unassigned");
                            }
                        }
                   }
               }
               //redirect to the roles list 
               return RedirectToAction("Index", "Roles");
           }
           return View();
        }

        // GET: Users in Role
        [Authorize(Roles = "Administrator")]
        public ActionResult UsersInRole(string roleId)
        {
            //locate the role in the DB
            var role = AspDb.Roles.Find(roleId);

            // instantiate the viewmodel
            var model = new UserRolesViewModel();

            //add role Id and Name to the Model 
            model.RoleId = role.Id;
            model.RoleName = role.Name;

            // instantiate the user list that is part of the view model
            List<BTUser> userlist = new List<BTUser>();

            // loop through the system users and, as long as the user Is already in the role, add the user to the list
            foreach (var user in AspDb.Users)
            {
                if (user.IsInRole(model.RoleName))
                {
                   userlist.Add(BtDb.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.Id));
                }
            }

            // instiate the MuliSelectList object (in the model) using the newly built list
            // with the appropriate value and display parameters ( Hint: see whiteboard)
            model.Users = new MultiSelectList(userlist.OrderBy(u => u.DisplayName), "AspNetUserId", "DisplayName", null);

            // send the model to the view

            return View(model);
        }

        // Post: Unassign Users in Role
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UsersInRole(UserRolesViewModel model)
        {
            //ApplicationDbContext db = new ApplicationDbContext();

            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    foreach (string id in model.SelectedUsers)
                    {
                        //locate the user int the database (AspNetUsers)
                        var user = AspDb.Users.FirstOrDefault(u => u.Id == id);
                        // remove the user from the role
                        user.RemoveUserFromRole(model.RoleName);

                        if (user.Roles.Count == 0)
                        {
                            user.AddUserToRole("Unassigned");
                        }
                        //redirect to the roles list 
                    }
                    return RedirectToAction("Index", "Roles");
                }
             }
            return View();
        }
   
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
