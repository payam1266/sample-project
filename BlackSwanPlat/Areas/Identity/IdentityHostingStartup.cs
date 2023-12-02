 using System;
using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebPWrecover.Services;
using Microsoft.AspNetCore.Authentication.Google;

[assembly: HostingStartup(typeof(BlackSwanPlat.Areas.Identity.IdentityHostingStartup))]
namespace BlackSwanPlat.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DbBlack>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DbBlackConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                  .AddRoles<IdentityRole>()  .AddEntityFrameworkStores<DbBlack>();
                services.AddTransient<IEmailSender, EmailSender>();



            


                services.AddAuthorization(x =>
                {
                    x.AddPolicy("Adminpolicy", p => p.RequireRole("Admins"));
                    x.AddPolicy("SellerPlicy", p => p.RequireRole("Sellers"));
                    x.AddPolicy("DeliverPolicy", p => p.RequireRole("Delivers"));

                });
                services.ConfigureApplicationCookie(x =>
                {
                    x.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents()
                    {
                        OnRedirectToAccessDenied = y =>
                    {
                        y.Response.Redirect("/Account/RegisterLogin");
                        return Task.CompletedTask;
                    },
                        OnRedirectToLogin = y =>
                        {
                            y.Response.Redirect("/Account/RegisterLogin");
                            return Task.CompletedTask;
                        }

                    };
                });
            });
        }
    }
}
