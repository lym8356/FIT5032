using FIT5032_Project.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User profile picture
        public FileContentResult UserImages()
        {
            if (User.Identity.IsAuthenticated)
            {
                String userId = User.Identity.GetUserId();
                if (userId == null || User.IsInRole("Admin"))
                {
                    string fileName = HttpContext.Server.MapPath(@"~/Image/profile_picture/default.png");

                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);

                    return File(imageData, "image/png");
                }
                var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var userImage = bdUsers.Users.Where(x => x.Id == userId).FirstOrDefault();
                return new FileContentResult(userImage.ProfilePicturePath, "image/jpeg");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/Image/profile_picture/default.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");
            }
        }
    }
}