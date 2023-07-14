using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Services;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                // cau hinh de tim template voi cau truc thu muc tuy chon
                // {0}: ten action
                // {1}: ten controller
                // {2}: ten area
                options.ViewLocationFormats.Add("/MyView/{1}/{0}.cshtml");
            });
            builder.Services.AddSingleton<ProductServices>();
            builder.Services.AddSingleton<PlanetServices>();

            // ADD DB context
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MVC");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Configure password
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // Configure lockout
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // Configure user
                options.User.RequireUniqueEmail = true;
                // Configure log-in
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/accessdenied/";
            });


            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                var googleConfig = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleConfig["ClientId"] ?? "";
                options.ClientSecret = googleConfig["ClientSecret"] ?? "";
                options.CallbackPath = "/login-from-google";
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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


            //app.MapControllers();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "first-route",
                    pattern: "product/{id?}",
                    defaults: new
                    {
                        controller = "First",
                        action = "ViewProduct",
                    });
            });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}