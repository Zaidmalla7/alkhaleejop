using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using alkhaleejop.Models;
using alkhaleejop.Data;

namespace alkhaleejop.Controllers
{
    [Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // دالة ذكية لجلب إحصائيات لوحة التحكم
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var today = DateTime.UtcNow.AddHours(3).Date;

                var totalPatients = await _context.Patients.CountAsync();
                var totalExams = await _context.Examinations.CountAsync();

                var examsThisMonth = await _context.Examinations
                    .CountAsync(e => e.ExamDate.Year == today.Year && e.ExamDate.Month == today.Month);

                var pendingReminders = await _context.Examinations
                    .CountAsync(e => e.NextExamDate != null && e.NextExamDate >= today && e.NextExamDate <= today.AddDays(14) && e.IsReminderSent == false);

                var sixMonthsAgo = today.AddMonths(-5);
                var recentExamsForChart = await _context.Examinations
                    .Where(e => e.ExamDate >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                    .Select(e => e.ExamDate)
                    .ToListAsync();

                var chartLabels = new List<string>();
                var chartData = new List<int>();

                string[] arabicMonths = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };

                for (int i = 5; i >= 0; i--)
                {
                    var m = today.AddMonths(-i);
                    chartLabels.Add($"{arabicMonths[m.Month - 1]} {m.Year}");
                    chartData.Add(recentExamsForChart.Count(e => e.Year == m.Year && e.Month == m.Month));
                }

                var recentVisits = await _context.Examinations
                    .Include(e => e.Patient)
                    .OrderByDescending(e => e.ExamDate)
                    .Take(5)
                    .Select(e => new {
                        patientName = e.Patient.FirstName + " " + e.Patient.FamilyName,
                        date = e.ExamDate.ToString("yyyy/MM/dd"),
                        isVip = e.Patient.IsVIP,
                        phoneNumber = e.Patient.PhoneNumber 
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    totalPatients,
                    totalExams,
                    examsThisMonth,
                    pendingReminders,
                    chartLabels,
                    chartData,
                    recentVisits
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء جلب بيانات لوحة التحكم");
                return Json(new { success = false });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}