using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIT5032_Project.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "The full name field is required.")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public byte[] ProfilePicturePath { get; set; }

        //[Required(ErrorMessage = "The username field is required.")]
        //public string UserName { get; set; }
        //[Required(ErrorMessage = "The password field is required.")]
        //public string PasswordHash { get; set; }
        //[Required(ErrorMessage = "The email field is required.")]
        //public string Email { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole() : base() { }

    //    public ApplicationRole(string roleName) : base(roleName) { }
    //}

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