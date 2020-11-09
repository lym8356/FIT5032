using FIT5032_Project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FIT5032_Project.Controllers
{
    public class HomeController : Controller
    {
        PC_KingdomEntities db = new PC_KingdomEntities();
        public ActionResult Index()
        {
            var productList = db.Products.ToList();
            return View(productList);
        }

        public ActionResult ProductPage(int id)
        {
            var product = db.Products.Where(x => x.productId == id).FirstOrDefault();
            return View(product);
        }

        public ActionResult GetProductReview(int product_id)
        {
            var reviewList = db.Comments.Where(x => x.productId == product_id).ToList();
            ViewBag.ProductId = product_id;
            return View(reviewList);
        }

        [Authorize]
        public ActionResult AddComment(int product_id, int rating, string product_comment)
        {
            Comment newComment = new Comment();
            newComment.commentDate = DateTime.Now;
            newComment.productId = product_id;
            newComment.commentDesc = product_comment;
            newComment.rating = rating;
            newComment.userId = User.Identity.GetUserId();

            db.Comments.Add(newComment);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DataException) {
                System.Diagnostics.Debug.WriteLine(product_id);
                System.Diagnostics.Debug.WriteLine(newComment.userId);

                return RedirectToAction("Index");
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Location()
        {
            return View();
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

    }
}