using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Splatter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private UserManager<ApplicationUser> userManager = 
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));

        public bool AddUserToRole(string roleName)
        {
            var idresult = userManager.AddToRole(this.Id, roleName);
            return idresult.Succeeded;
        }


        public bool RemoveUserFromRole(string roleName)
        {
            var idresult = userManager.RemoveFromRole(this.Id, roleName);
            return idresult.Succeeded;
        }

        public bool IsInRole(string roleName)
        {
           var result = userManager.IsInRole(this.Id, roleName);
           return result;
        }

        public string IsUser(string userId, string Email)
        {
            var idresult = userManager.FindById(userId);
            if (idresult.Email != null)
            {
                return Email = idresult.Email;
            }
            return null;
        }
       
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}