using alkhaleejop.Data;
using alkhaleejop.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace alkhaleejop.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Examination> Examinations { get; set; }


        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // هذا السطر إجباري جداً مع الـ Identity، بدونه رح يضرب النظام
            base.OnModelCreating(builder);
            // ==========================================
            // إضافة صلاحية الأدمن اللي طلبتها بالظبط بالـ ID تبعها
            // ==========================================
            var admin = new IdentityRole
            {
                Id = "9A2E45B8-6D7A-4D99-8A4F-B1E2A3F7E9D1",
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            };
            builder.Entity<IdentityRole>().HasData(admin);
            // 1. تسريع البحث عن مريض برقم الهاتف
            builder.Entity<Patient>()
                .HasIndex(p => p.PhoneNumber);

            // 2. تسريع جلب المرضى الذين حان موعد فحصهم (نظام التذكير)
            builder.Entity<Examination>()
                .HasIndex(e => e.NextExamDate);
        }
    }
}