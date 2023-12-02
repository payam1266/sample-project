using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using BlackSwanPlat.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using NuGet.Protocol;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;

namespace BlackSwanPlat.Controllers
{
    public class HomeController : Controller
    {
        public readonly DbBlack db;
        public HomeController(DbBlack _db)
        {
            db = _db;
        }
        public IActionResult Index(string userId)
        {
            db.Database.EnsureCreated();
            var topProducts = db.OrderDetails
                 .GroupBy(x => x.productId)
                 .Select(y => new
                 {
                     ProductId = y.Key,
                     SalesCount = y.Sum(x => x.quantity)
                 })
                 .OrderByDescending(p => p.SalesCount)
            .Take(3)
                 .ToList();

            var products = new List<Product>();

            foreach (var item in topProducts)
            {
                var product = db.products.Include(x => x.productImages).Include(x => x.productCategory).Include(x => x.brand).FirstOrDefault(p => p.id == item.ProductId);
                if (product != null)
                {
                    products.Add(product);
                }
            }



            return View(products);
        }

        public async Task<IActionResult> ProductPage()
        {
            var p = await db.products.Include(x => x.brand).Include(x => x.city).Include(x => x.productCategory).Include(x => x.productImages).ToListAsync();
            return View(p);
        }


        public async Task<IActionResult> ProductDetails(int productid, int cateid)
        {


            ViewData["relproduct"] = db.products.Include(x => x.productImages).Where(x => x.productCategoryId == cateid).Include(x => x.brand).ToList();
            var product = db.products.Include(x => x.productCategory).Include(x => x.productImages).Include(x => x.brand).Include(x => x.seller).Where(x => x.id == productid).ToList();


            if (product == null)
            {
                return RedirectToAction("Error", "Error", new { statusCode = 404 });
            }


            return View(product);
        }
        public IActionResult AbouetMe()
        {
            return View();
        }

        public IActionResult GetProductShoppedAndLiked(string userId)
        {
            var shopedProductId = db.orders
                .Where(x => x.customerid == userId)
                .SelectMany(x => x.orderDetails)
                .Select(z => z.productId)
                .ToList();

            var likedProductId = db.likes
                .Where(l => l.UserId == userId)
                .Select(l => l.productid)
                .ToList();

            var products2 = db.products
                .Where(p => shopedProductId.Contains(p.id) || likedProductId.Contains(p.id))
                .Include(p => p.productImages)
                .Select(p => new ProductDTO
                {
                    Id = p.id,
                    Name = p.name,
                    Brand = p.brand.name,
                    price = p.price,
                    size = p.size,
                    color = p.color,
                    description = p.description,
                    Images = p.productImages.Take(1).ToList()

                })
                .ToList();

            return Json(products2);
        }


        public IActionResult GetProductsLikedByUser(string userId)
        {


            var likedProductId = db.likes
                .Where(l => l.UserId == userId)
                .Select(l => l.productid)
                .ToList();

            var products = db.products
                .Where(p => likedProductId.Contains(p.id))
                .Include(p => p.productImages)
                .Select(p => new ProductDTO
                {
                    Id = p.id,
                    Name = p.name,
                    Brand = p.brand.name,
                    price = p.price,
                    size = p.size,
                    color = p.color,
                    description = p.description,
                    Images = p.productImages.Take(1).ToList()

                })
                .ToList();

            return Json(products);
        }



        public void DiscountProducSpecialDay()
        {
            DateTime currentDate = DateTime.Now;


            DateTime startDate = new DateTime(currentDate.Year, 12, 24);
            DateTime endDate = new DateTime(currentDate.Year, 12, 26);


            if (currentDate >= startDate && currentDate <= endDate)
            {
                var p = db.products.ToList();
                foreach (var item in p)
                {
                    item.price -= 20;
                }
                db.Update(p);
                db.SaveChanges();
            }
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult NewArtist()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult LoadLike(int id)
        {
            int likeCount = db.likes.Count(x => x.productid == id);
            return Json(likeCount);
        }
      
    }
}