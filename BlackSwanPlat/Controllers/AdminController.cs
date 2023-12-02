using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using BlackSwanPlat.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using NuGet.Packaging.Core;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace BlackSwanPlat.Controllers
{
    [Authorize(Policy = ("Adminpolicy"))]
    public class AdminController : Controller
    {
        public readonly DbBlack db;

        public AdminController(DbBlack _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Users()
        {
            List<IdentityRole> Roles = db.Roles.ToList();
            ViewData["Roles"] = Roles;
            return View();
        }


        public async Task<IActionResult> ListOfUsers([FromServices] DbBlack db)
        {
            var users = db.Users.ToList();
            List<UserViewModel> lstusers = new List<UserViewModel>();
            users.ForEach(x =>
            {
                var userroles = db.UserRoles.ToList().Where(y => y.UserId == x.Id).ToList();


                userroles.ForEach(k =>
                {
                    var roles = db.Roles.ToList().Where(z => z.Id == k.RoleId).ToList();
                    roles.ForEach(h =>
                    {
                        UserViewModel userview = new UserViewModel();

                        userview.rolename = h.Name;
                        userview.name = x.firstName;
                        userview.family = x.lastName;
                        userview.roleId = k.RoleId;
                        userview.id = x.Id;

                        userview.email = x.Email;
                        userview.phone = x.PhoneNumber;
                        userview.age = x.age
                        ; userview.city = x.city;


                        lstusers.Add(userview);
                    });
                });
            });
            return Json(lstusers);
        }
        public async Task<IActionResult> EditUsers(string id, [FromServices] DbBlack db, [FromServices] UserManager<ApplicationUser> userManager)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            List<UserViewModel> lstuser = new List<UserViewModel>();
            if (user != null)
            {
                var rolId = db.UserRoles.Where(x => x.UserId == id).ToList();
                rolId.ForEach(z =>
                {
                    var roles = db.Roles.Where(s => s.Id == z.RoleId).ToList();
                    roles.ForEach(d =>
                    {
                        UserViewModel model = new UserViewModel();
                        model.id = user.Id;
                        model.name = user.firstName;
                        model.family = user.lastName;
                        model.phone = user.PhoneNumber;
                        model.age = user.age;
                        model.city = user.city;
                        model.email = user.Email;

                        model.rolename = d.Name;
                        model.roleId = d.Id;
                        lstuser.Add(model);
                    });
                });

            }
            return Json(lstuser);

        }

        public async Task<IActionResult> UpdateUser(UserViewModel model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager, [FromServices] DbBlack db)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user == null)
            {
                return Json("user not find");
            }


            user.firstName = model.name;
            user.lastName = model.family;
            user.Email = model.email;
            user.age = model.age;
            user.PhoneNumber = model.phone;
            user.city = model.city;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {


                return Json("update user not successful");
            }


            var role = await roleManager.FindByIdAsync(model.roleId);

            if (role == null)
            {

                return Json("role not find");
            }

            var currentRoles = await userManager.GetRolesAsync(user);



            var newRole = await roleManager.FindByIdAsync(model.roleId);

            if (newRole == null)
            {
                return Json("role not find");
            }


            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, newRole.Name);


            return Json("Update user successful");
        }
        public async Task<IActionResult> AddRoleToUser(UserViewModel model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {

            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user == null)
            {
                return Json("user not find");
            }


            var role = await roleManager.FindByIdAsync(model.roleId);

            if (role == null)
            {
                return Json("role not find");
            }


            var addToRole = await userManager.AddToRoleAsync(user, role.Name);

            if (addToRole.Succeeded)
            {
                return Json("new role add successful");
            }
            else
            {
                return Json("error in adding user role");
            }
        }


        public async Task<IActionResult> DeleteUser(string id, [FromServices] UserManager<ApplicationUser> usermanager)
        {
            ApplicationUser user = await usermanager.FindByIdAsync(id);
            if (user != null)
            {
                await usermanager.DeleteAsync(user);
                return Json("delete confirm");
            }
            else
            {
                return Json("id not find");
            }
        }

        public async Task<IActionResult> ListOfSellers([FromServices] UserManager<ApplicationUser> userManager)
        {
            var sellers = await userManager.GetUsersInRoleAsync("Sellers");
            var lstSeller = sellers.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "Sellers"
            }).ToList();

            return Json(lstSeller);
        }
        public async Task<IActionResult> ListOfDelivers([FromServices] UserManager<ApplicationUser> userManager)
        {
            var delivers = await userManager.GetUsersInRoleAsync("Delivers");
            var lstDeliver = delivers.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "Delivers"
            }).ToList();

            return Json(lstDeliver);
        }
        public async Task<IActionResult> ListOfCustomers([FromServices] UserManager<ApplicationUser> userManager)
        {
            var customers = await userManager.GetUsersInRoleAsync("Customers");
            var lstCustomer = customers.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "Customers"
            }).ToList();

            return Json(lstCustomer);
        }
        public async Task<IActionResult> ListOfAdmins([FromServices] UserManager<ApplicationUser> userManager)
        {
            var admins = await userManager.GetUsersInRoleAsync("Admins");
            var lstadmin = admins.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "Admins"
            }).ToList();

            return Json(lstadmin);
        }
        public async Task<IActionResult> OrderList()
        {
            var p = await db.orders.ToListAsync();
            return View(p);
        }
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var p = await db.OrderDetails.Where(x => x.orderId == id).ToListAsync();
            return Json(p);
        }
        public async Task<IActionResult> DeleteOrderWithDeyails(int id)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var check = db.orders.FirstOrDefault(x => x.id == id);
                    if (check != null)
                    {
                        if (check.status == Order.OrderStatus.Paid || check.status == Order.OrderStatus.Canceled)
                        {
                            var p = db.OrderDetails.Where(x => x.orderId == id).ToList();
                            foreach (var item in p)
                            {
                                var product = db.products.Where(x => x.id == item.productId).ToList();
                                product.ForEach(x => x.count += item.quantity);

                            }
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            var ord = db.OrderDetails.Where(x => x.orderId == id).ToList();
                            foreach (var item in ord)
                            {
                                db.OrderDetails.Remove(item);
                            }
                            await db.SaveChangesAsync();

                        }
                    }



                    var order = db.orders.FirstOrDefault(x => x.id == id);
                    if (order != null)
                    {
                        db.orders.Remove(order);
                        await db.SaveChangesAsync();
                    }



                    trans.Commit();
                    return Json("Delete success");
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return RedirectToAction("ErrorPage");
                }
            }
        }
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var ord = db.OrderDetails.Include(x => x.Order).FirstOrDefault(x => x.id == id);
            if (ord != null)
            {
                if (ord.Order.status == Order.OrderStatus.Paid)
                {
                    await db.products.Where(x => x.id == ord.productId).ForEachAsync(y =>
                     {
                         y.count += ord.quantity;
                     });

                    db.SaveChanges();
                }
                db.Remove(ord);
                db.SaveChanges();
                return Json("Delete Successful");
            }
            else
            {
                return Json("Not Find");
            }

        }
        public async Task<IActionResult> UpdateOrder(int id, OrderViewModel updatedData)
        {
            var p = db.orders.FirstOrDefault(x => x.id == id);
            if (p != null)
            {




                p.paymentmethod = updatedData.paymentmethod;
                p.phone = updatedData.phone;
                p.address = updatedData.address;
                p.status = updatedData.status;
                p.fullname = updatedData.fullname;
                p.city = updatedData.city;
                p.dateTime = DateTime.Now;
                p.totalPrice = updatedData.totalPrice;
                p.postalcode = updatedData.postalcode;
                db.Update(p);
                await db.SaveChangesAsync();
                return Json("update successful");

            }
            else
            {
                return Json("Error for Update");
            }

        }


        public IActionResult UpdateOrderDetails(int id, OrderDetailsViewModel updatedDetail)
        {

            if (updatedDetail.quantity < 0)
            {
                return Json("Quantity can not be negative");
            }


            var p = db.OrderDetails.Include(x => x.Order).FirstOrDefault(x => x.id == id && x.Order.status == Order.OrderStatus.Paid);
            if (p != null)
            {

                var product = db.products.FirstOrDefault(y => y.id == p.productId);
                if (product != null)
                {
                    if (updatedDetail.quantity > product.count)
                    {
                        return Json("Not Enough Quantity");
                    }



                    if (updatedDetail.quantity > p.quantity)
                    {
                        product.count -= (updatedDetail.quantity - p.quantity);
                    }
                    else if (updatedDetail.quantity < p.quantity)
                    {
                        product.count += (p.quantity - updatedDetail.quantity);
                    }

                    db.SaveChanges();

                }

            }

            var g = db.OrderDetails.FirstOrDefault(x => x.id == id);
            if (g != null)
            {
                g.productName = updatedDetail.productName;
                g.productPrice = updatedDetail.productPrice;
                g.subtotal = updatedDetail.subtotal;
                g.quantity = updatedDetail.quantity;

                db.Update(g);
                db.SaveChanges();

                return Json("update successful");

            }

            else
            {
                return Json("Order details not find");
            }


        }
        public IActionResult GetTotalOrder()
        {

            var totalorder = db.orders.Count();

            return Json(totalorder);

        }
        public IActionResult GetTotalAmount()
        {
            var totalamount = db.orders.Sum(x => x.totalPrice);

            return Json(totalamount);

        }
        public IActionResult GetOrderPaid()
        {
            var totalSales = db.orders.Where(x => x.status == Order.OrderStatus.Paid).Sum(x => x.totalPrice);

            return Json(totalSales);
        }

        public IActionResult GetTotalSaleProduct()
        {
            var totalprsales = db.OrderDetails.Include(x => x.Order).Where(x => x.Order.status == Order.OrderStatus.Paid).Sum(x => x.quantity);



            return Json(totalprsales);
        }



        public IActionResult GetPopularProduct(int num)
        {

            var topProducts = db.OrderDetails
                .GroupBy(x => x.productId)
                .Select(z => new
                {
                    ProductId = z.Key,
                    SalesCount = z.Sum(x => x.quantity)
                })
                .OrderByDescending(p => p.SalesCount)
                .Take(num)
                .ToList();

            var products = new List<Product>();

            foreach (var item in topProducts)
            {
                var product = db.products.FirstOrDefault(p => p.id == item.ProductId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            return Json(products);
        }




        public IActionResult GetPopularCategory()
        {
            var popCategory = db.OrderDetails
                .GroupBy(x => x.product.productCategory.name)
                .Select(z => new
                {
                    categoryName = z.Key,
                    SalesCount = z.Sum(x => x.quantity)
                })
                .OrderByDescending(p => p.SalesCount)
                .FirstOrDefault();

            if (popCategory != null)
            {
                return Json(popCategory.categoryName);
            }
            else
            {
                return Json("Not found");
            }
        }


        public IActionResult GetPopularBrand()
        {
            var popBrand = db.OrderDetails
                .GroupBy(x => x.product.brand.name)
                .Select(z => new
                {
                    BrandName = z.Key,
                    SalesCount = z.Sum(x => x.quantity)
                })
                .OrderByDescending(p => p.SalesCount)
                .FirstOrDefault();

            if (popBrand != null)
            {
                return Json(popBrand.BrandName);
            }
            else
            {
                return Json("Not found");
            }
        }





        public async Task<IActionResult> GetTopCustomer([FromServices] UserManager<ApplicationUser> userManager, int num)
        {
            var topCustomers = db.orders
                .GroupBy(z => z.customerid)
                .Select(x => new
                {
                    UserId = x.Key,
                    TotalAmount = x.Sum(z => z.totalPrice)
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(num)
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

            return Json(top);
        }


        public IActionResult OrderDetailsTopCustomer(string userid)
        {
            var p = db.OrderDetails.Where(x => x.Order.customerid == userid).ToList();

            return Json(p);
        }


        public IActionResult GetTotalProduct()
        {
            var p = db.products.Sum(x => x.count);

            return Json(p);

        }




        public IActionResult CityInsert()
        {

            return View();
        }
        public IActionResult BrandActiont()
        {

            return View();
        }

        public IActionResult BrandInsertConfirm(BrandViewModel model)
        {
            Brand brand = new Brand();
            brand.id = model.id;
            brand.date = model.date;
            brand.name = model.name;
            brand.description = model.description;
            db.Add(brand);
            db.SaveChanges();
            return Json("Insert confirm");

        }
        public async Task<IActionResult> BrandSelect()
        {
            var p = await db.brands.ToListAsync();
            return Json(p);
        }
        public async Task<IActionResult> BrandDelete(int id)
        {

            var p = db.brands.Find(id);
            if (p != null)
            {
                db.Remove(p);
                await db.SaveChangesAsync();
                return Json("delete confirm");
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public IActionResult BrandEdit(int id)
        {

            var p = db.brands.Find(id);
            if (p != null)
            {

                return Json(p);
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public async Task<IActionResult> BrandUpdate(BrandViewModel model)
        {

            var p = db.brands.Find(model.id);
            if (p != null)
            {

                p.name = model.name;
                p.date = model.date;
                p.description = model.description;

                db.Update(p);
                await db.SaveChangesAsync();
                return Json("update successful");
            }
            else
            {
                return Json("not find!!!");
            }


        }
        public async Task<IActionResult> CityInsertConfirm(CityViewModel model)
        {
            City city = new City();
            city.id = model.id;
            city.date = model.date;
            city.name = model.name;
            db.Add(city);
            await db.SaveChangesAsync();
            return Json("Insert confirm");

        }

        public async Task<IActionResult> CitySelect()
        {
            var p = await db.cities.ToListAsync();

            return Json(p);
        }
        public async Task<IActionResult> CityDelete(int id)
        {

            var p = db.cities.Find(id);
            if (p != null)
            {
                db.Remove(p);
                await db.SaveChangesAsync();
                return Json("delete confirm");
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public IActionResult CityEdit(int id)
        {

            var p = db.cities.Find(id);
            if (p != null)
            {

                return Json(p);
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public async Task<IActionResult> CityUpdate(CityViewModel model)
        {

            var p = db.cities.Find(model.id);
            if (p != null)
            {

                p.name = model.name;
                p.date = model.date;

                db.Update(p);
                await db.SaveChangesAsync();
                return Json("update successful");
            }
            else
            {
                return Json("not find!!!");
            }


        }
        public IActionResult ProductCategoryActiont()
        {

            return View();
        }
        public IActionResult ProductCategoryInsertConfirm(ProductCetegoryViewModel model)
        {
            ProductCategory productCategory = new ProductCategory();
            productCategory.id = model.id;
            productCategory.date = model.date;
            productCategory.name = model.name;
            db.Add(productCategory);
            db.SaveChanges();
            return Json("Insert confirm");

        }
        public IActionResult ProductCategoryselect()
        {
            var p = db.productCategories.ToList();
            return Json(p);
        }
        public async Task<IActionResult> ProductCategoryDelete(int id)
        {

            var p = db.productCategories.Find(id);
            if (p != null)
            {
                db.Remove(p);
                await db.SaveChangesAsync();
                return Json("delete confirm");
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public IActionResult ProductCategoryEdit(int id)
        {

            var p = db.productCategories.Find(id);
            if (p != null)
            {

                return Json(p);
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public async Task<IActionResult> ProductCategoryUpdate(CityViewModel model)
        {

            var p = db.productCategories.Find(model.id);
            if (p != null)
            {

                p.name = model.name;
                p.date = model.date;

                db.Update(p);
                await db.SaveChangesAsync();
                return Json("update successful");
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public async Task<IActionResult> InsertProduct([FromServices] UserManager<ApplicationUser> userManager)
        {
            var sellers = await userManager.GetUsersInRoleAsync("Sellers");
            ViewData["sellers"] = sellers;

            ViewData["cities"] = await db.cities.ToListAsync();
            ViewData["brands"] = await db.brands.ToListAsync();
            ViewData["category"] = await db.productCategories.ToListAsync();
            ViewData["sellers"] = sellers;
            return View();
        }
        public IActionResult checkProductName(string name)
        {
            return Json(db.products.ToList().Count(x => x.name == name) == 0);
        }
        public async Task<IActionResult> InsertProductConfirm(ProductViewModel p)
        {
            Product product = new();
            List<ProductImages> lstImages = new List<ProductImages>();
            p.img.ForEach(x =>
            {
                ProductImages images = new ProductImages();
                byte[] b = new byte[x.Length];
                x.OpenReadStream().Read(b, 0, b.Length);

                MemoryStream memory = new MemoryStream(b);
                Image imagefile = Image.FromStream(memory);
                Bitmap bitmap = new Bitmap(imagefile, 200, 200 * imagefile.Height / imagefile.Width);
                MemoryStream memory1 = new MemoryStream();
                bitmap.Save(memory1, System.Drawing.Imaging.ImageFormat.Jpeg);


                images.img = b;
                images.thumbnail = memory1.ToArray();
                lstImages.Add(images);

            });


            product.productImages = lstImages;


            product.name = p.name;
            product.count = p.count;
            product.price = p.price;
            product.brandId = p.brandid;
            product.cityId = p.cityid;
            product.productCategoryId = p.catrgoryid;
            product.date = p.date;
            product.color = p.color;
            product.size = p.size;
            product.description = p.description;
            product.IsAvalaible = p.IsAvalaible;
            product.sellerid = p.sellerid;




            db.products.Add(product);
            await db.SaveChangesAsync();
            return RedirectToAction("InsertProduct");
        }
        public IActionResult ListOfProduct()
        {

            var p = db.products.Include(x => x.seller).Include(x => x.city).Include(x => x.brand).Include(x => x.productCategory).Include(x => x.productImages).ToList();
            return View(p);
        }
        public IActionResult ShowImg(int id)
        {

            var p = db.productImages.Find(id).img;
            return File(p, "image/jpeg");

        }
        public async Task<IActionResult> DeleteProduct(int productid)
        {

            var x = db.Comments.Where(c => c.ProductId == productid).ToList();
            if (x != null)
            {

                db.Comments.RemoveRange(x);
                db.SaveChanges();
            }
            var y = db.likes.Where(c => c.productid == productid).ToList();
            if (y != null)
            {

                db.likes.RemoveRange(y);
                db.SaveChanges();
            }

            var p = db.products.Find(productid);
            if (p != null)
            {
                db.Remove(p);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("ListOfProduct");
        }
        public IActionResult Edit(int productid, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var sellers = userManager.GetUsersInRoleAsync("Sellers").Result;
            ViewData["sellers"] = sellers;
            ViewData["category"] = db.productCategories.ToList();
            ViewData["brands"] = db.brands.ToList();
            ViewData["cities"] = db.cities.ToList();
            ProductViewModel product = new ProductViewModel();
            var p = db.products.Include(x => x.productImages).FirstOrDefault(x => x.id == productid);
            product.id = p.id;
            product.name = p.name;
            product.count = p.count;
            product.price = p.price;

            product.imagesViewModels = p.productImages.Select(x => new ImagesViewModel { id = x.id, imgbyte = x.thumbnail }).ToList();
            product.brandid = p.brandId;
            product.cityid = p.cityId;
            product.catrgoryid = p.productCategoryId;
            product.IsAvalaible = p.IsAvalaible;
            product.date = p.date;
            product.color = p.color;
            product.size = p.size;
            product.description = p.description;

            return View(product);
        }
        public async Task<IActionResult> Update(ProductViewModel p)
        {


            var k = db.products.Include(x => x.productImages).FirstOrDefault(x => x.id == p.id);
            if (k != null)
            {
                k.name = p.name;
                k.count = p.count;
                k.price = p.price;
                k.brandId = p.brandid;
                k.cityId = p.cityid;
                k.date = p.date;
                k.color = p.color;
                k.sellerid = p.sellerid;
                k.productCategoryId = p.catrgoryid;
                k.size = p.size;
                k.description = p.description;
                k.IsAvalaible = p.IsAvalaible;
                p.img?.ForEach(x =>
                {
                    if (x != null)
                    {
                        byte[] b = new byte[x.Length];
                        x.OpenReadStream().Read(b, 0, b.Length);

                        MemoryStream memory = new MemoryStream(b);
                        Image imagefile = Image.FromStream(memory);
                        Bitmap bitmap = new Bitmap(imagefile, 200, 200 * imagefile.Height / imagefile.Width);
                        MemoryStream memory1 = new MemoryStream();
                        bitmap.Save(memory1, System.Drawing.Imaging.ImageFormat.Jpeg);

                        ProductImages images = new ProductImages();
                        images.img = b;
                        images.thumbnail = memory1.ToArray();
                        k.productImages.Add(images);

                    }
                });

                if (p.imageDeletId != null)
                {
                    var g = k.productImages.Where(x => p.imageDeletId.Contains(x.id)).ToList();
                    g.ForEach(x => db.Remove(x));

                }
                db.Update(k);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("ListOfProduct");

        }
        public IActionResult DiscountCode()
        {

            return View();
        }
        public async Task<IActionResult> DiscountInsert(DisViewModel model)
        {
            dis dis = new dis();

            dis.code = model.code;
            dis.isactive = model.isactive;
            db.Add(dis);
            await db.SaveChangesAsync();
            return Json("Insert confirm");

        }
        public async Task<IActionResult> DisSelect()
        {
            var p = await db.dis.ToListAsync();

            return Json(p);
        }
        public async Task<IActionResult> DisDelete(int id)
        {

            var p = db.dis.Find(id);
            if (p != null)
            {
                db.Remove(p);
                await db.SaveChangesAsync();
                return Json("delete confirm");
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public IActionResult DisEdit(int id)
        {

            var p = db.dis.Find(id);
            if (p != null)
            {

                return Json(p);
            }
            else
            {
                return Json("not find!!!");
            }
        }
        public async Task<IActionResult> DisUpdate(DisViewModel model)
        {

            var p = db.dis.Find(model.id);
            if (p != null)
            {

                p.code = model.code;
                p.isactive = model.isactive;

                db.Update(p);
                await db.SaveChangesAsync();
                return Json("update successful");
            }
            else
            {
                return Json("not find!!!");
            }


        }
        public IActionResult AverageSalesLastWeek()
        {

            DateTime currentDate = DateTime.Now;


            DateTime lastWeekStartDate = currentDate.AddDays(-7).Date;




            var averageSalesLsw = db.orders
                .Where(x => x.status == Order.OrderStatus.Paid && x.dateTime >= lastWeekStartDate && x.dateTime <= currentDate).ToList();


            if (averageSalesLsw.Any())
            {
                var averageSales = averageSalesLsw.Average(x => x.totalPrice);
                return Json(averageSales);
            }
            else
            {
                return Json(0);
            }

        }

        public IActionResult AverageSalesLastMonth()
        {

            DateTime currentDate = DateTime.Now;


            DateTime lastMonthStartDate = currentDate.AddMonths(-1).Date;





            var averageSaleslsm = db.orders
       .Where(x => x.status == Order.OrderStatus.Paid && x.dateTime >= lastMonthStartDate && x.dateTime <= currentDate).ToList();

            if (averageSaleslsm.Any())
            {
                var averageSales = averageSaleslsm.Average(x => x.totalPrice);
                return Json(averageSales);
            }
            else
            {
                return Json(0);
            }



        }
        public IActionResult AveSaleProductLastWeek()
        {
            DateTime currentdate = DateTime.Now;
            DateTime lastWeekStartDate = currentdate.AddDays(-7).Date;

            float totalsaleproductlsw = db.OrderDetails.Include(x => x.Order).Where(x => x.Order.status == Order.OrderStatus.Paid && x.Order.dateTime >= lastWeekStartDate && x.Order.dateTime <= currentdate).Sum(x => x.quantity);

            float aveprd = totalsaleproductlsw / 7;


            return Json(aveprd);
        }
        public IActionResult AverageSalesLastWeekPerDay()
        {

            DateTime currentDate = DateTime.Now;

            DateTime lastWeekStartDate = currentDate.AddDays(-7).Date;





            var averageSales = db.orders
                .Where(x => x.status == Order.OrderStatus.Paid && x.dateTime >= lastWeekStartDate && x.dateTime <= currentDate)
                .Sum(x => x.totalPrice);
            var avesalepday = averageSales / 7;

            return Json(avesalepday);



        }
        public IActionResult RecordWeeklySale()
        {
            DateTime currentDate = DateTime.Now;

            DateTime lastWeekStartDate = currentDate.AddDays(-7).Date;


            var averageSales = db.orders
                .Where(x => x.status == Order.OrderStatus.Paid && x.dateTime >= lastWeekStartDate && x.dateTime <= currentDate)
                .Sum(x => x.totalPrice);
            var avesalepday = averageSales / 7;

            DailySale daily = new DailySale();
            daily.TotalSales = (decimal)avesalepday;
            daily.SaleDate = DateTime.Now;
            db.dailySales.Add(daily);
            db.SaveChanges();
            return Json("Record Successful.");



        }

        public IActionResult FilterOrders(DateTime startDate, DateTime endDate)
        {
            var salefilter = db.orders
                  .Where(x => x.status == Order.OrderStatus.Paid && x.dateTime >= startDate && x.dateTime <= endDate)
                  .Sum(x => x.totalPrice);
            return Json(salefilter);

        }
        public IActionResult Filterprsold(DateTime startDate, DateTime endDate)
        {
            var prsoldfilter = db.OrderDetails.Include(x => x.Order).Where(x => x.Order.status == Order.OrderStatus.Paid && x.Order.dateTime >= startDate && x.Order.dateTime <= endDate).Sum(x => x.quantity);
            return Json(prsoldfilter);

        }

        public IActionResult RecordTotalinventory()
        {
            var p = db.products.Sum(x => x.count);
            Inventoryval m = new Inventoryval();
            m.date = DateTime.Now;
            m.inventoryvalue = (float)p;
            db.inventoryval.Add(m);
            db.SaveChangesAsync();
            return Json("record Successful");
        }

        public IActionResult IncreaseWeeklySale()
        {
            var sales = db.dailySales
                .OrderByDescending(x => x.SaleDate)
                .Take(2).ToList();



            if (sales.Count >= 2)
            {
                decimal weekAgo = sales[0].TotalSales;
                decimal twoWeeksAgo = sales[1].TotalSales;

                decimal increase = CalPercentChange(twoWeeksAgo, weekAgo);

                return Json(increase);

            }
            else
            {
                return Json("Data is Not Enough");
            }
        }

        public decimal CalPercentChange(decimal twoWeeksAgo, decimal weekAgo)
        {
            if (twoWeeksAgo == 0)
            {
                return 0;
            }

            return ((weekAgo - twoWeeksAgo) / twoWeeksAgo) * 100;
        }


        public IActionResult TopProductSaleLastWeek(int num)
        {
            DateTime currentDate = DateTime.Now;
            DateTime oneWeekAgo = currentDate.AddDays(-7);

            var topProductsInLastWeek = db.OrderDetails
                .Where(x => x.Order.status == Order.OrderStatus.Paid && x.Order.dateTime >= oneWeekAgo && x.Order.dateTime <= currentDate)
                .GroupBy(x => x.productId)
                .Select(y => new TopProductWithQuantity
                {
                    Product = db.products.FirstOrDefault(p => p.id == y.Key),
                    TotalQuantity = y.Sum(x => x.quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(num)
                .ToList();

            return Json(topProductsInLastWeek);
        }

        public IActionResult CheckCanceledOrder()
        {
            bool check = db.orders.Any(x => x.status == Order.OrderStatus.Canceled);
            if (check)
            {

                return Json("1");
            }

            else
            {
                return Json("0");
            }
        }

        public IActionResult CheckLowInventory()
        {
            List<string> lIP = new List<string>();

            foreach (var item in db.products)
            {
                if (item.count < 50)
                {
                    lIP.Add(item.name);
                }
            }

            return Json(lIP);
        }


    }

}
