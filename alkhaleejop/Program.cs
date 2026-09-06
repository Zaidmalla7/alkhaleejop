using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using alkhaleejop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;

namespace alkhaleejop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // === التعديل صار هون: رح يقرأ من متغيرات البيئة أول، وإذا ما لقاها بروح لملف appsettings ===
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                   ?? builder.Configuration.GetConnectionString("DefaultConnection22")
                                   ?? throw new InvalidOperationException("Connection string not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // تحديد مدة بقاء تسجيل الدخول (مثلاً 30 يوم)
                options.ExpireTimeSpan = TimeSpan.FromDays(30);

                // تجديد الوقت تلقائياً: 
                // يعني لو دخل في اليوم الـ 29، النظام برجع يمددله كمان 30 يوم
                options.SlidingExpiration = true;
            });
            builder.Services.AddScoped<alkhaleejop.Services.IImageService, alkhaleejop.Services.ImageService>();

            var app = builder.Build();

            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.UseHttpsRedirection();

            // ==========================================
            // التعديل هنا: هذا السطر ضروري جداً لعرض الصور المرفوعة
            app.UseStaticFiles();
            // ==========================================

            app.UseResponseCompression();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
                .WithStaticAssets();

            app.Run();
        }
    }
}