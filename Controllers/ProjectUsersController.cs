using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;

namespace Splatter.Controllers
{
    public class ProjectUsersController : Controller
    {
        private BugTracker db = new BugTracker();

        // GET: Assigned User in Projects
        [Authorize(Roles = "Administrator")]
        public ActionResult AssignUsers(int id, string pName)
        {
            ////locate the Project in the DB
            //var project = db.Projects.Find(id);

            // instantiate the viewmodel
            var model = new ProjectUsersViewModel();
            model.ProjectId = id;
            model.ProjectName = pName;
            
            
            // instantiate the user list that is part of the view model
            List<BTUser> btUserlist = new List<BTUser>();
            btUserlist = db.BTUsers.ToList();
           
            List<BTUser> selectBtUser = new List<BTUser>();
           
            // loop through the system users and, as long as the user is NOT already in the role, add the user to the list
            foreach (var user in btUserlist)
            {
                if(!user.IsOnProject(model.ProjectId))
                {
                   var templist = db.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.AspNetUserId);
                   selectBtUser.Add(templist);
                }
            } 
            
            // instiate the MuliSelectList object (in the model) using the newly built list with the appropriate value and display parameters 
            model.ProjectUsers = new MultiSelectList(selectBtUser.OrderBy(u => u.DisplayName), "UserName", "DisplayName", null);
               
            // send the model to the view
            return View(model);
        }

        // Post: Assigned User in Projects
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    Project unAssigned = new Project();
                    unAssigned = db.Projects.FirstOrDefault(u => u.Name == "Unassigned");
                    var projects = db.Projects.ToList();

                    foreach (var user in model.SelectedUsers)
                    {
                        // instantiate the user list that is part of the view model
                        BTUser btuser = new BTUser();
                        // locate the user int the database
                        btuser = db.BTUsers.Find(user);

                        if (model.ProjectId == unAssigned.Id)
                        {
                            // see if user is on a Project  
                            foreach (var item in projects)
                            {
                                if (item.Id != unAssigned.Id)
                                {
                                if (btuser.IsOnProject(item.Id))
                                {
                                   btuser.RemoveUserFromProject(item.Id);
                                }
                            }
                            // add the user to the Project
                            btuser.AddUserToProjects(model.ProjectId);
                        }
                            // add the user to the Project
                            btuser.AddUserToProjects(model.ProjectId);
                        }
                        else
                        {
                            // add the user to the Project
                            btuser.AddUserToProjects(model.ProjectId);
                            //if the user is in the "Unassigned" Project, remove them from that Project
                            if (btuser.IsOnProject(unAssigned.Id))
                            {
                                btuser.RemoveUserFromProject(unAssigned.Id);
                            }
                        }
                    }
                }
                //redirect to the Projects list 
                return RedirectToAction("Index", "Projects");
            }
            return View();
        }


        // GET: Users in Project
        [Authorize(Roles = "Administrator")]
        public ActionResult UsersInProject(int id, string pName)
        {
            ////locate the Project in the DB
            //var project = db.Projects.Find(id);

            // instantiate the viewmodel
            var model = new ProjectUsersViewModel();
            model.ProjectId = id;
            model.ProjectName = pName;
            
            
            // instantiate the user list that is part of the view model
            List<BTUser> btUserlist = new List<BTUser>();
            btUserlist = db.BTUsers.ToList();
           
            List<BTUser> selectBtUser = new List<BTUser>();
           
            // loop through the system users and, as long as the user is NOT already in the role, add the user to the list
            foreach (var user in btUserlist)
            {
                if(user.IsOnProject(model.ProjectId))
                {
                   var templist = db.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.AspNetUserId);
                   selectBtUser.Add(templist);
                }
            } 
            
            // instiate the MuliSelectList object (in the model) using the newly built list with the appropriate value and display parameters 
            model.ProjectUsers = new MultiSelectList(selectBtUser.OrderBy(u => u.DisplayName), "UserName", "DisplayName", null);
          
            // send the model to the view
            return View(model);
        }

        // Post: Assinged User in Projects
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UsersInProject(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    Project unAssigned = new Project();
                    unAssigned = db.Projects.FirstOrDefault(u => u.Name == "Unassigned");
                    var projects = db.Projects.ToList();

                    foreach (var user in model.SelectedUsers)
                    {
                        // instantiate the user list that is part of the view model
                        BTUser btuser = new BTUser();
                        // locate the user int the database
                        btuser = db.BTUsers.Find(user);

                        btuser.RemoveUserFromProject(model.ProjectId);
                        
                        // Set User to Unassigned if NOT on any Projects  
                        bool onProject = false;
                        foreach (var item in projects)
                        {
                            if (btuser.IsOnProject(item.Id))
                            {
                                onProject = true;
                            }
                        }
                        if (!onProject)
                            {
                               btuser.AddUserToProjects(unAssigned.Id);
                            }   
                    }
                }
                //redirect to the Projects list 
                return RedirectToAction("Index", "Projects");
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