using BlackSwanPlat.Areas.Identity.Data;
using BlackSwanPlat.Data;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.DependencyInjection;
using WebPWrecover.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.SignalR;
namespace BlackSwanPlat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<DbBlack>();
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddSignalR();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AuthMessageSender"));
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); 
            });

          

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)

        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/updateHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=index}/{id?}");
            });


            app.UseStatusCodePagesWithReExecute("/Error/{0}");


            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}");
            });

            InitIdentity(userManager, roleManager).Wait();
        }

        private async Task InitIdentity(UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser admin = await userManager.FindByEmailAsync("payam@gmail.com");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "payam@gmail.com",
                    Email = "payam@gmail.com",
                    EmailConfirmed = true,
                    firstName = "payam",
                    lastName = "ghaznavi",
                    PhoneNumber = "09364154728",
                    age = 36,
                    city="shiraz"
                };

                var status = await userManager.CreateAsync(admin, "Pp=-123456");
            }
              ApplicationUser seller = await userManager.FindByEmailAsync("parham@gmail.com");
            if (seller == null)
            {
                seller = new ApplicationUser
                {
                    UserName = "parham@gmail.com",
                    Email = "parham@gmail.com",
                    EmailConfirmed = true,
                    firstName = "parham",
                    lastName = "ghaznavi",
                    PhoneNumber = "09364154728",
                    age = 36,
                    city = "shiraz"
                };

                var status = await userManager.CreateAsync(seller, "Mm=-123456");
            }
            ApplicationUser deliver = await userManager.FindByEmailAsync("parsa@gmail.com");
            if (deliver == null)
            {
                deliver = new ApplicationUser
                {
                    UserName = "parsa@gmail.com",
                    Email = "parsa@gmail.com",
                    EmailConfirmed = true,
                    firstName = "parsa",
                    lastName = "ghaznavi",
                   PhoneNumber = "09364154728",
                    age = 36,
                    city = "shiraz"
                };

                var status = await userManager.CreateAsync(deliver, "Dd=-123456");
            }

            if (await roleManager.RoleExistsAsync("Admins") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Admins"));
            }
            if (await roleManager.RoleExistsAsync("Sellers") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Sellers"));
            }
            if (await roleManager.RoleExistsAsync("Customers") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Customers"));
            }
            if (await roleManager.RoleExistsAsync("Delivers") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Delivers"));
            }

            if (await userManager.IsInRoleAsync(admin, "Admins") == false)
            {
                await userManager.AddToRoleAsync(admin, "Admins");
            } 
            if (await userManager.IsInRoleAsync(admin, "Sellers") == false)
            {
                await userManager.AddToRoleAsync(admin, "Sellers");
            } 
            if (await userManager.IsInRoleAsync(admin, "Customers") == false)
            {
                await userManager.AddToRoleAsync(admin, "Customers");
            } 
            if (await userManager.IsInRoleAsync(admin, "Delivers") == false)
            {
                await userManager.AddToRoleAsync(admin, "Delivers");
            }
            if (await userManager.IsInRoleAsync(seller, "Sellers") == false)
            {
                await userManager.AddToRoleAsync(seller, "Sellers");
            } 
            if (await userManager.IsInRoleAsync(seller, "Delivers") == false)
            {
                await userManager.AddToRoleAsync(seller, "Delivers");
            }
            if (await userManager.IsInRoleAsync(deliver, "Delivers") == false)
            {
                await userManager.AddToRoleAsync(deliver, "Delivers");
            }

        }
        }
    }

