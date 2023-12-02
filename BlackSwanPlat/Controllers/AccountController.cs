using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using BlackSwanPlat.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackSwanPlat.Controllers
{
    public class AccountController : Controller
    {
        private object usermansger;


        public IActionResult RegisterLogin()
        {
            return View();
        }
        public async Task<IActionResult> RegisterConfirm(RegisterLoginViewModel model, [FromServices] UserManager<ApplicationUser> usermaneger, [FromServices] IEmailSender emailSender)
        {
            ApplicationUser user = await usermaneger.FindByEmailAsync(model.username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = model.username,
                    firstName = model.firstName,
                    lastName = model.lastName,
                    Email = model.username,
                    EmailConfirmed = true,
                    PhoneNumber = model.phone,
                    age = model.age,
                    city = model.city,

                };
                var status = await usermaneger.CreateAsync(user, model.password);
                if (status.Succeeded)
                {
                    if (await usermaneger.IsInRoleAsync(user, "Customers") == false)
                    {
                        await usermaneger.AddToRoleAsync(user, "Customers");
                    }
                    //string emailConfirmationToken = await usermaneger.GenerateEmailConfirmationTokenAsync(user);
                    //string emailConfirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = emailConfirmationToken }, Request.Scheme);


                    //await emailSender.SendEmailAsync(model.username, "confirm email", emailConfirmationLink);


                    return RedirectToAction("RegisterLogin");
                }
                else
                {
                    return RedirectToAction("RegisterLogin");

                }
            }
            else
            {
                return RedirectToAction("RegisterLogin");

            }
        }
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> LoginConfirm([FromServices] UserManager<ApplicationUser> userManager,
          [FromServices] SignInManager<ApplicationUser> signInManager, RegisterLoginViewModel models)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(models.username);
            if (user != null)
            {
                var status = await signInManager.PasswordSignInAsync(user, models.password, false, true);
                if (status.Succeeded)
                {
                    await userManager.ResetAccessFailedCountAsync(user);
                    if (await userManager.IsInRoleAsync(user, "Customers") == true && models.roleName == "Customers")
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else if (await userManager.IsInRoleAsync(user, "Admins") == true && models.roleName == "Admins")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (await userManager.IsInRoleAsync(user, "Sellers") == true && models.roleName == "Sellers")
                    {
                        return RedirectToAction("Index", "Seller");
                    }
                    else if (await userManager.IsInRoleAsync(user, "Delivers") == true && models.roleName == "Delivers")
                    {
                        return RedirectToAction("Index", "Deliver");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else if (status.IsLockedOut)
                {

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("RegisterLogin");
                }
            }
            else
            {
                return RedirectToAction("RegisterLogin");
            }

        }




        public async Task<IActionResult> Logout([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }




        public async Task<IActionResult> UserPanel(string userid, [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser User = await userManager.FindByEmailAsync(userid);

            if (User != null)
            {


                UserProfileViewModel viewModel = new UserProfileViewModel
                {
                    id = User.Id,
                    UserName = User.UserName,

                    firstName = User.firstName,
                    lastName = User.lastName,
                    PhoneNumber = User.PhoneNumber,
                    city = User.city,
                    age = User.age,

                };


                return View(viewModel);
            }
            else
            {

                return RedirectToAction("RegisterLogin");
            }
        }

        public async Task<IActionResult> UpdateUserProfile(UserProfileViewModel model, [FromServices] UserManager<ApplicationUser> userManager)
        {



            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user != null)
            {

                user.firstName = model.firstName;
                user.lastName = model.lastName;
                user.PhoneNumber = model.PhoneNumber;
                user.city = model.city;
                user.age = model.age;
                user.UserName = model.UserName;
                user.EmailConfirmed = true;


                var status = await userManager.UpdateAsync(user);

                if (status.Succeeded)
                {

                    return Json("Update successful");
                }
                else
                {

                    return Json("error in update profile");
                }
            }
            else
            {

                return Json("User Not find");
            }

        }
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if (user != null)
            {

                var Check = await userManager.CheckPasswordAsync(user, currentPassword);

                if (!Check)
                {

                    return Json("Current password is incorrect");
                }


                var status = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (status.Succeeded)
                {

                    return Json("Password changed is successful");
                }
                else
                {

                    return Json("Error changing password");
                }
            }
            else
            {

                return Json("User Not Find");
            }

        }
        public async Task<IActionResult> ResetPassword(string email, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(email);


            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            ViewData["token"] = token;
            return Json(token);




        }



        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordViewModel model, [FromServices] UserManager<ApplicationUser> userManager)
        {

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {

                return Json("User Not Find.");
            }


            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {

                return Json("Reset Password Successful.");
            }
            else
            {

                return Json("Error in Reset Password.");
            }
        }




        public async Task<IActionResult> ConfirmEmail(string userId, string token, [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (userId == null || token == null)
            {

                return RedirectToAction("Error", "Error", new { statusCode = 301 }); 
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {

                return RedirectToAction("Error", "Error", new { statusCode = 404 });
            }


            var status = await userManager.ConfirmEmailAsync(user, token);
            if (status.Succeeded)
            {

                return RedirectToAction("Login", "Account");
            }
            else
            {

                return RedirectToAction("Error", "Error", new { statusCode = 500 });
            }
        }



    }
}
