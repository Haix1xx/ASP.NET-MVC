using Microsoft.AspNetCore.Mvc.Razor;
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}