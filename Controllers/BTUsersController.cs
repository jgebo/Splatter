using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Splatter.Views.BTUsers
{
    public class BTUsersController : Controller
    {
        private BugTracker db = new BugTracker();
      
        // GET: BTUsers
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            List<BTUser> btUser = new List<BTUser>();
            //List<BTUserViewModel> btUser = new List<BTUserViewModel>();
            var temp = db.BTUsers.ToList().OrderBy(u => u.DisplayName);

            foreach (var item in temp)
            {
                  //btUser.Add(new BTUserViewModel
                btUser.Add(new BTUser
                {
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    DisplayName = item.DisplayName,
                    AspNetUserId = item.AspNetUserId
                });
            }
            return View(btUser);
        }

        // GET: BTUsers/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
                var u = db.BTUsers.FirstOrDefault(a => a.AspNetUserId == id);
                //BTUserViewModel b = new BTUserViewModel();  
            BTUser b = new BTUser();
                b.UserName = u.UserName;
                b.FirstName = u.FirstName;
                b.LastName = u.LastName;
                b.DisplayName = u.DisplayName;
                b.AspNetUserId = u.AspNetUserId;
                return View(b);
        }

        // POST: BTUsers/Edit/5
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,FirstName,LastName,DisplayName,AspNetUserId")] BTUser bTUser)
        {
            if (ModelState.IsValid)
            {
                if (bTUser.FirstName == null || (bTUser.FirstName.Trim().Length == 0))
                {
                  ModelState.AddModelError("", "First Name Required!");
                }
                if (bTUser.LastName == null || (bTUser.LastName.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Last Name Required!");
                }
                if (bTUser.DisplayName == null || (bTUser.DisplayName.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Display Name Required!");
                }
               
                if(ModelState.IsValid)
                {
                    db.Entry(bTUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(bTUser);
        }

        // GET: BTUsers/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
           var u = db.BTUsers.FirstOrDefault(a => a.AspNetUserId == id);
           //BTUserViewModel b = new BTUserViewModel();
            BTUser b = new BTUser();
           b.UserName = u.UserName;
           b.FirstName = u.FirstName;
           b.LastName = u.LastName;
           b.DisplayName = u.DisplayName;
           b.AspNetUserId = u.AspNetUserId;
           return View(b);
        }

        // POST: BTUsers/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(string id)
       
        {
            {
                var u = db.BTUsers.FirstOrDefault(a => a.AspNetUserId == id);
                db.BTUsers.Remove(u);
                db.SaveChanges();

                ApplicationDbContext adb = new ApplicationDbContext();
                var user = adb.Users.Find(id);
                // tell the DB the role entry has been modified
                if (user != null)
                {
                    adb.Users.Remove(user);
                    // save the changes
                    adb.SaveChanges();
                } 
              

                return RedirectToAction("Index");
            }


         //public ActionResult Delete(BTUser BtUsers)
            //BTUser b = new BTUser();
            //db.Entry(BTUsers).State = EntityState.Modified;
            //db.SaveChanges();
            //return RedirectToAction("Index");

            //var UserManager = new UserManager<IdentityUser>(new RoleStore<IdentityUser>());

            ////IdentityResult roleResult;
            //try
            //{
            //    //roleResult = RoleManager.Delete(new IdentityRole(id));
            //    UserManager.Delete(new IdentityUser(model.Name));
            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}




            //var u = db.BTUsers.FirstOrDefault(a => a.AspNetUserId == id);
           
            ////BTUser b = db.BTUsers.Find(id);
                       
            //db.BTUsers.Remove(u);
            //db.SaveChanges();
            //return RedirectToAction("Index");


            //var RoleManager = new RoleManager<IdentityUser>(new RoleStore<IdentityRole>());

            ////IdentityResult roleResult;
            //try
            //{
            //    //roleResult = RoleManager.Delete(new IdentityRole(id));
            //    RoleManager.Delete(new IdentityRole(model.Name));
            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //return View();
            //}


              //var user = new ApplicationUser { UserName = id };
              //  var result = UserManager.Delete(user);
              //  if (result.Succeeded)
                //{
                  

                    //BugTracker db = new BugTracker();
                    //BTUser btUser = new BTUser();
                    //btUser.AspNetUserId = user.Id;
                    //btUser.FirstName = FirstName;
                    //btUser.LastName = LastName;
                    //btUser.UserName = Email;

                    //// bucket list item : make display names unique
                    //if (model.DisplayName != null)
                    //{
                    //    btUser.DisplayName = model.DisplayName;
                    //}
                    //else
                    //{
                    //    btUser.DisplayName = model.FirstName + " " + model.LastName;
                    //}
                    //db.BTUsers.Delete(btUser);
                    //db.SaveChanges();
                //}
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
