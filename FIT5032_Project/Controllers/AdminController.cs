using FIT5032_Project.Models;
using FIT5032_Project.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        PC_KingdomEntities db = new PC_KingdomEntities();
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Dashboard()
        {
            var userCount = db.AspNetUsers.ToList().Count;
            var productCount = db.Products.ToList().Count;
            ViewBag.userCount = userCount;
            ViewBag.productCount = productCount;
            return View();
        }

        public ActionResult UserIndex(string notification)
        {
            ViewBag.message = notification;
            return View();
        }

        [HttpGet]
        public ActionResult UserAdd()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var role in roleManager.Roles) 
            {
                items.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            }

            var roleList = new SelectList(items, "Value", "Text");
            
            ViewBag.Roles = roleList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAdd([Bind(Exclude = "ProfilePicturePath")] ApplicationUser user)
        {

            UserManager<ApplicationUser> userManager;
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            PasswordHasher hasher = new PasswordHasher();
            string password = hasher.HashPassword(user.PasswordHash);
            var newRole = Request.Form["RoleName"];
            byte[] imageData = null;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase ImageFile = Request.Files["ProfilePicturePath"];
                using (var binary = new BinaryReader(ImageFile.InputStream))
                {
                    imageData = binary.ReadBytes(ImageFile.ContentLength);
                }
            }
            var newUser = new ApplicationUser();
            newUser.ProfilePicturePath = imageData;
            newUser.Email = user.Email;
            newUser.DOB = user.DOB;
            newUser.PhoneNumber = user.PhoneNumber;
            newUser.UserName = user.UserName;
            newUser.FullName = user.FullName;
            newUser.Address = user.Address;
            var result = userManager.Create(newUser, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(newUser.Id, newRole);
                string notification = "User added successfully";
                return RedirectToAction("UserIndex", "Admin", new { message = notification });
            }
            else
            {
                string notification = "An error occured, please try again later";
                return RedirectToAction("UserIndex", "Admin", new { message = notification });
            }

        }

        [HttpGet]
        public ActionResult UserEdit(string id)
        {
            var user = db.AspNetUsers.Where(u => u.Id == id).FirstOrDefault();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var role in roleManager.Roles)
            {
                items.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            }

            var roleList = new SelectList(items, "Value", "Text");

            ViewBag.Roles = roleList;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit([Bind(Exclude = "ProfilePicturePath")] ApplicationUser appuser)
        {

            UserManager<ApplicationUser> userManager;
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var userToUpdate = db.AspNetUsers.Where(u => u.Id == appuser.Id).FirstOrDefault();
            PasswordHasher hasher = new PasswordHasher();
            if(appuser.PasswordHash != null)
            {
                string password = hasher.HashPassword(appuser.PasswordHash);
                userToUpdate.PasswordHash = password;
            }

            string oldRoleName = userManager.GetRoles(appuser.Id).FirstOrDefault();
            string newRoleName = Request.Form["RoleName"];
            if (oldRoleName != newRoleName)
            {
                userManager.RemoveFromRole(appuser.Id, oldRoleName);
                userManager.AddToRole(appuser.Id, newRoleName);
            }

            byte[] imageData = appuser.ProfilePicturePath;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase ImageFile = Request.Files["ProfilePicturePath"];
                using (var binary = new BinaryReader(ImageFile.InputStream))
                {
                    imageData = binary.ReadBytes(ImageFile.ContentLength);
                }
            }

            userToUpdate.ProfilePicturePath = imageData;
            userToUpdate.Email = appuser.Email;
            userToUpdate.DOB = appuser.DOB;
            userToUpdate.PhoneNumber = appuser.PhoneNumber;
            userToUpdate.UserName = appuser.UserName;
            userToUpdate.FullName = appuser.FullName;
            userToUpdate.Address = appuser.Address;
            

            try
            {
                db.SaveChanges();
                string notification = "User profile updated.";
                return RedirectToAction("UserIndex", "Admin", notification);
            }
            catch (DataException)
            {
                string notification = "Unable to save changes. Please try again later.";
                return RedirectToAction("UserIndex", "Admin", notification);
            }
        }

        public ActionResult UserDelete(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser userToRemove = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault<AspNetUser>();
            
            db.AspNetUsers.Remove(userToRemove);
            db.SaveChanges();

            string notification = "User deleted";
            return RedirectToAction("UserIndex", "Admin", new { message = notification });
        }

        public ActionResult GetUserTable()
        {
            var data = db.AspNetUsers.Select(x => new
            {
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FullName,
                Address = x.Address,
                PhoneNumber = x.PhoneNumber,
                DOB = x.DOB,
                UserID = x.Id
            }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send_Email([Bind(Exclude = "Attachment")]SendEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    byte[] attachment = null;
                    String[] toEmailArray = model.ToEmail.Split(',');
                    string filename = null;

                    String subject = model.Subject;
                    String contents = model.Contents;

                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase File = Request.Files["Attachment"];
                        using(var binary = new BinaryReader(File.InputStream))
                        {
                            attachment = binary.ReadBytes(File.ContentLength);
                            filename = Request.Files["Attachment"].FileName;
                        }
                    }

                    emailSender es = new emailSender();
                    es.Send(toEmailArray, subject, contents, attachment, filename);

                    ModelState.Clear();

                    return RedirectToAction("UserIndex");
                }
                catch
                {
                    return RedirectToAction("UserIndex");
                }
            }

            return RedirectToAction("UserIndex");
        }

        public ActionResult Booking_Calendar()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            using (PC_KingdomEntities db = new PC_KingdomEntities())
            {
                var events = db.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            var status = false;
            using (PC_KingdomEntities db = new PC_KingdomEntities())
            {
                if(e.eventID > 0)
                {
                    //update the event
                    var eventToUpdate = db.Events.Where(x => x.eventID == e.eventID).FirstOrDefault();
                    if(eventToUpdate != null)
                    {
                        eventToUpdate.subject = e.subject;
                        eventToUpdate.startDate = e.startDate;
                        eventToUpdate.endDate = e.endDate;
                        eventToUpdate.description = e.description;
                        eventToUpdate.themeColor = e.themeColor;
                        eventToUpdate.isFullDay = e.isFullDay;

                    }
                }
                else
                {
                    //add an event
                    db.Events.Add(e);
                }

                db.SaveChanges();
                status = true;
            }
                return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (PC_KingdomEntities db = new PC_KingdomEntities())
            {
                var eventToDelete = db.Events.Where(x => x.eventID == eventID).FirstOrDefault();
                if (eventToDelete != null)
                {
                    db.Events.Remove(eventToDelete);
                    db.SaveChanges();
                    status = true;
                }
            }
                return new JsonResult { Data = new { status = status } };
        }

         public ActionResult GetProductTable()
        {
            var data = db.Products.Select(x => new
            {
                ProductID = x.productId,
                ProductName = x.productName,
                ProductDescription = x.productDescription,
                ProductPrice = x.productPrice,
                ProductCategory = x.productCategory,
                //ProductImage = x.productImage,
                //ProductRating = x.productRating

            }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductIndex()
        {
            return View();
        }

        public ActionResult ProductAdd() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductAdd([Bind(Exclude = "productImage")]Product product)
        {

            byte[] imageData = null;
            System.Diagnostics.Debug.WriteLine(Request.Files.Count);
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase ImageFile = Request.Files[0];
                using (var binary = new BinaryReader(ImageFile.InputStream))
                {
                    imageData = binary.ReadBytes(ImageFile.ContentLength);
                }
            }
            var newProduct = new Product();
            newProduct.productImage = imageData;
            newProduct.productName = product.productName;
            newProduct.productDescription = product.productDescription;
            newProduct.productCategory = product.productCategory;
            newProduct.productPrice = product.productPrice;
            newProduct.productRating = product.productRating;
            var result = db.Products.Add(newProduct);

            try
            {
                db.SaveChanges();
                string notification = "Product added successfully.";
                return RedirectToAction("ProductIndex", "Admin");
            }
            catch (DataException)
            {
                string notification = "Unable to add product. Please try again later.";
                return RedirectToAction("ProductIndex", "Admin");
            }
        }

        public ActionResult ProductEdit(int id)
        {
            var productToUpdate = db.Products.Where(x => x.productId == id).FirstOrDefault();
            return View(productToUpdate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult ProductEdit([Bind(Exclude = "productImage")]Product product)
        {
            var productToUpdate = db.Products.Where(x => x.productId == product.productId).FirstOrDefault();
            byte[] imageData = product.productImage;
            if (Request.Files.Count > 0)
            {
                if (Request.Files[0].ContentLength > 0) {
                    HttpPostedFileBase ImageFile = Request.Files[0];
                    using (var binary = new BinaryReader(ImageFile.InputStream))
                    {
                        imageData = binary.ReadBytes(ImageFile.ContentLength);
                    }
                }
               
            }
            productToUpdate.productImage = imageData;
            productToUpdate.productName = product.productName;
            productToUpdate.productCategory = product.productCategory;
            productToUpdate.productDescription = product.productDescription;
            productToUpdate.productPrice = product.productPrice;
            productToUpdate.productRating = product.productRating;
            
            try
            {
                db.SaveChanges();
                string notification = "Product updated.";
                return RedirectToAction("ProductIndex", "Admin");
            }
            catch (DataException)
            {
                string notification = "Unable to save changes. Please try again later.";
                return RedirectToAction("ProductIndex");
            }
        }

        public FileContentResult ProductImages(int product_id)
        {
            var product = db.Products.Where(x => x.productId == product_id).FirstOrDefault();
            if(product.productImage == null)
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
            return new FileContentResult(product.productImage, "image/jpeg");
        }

         public ActionResult ProductDelete(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product productToRemove = db.Products.Where(x => x.productId == id).FirstOrDefault();
            db.Products.Remove(productToRemove);

            try
            {
                db.SaveChanges();
                return RedirectToAction("ProductIndex", "Admin");
            }
            catch (DataException)
            {
                return RedirectToAction("ProductIndex", "Admin");
            }

        }
       
    } 
}