using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using BlackSwanPlat.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using NuGet.Versioning;
using System;
using System.Runtime.InteropServices;
using static BlackSwanPlat.Areas.Identity.Data.Order;

namespace BlackSwanPlat.Controllers
{

    public class ShoppingCard : Controller
    {


        public readonly DbBlack db;

        public ShoppingCard(DbBlack _db)
        {
            db = _db;
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(CardItem p)
        {
            CardItem card = new CardItem();
            card.userid = p.userid;
            card.productprice = p.productprice;
            card.productid = p.productid;

            card.productcount = p.productcount;
            card.productname = p.productname;
            db.Add(card);
            await db.SaveChangesAsync();

            return Json("Add Successful");
        }

        public async Task<IActionResult> ListOfCard([FromServices] UserManager<ApplicationUser> userManager)
        {
            var topCustomers = db.orders
               .GroupBy(x => x.customerid)
               .Select(g => new
               {
                   UserId = g.Key,
                   TotalAmount = g.Sum(x => x.totalPrice)
               })
               .OrderByDescending(c => c.TotalAmount)
               .Take(3)
               .ToList();

            var top = new List<ApplicationUser>();

            foreach (var item in topCustomers)
            {
                var user = await userManager.FindByIdAsync(item.UserId);
                if (user != null)
                {
                    top.Add(user);
                }
            }
            ViewData["top"] = top;

            ViewData["dis"] = db.dis.ToList();


            var card = await db.cardItems.ToListAsync();
            return View(card);
        }

        public async Task<IActionResult> UpdateCard(CardItem model)
        {
            var check = 0;
            var k = db.products.Where(x => x.id == model.productid).ToList();
            k.ForEach(x =>
            {
                check = x.count;
            });


            if (check >= model.productcount)
            {
                var p = db.cardItems.Find(model.id);
                if (p != null)
                {
                    var discountAmount = CalculateDiscount(model.dis, model.productprice);

                    p.productname = model.productname;
                    if (discountAmount == 0)
                    {
                        p.productprice = model.productprice;
                    }
                    else
                    {
                        p.productprice = discountAmount;
                    }

                    p.userid = model.userid;
                    p.productcount = model.productcount;
                    p.productid = model.productid;

                    p.dis = model.dis;
                    db.Update(p);
                }

                await db.SaveChangesAsync();
                return Json("1");

            }
            else
            {
                return Json("0");
            }

        }
        public IActionResult DeleteCard(int id)
        {

            var p = db.cardItems.Find(id);
            if (p != null)
            {

                db.Remove(p);
                db.SaveChanges();
            }


            return Json("delete successful");


        }


        private float CalculateDiscount(string discountCode, float originalPrice)
        {

            var discount = db.dis.FirstOrDefault(x => x.code == discountCode && x.isactive == false);



            if (discount != null)
            {

                var discountAmount = originalPrice - 5;


                discount.isactive = true;
                db.SaveChanges();




                return discountAmount;
            }


            return 0;
        }
        public async Task<IActionResult> OrderConfirm()
        {
            ViewData["listcard"] = await db.cardItems.ToListAsync();
            return View();
        }


        public async Task<IActionResult> SubmitOrder(Order model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        fullname = model.fullname,
                        phone = model.phone,
                        address = model.address,
                        city = model.city,
                        postalcode = model.postalcode,
                        dateTime = DateTime.Now,
                        customerid = model.customerid,
                        paymentmethod = model.paymentmethod,
                        status = Order.OrderStatus.PendingPayment
                    };

                    db.orders.Add(order);
                    await db.SaveChangesAsync();

                    float totalPrice = 0;
                    foreach (var cardItem in db.cardItems.Where(x => x.userid == model.customerid))
                    {
                        var orderDetails = new OrderDetails
                        {
                            productId = cardItem.productid,
                            productName = cardItem.productname,
                            quantity = cardItem.productcount,
                            productPrice = cardItem.productprice,
                            orderId = order.id,
                            subtotal = cardItem.productprice * cardItem.productcount
                        };
                        totalPrice += orderDetails.subtotal;
                        db.OrderDetails.Add(orderDetails);
                    }

                    order.totalPrice = totalPrice;
                    await db.SaveChangesAsync();

                    transaction.Commit();
                    return RedirectToAction("FinalizeOrder");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return RedirectToAction("ListOfCard");
                }
            }
        }

        public async Task<IActionResult> FinalizeOrder()
        {

            ViewData["order"] = db.orders.ToList();
            var p = await db.OrderDetails.Include(x => x.Order).ToListAsync();

            return View(p);
        }
        public async Task<IActionResult> DeleteOrderAndDetails(int id)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {

                    var ord = db.OrderDetails.Where(x => x.orderId == id).ToList();
                    foreach (var item in ord)
                    {
                        db.OrderDetails.Remove(item);
                    }
                    db.SaveChanges();


                    var order = db.orders.FirstOrDefault(x => x.id == id);
                    if (order != null)
                    {
                        db.orders.Remove(order);
                        db.SaveChanges();
                    }

                    trans.Commit();
                    return Json("1");
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return Json("Error In Delete!");
                }
            }
        }

        public IActionResult FinalizePayment(int id)
        {
            var order = db.orders.Include(x => x.orderDetails).SingleOrDefault(x => x.id == id);

            if (order != null)
            {
                foreach (var item in order.orderDetails)
                {
                    var product = db.products.SingleOrDefault(p => p.id == item.productId);

                    if (product != null)
                    {

                        if (product.count >= item.quantity)
                        {

                            product.count -= item.quantity;

                            db.SaveChanges();
                        }
                        else
                        {


                            return Json("1");
                        }
                    }
                    else
                    {


                        return Json("2");
                    }
                }

                order.status = Order.OrderStatus.Paid;
                order.dateTime = DateTime.Now;
                db.Update(order);
                db.SaveChanges();


                return Json("3");
            }
            else
            {
                return Json("4");

            }
        }
        public async Task<IActionResult> GetOrderUser(string userid)
        {
            var p = await db.orders.Where(x => x.customerid == userid && x.status == Order.OrderStatus.Paid).ToListAsync();
            return Json(p);
        }
        public async Task<IActionResult> GetOrderUserDetails(int id)
        {
            var p = await db.OrderDetails.Where(x => x.orderId == id).ToListAsync();
            return Json(p);
        }

        public IActionResult AddComment(string text, string userId, int ProductId)
        {
            try
            {
                Comment c = new Comment();

                c.Text = text;
                c.CreatedAt = DateTime.Now;
                c.UserId = userId;
                c.ProductId = ProductId;


                db.Comments.Add(c);
                db.SaveChanges();


                return Content("success");
            }
            catch (Exception)
            {

                return Content("error");
            }
        }
        public IActionResult AddLike(string userId, int ProductId)
        {

            bool Check = db.likes.Any(x => x.UserId == userId && x.productid == ProductId);

            if (Check)
            {

                return Json("You've already liked this product.");
            }

            else
            {

                Like like = new Like();
                like.productid = ProductId;
                like.UserId = userId;
                db.likes.Add(like);
                db.SaveChanges();

                return Json("Like Added");
            }
        }
        public IActionResult Deletelike(string userId, int ProductId)
        {
            var p = db.likes.FirstOrDefault(x => x.UserId == userId && x.productid == ProductId);
            db.likes.Remove(p);
            db.SaveChanges();

            return Json("Like deleted");
        }

        public IActionResult LoadComments(int productId)
        {
            var comments = db.Comments.Where(x => x.ProductId == productId).ToList();

            if (comments != null && comments.Count > 0)
            {
                return Json(comments);
            }

            return Json("0");
        }

        public IActionResult CancelOrder(int id)
        {
            var p = db.orders.Find(id);
            if (p == null)
            {
                return Json("0");
            }
            else
            {
                p.status = Order.OrderStatus.Canceled;
                db.Update(p);
                db.SaveChanges();
            }
            return Json("1");
        }









        public IActionResult FinalizePaymentTest(int id)
        {
            var order = db.orders.Include(x => x.orderDetails).SingleOrDefault(x => x.id == id);

            if (order == null)
            {
                return Json("Order Not Found.");
            }

            bool CheckPey = true;


            foreach (var item in order.orderDetails)
            {
                var product = db.products.SingleOrDefault(p => p.id == item.productId);

                if (product != null)
                {
                    if (product.count >= item.quantity)
                    {
                        product.count -= item.quantity;
                    }
                    else
                    {
                        CheckPey = false;
                        return Json($"Not Enough Quantity for Product: {product.name}");
                    }
                }
                else
                {
                    CheckPey = false;
                    return Json("Product Not Found.");
                }
            }

            if (CheckPey)
            {
                var payment = new ZarinpalSandbox.Payment((int)order.totalPrice);
                var res = payment.PaymentRequest("Paying Order", $"https://localhost:44377/ShoppingCard/FinalizeOrder/OnlinePayment/{order.id}");

                if (res.Result.Status == 100)
                {
                    return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{res.Result.Authority}");
                }
                else
                {

                    return Json("Error In Payment Request.");
                }
            }

            else
            {

                return Json("Payment Not Successful.");
            }

        }

        public IActionResult OnlinePayment(int id)
        {
            if (!HttpContext.Request.Query.ContainsKey("status") || !HttpContext.Request.Query.ContainsKey("Authority"))
            {
                ViewData["success"] = false;
                return View();
            }

            string status = HttpContext.Request.Query["status"].ToString().ToLower();
            string authority = HttpContext.Request.Query["Authority"];

            if (status != "ok" || string.IsNullOrWhiteSpace(authority))
            {
                ViewData["success"] = false;
                return View();
            }

            var order = GetOrderByOrderId(id);

            if (order == null)
            {
                ViewData["success"] = false;
                return View();
            }

            var payment = new ZarinpalSandbox.Payment((int)order.totalPrice);
            var res = payment.Verification(authority).Result;

            if (res.Status == 100)
            {
                ViewData["success"] = true;
                ViewData["code"] = res.RefId;

                order.status = OrderStatus.Paid;
                db.Update(order);
                db.SaveChanges();

                return View();
            }
            else
            {
                ViewData["success"] = false;
            }

            return View();
        }

        public Order GetOrderByOrderId(int id)
        {
            var p = db.orders.Find(id);
            return p;
        }



    }

}
