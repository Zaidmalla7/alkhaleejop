using alkhaleejop.Data;
using alkhaleejop.Models.Entity;
using alkhaleejop.Models.ViewModels;
using alkhaleejop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alkhaleejop.Controllers
{
    [Authorize]
    [Route("ExaminationsController1/[action]")]
    public class ExaminationsController1 : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExaminationsController1> _logger;
        private readonly IImageService _imageService;

        public ExaminationsController1(ApplicationDbContext context, ILogger<ExaminationsController1> logger, IImageService imageService)
        {
            _context = context;
            _logger = logger;
            _imageService = imageService;
        }

        // 1. عرض صفحة الفحوصات الرئيسية
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // إضافة عميل جديد بسرعة من شاشة الفحوصات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatientQuickly(string firstName, string fatherName, string familyName, string phoneNumber, int age, string address)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(fatherName) || string.IsNullOrWhiteSpace(familyName) ||
                string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(address) || age <= 0)
            {
                return Json(new { success = false, message = "يرجى تعبئة جميع الحقول المطلوبة (الاسم، الأب، العائلة، الهاتف، العمر، والعنوان)." });
            }

            try
            {
                var newPatient = new Patient
                {
                    FirstName = firstName.Trim(),
                    FatherName = fatherName.Trim(),
                    FamilyName = familyName.Trim(),
                    PhoneNumber = phoneNumber.Trim(),
                    Age = age,
                    Address = address.Trim()
                };

                _context.Patients.Add(newPatient);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "تمت إضافة العميل بنجاح!",
                    patientId = newPatient.Id,
                    patientName = $"{newPatient.FirstName} {newPatient.FatherName} {newPatient.FamilyName} - {newPatient.PhoneNumber}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الإضافة السريعة للعميل.");
                return Json(new { success = false, message = "حدث خطأ داخلي أثناء إضافة العميل." });
            }
        }
        // ==========================================
        // البحث الذكي عن المرضى
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> SearchPatients(string term)
        {
            var query = _context.Patients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                var searchWords = term.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in searchWords)
                {
                    query = query.Where(p =>
                        p.FirstName.Contains(word) ||
                        (p.FatherName != null && p.FatherName.Contains(word)) ||
                        p.FamilyName.Contains(word) ||
                        p.PhoneNumber.Contains(word) ||
                        (p.SecondaryPhoneNumber != null && p.SecondaryPhoneNumber.Contains(word)));
                }
            }

            var patients = await query.Take(20).Select(p => new {
                id = p.Id,
                text = $"{p.FirstName} {p.FatherName} {p.FamilyName} - {p.PhoneNumber}"
            }).ToListAsync();

            return Json(patients);
        }

        // 3. جلب بيانات الفحوصات لجدول العرض
        [HttpGet]
        public async Task<IActionResult> GetExamsData()
        {
            try
            {
                var exams = await _context.Examinations
                    .Include(e => e.Patient)
                    .AsNoTracking()
                    .OrderByDescending(e => e.ExamDate)
                    .Select(e => new {
                        id = e.Id,
                        examDate = e.ExamDate,
                        patientName = e.Patient.FirstName + " " + e.Patient.FamilyName,
                        phoneNumber = e.Patient.PhoneNumber,
                        secondaryPhoneNumber = e.Patient.SecondaryPhoneNumber,
                        secondaryPhoneOwner = e.Patient.SecondaryPhoneOwner,
                        nextExamDate = e.NextExamDate,
                        isReminderSent = e.IsReminderSent,
                        hasImage = !string.IsNullOrEmpty(e.GlassImagePath)
                    })
                    .ToListAsync();

                return Json(new { data = exams });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء جلب الفحوصات");
                return StatusCode(500, new { message = "حدث خطأ داخلي." });
            }
        }

        // 4. جلب فحص محدد للتعديل
        [HttpGet]
        public async Task<IActionResult> GetExamById(int id)
        {
            var exam = await _context.Examinations.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null)
                return Json(new { success = false, message = "الفحص غير موجود." });

            return Json(new { success = true, data = exam });
        }

        // 5. جلب معلومات التواصل السريعة
        [HttpGet]
        public async Task<IActionResult> GetContactInfo(int id)
        {
            var exam = await _context.Examinations
                .Include(e => e.Patient)
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new {
                    examId = e.Id,
                    patientName = e.Patient.FirstName + " " + e.Patient.FamilyName,
                    primaryPhone = e.Patient.PhoneNumber,
                    secondaryPhone = e.Patient.SecondaryPhoneNumber,
                    secondaryPhoneOwner = e.Patient.SecondaryPhoneOwner,
                    nextExamDateStr = e.NextExamDate.HasValue ? e.NextExamDate.Value.ToString("yyyy/MM/dd") : "غير محدد",
                    isReminderSent = e.IsReminderSent
                })
                .FirstOrDefaultAsync();

            if (exam == null)
                return Json(new { success = false, message = "بيانات الفحص غير موجودة." });

            return Json(new { success = true, data = exam });
        }

        // 6. إضافة فحص جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExaminationViewModel vm)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "يرجى تعبئة الحقول المطلوبة." });

            try
            {
                string? uploadedImagePath = null;
                if (vm.ImageFile != null)
                {
                    uploadedImagePath = await _imageService.UploadImageAsync(vm.ImageFile);
                }

                var exam = new Examination
                {
                    PatientId = vm.PatientId,
                    ExamDate = DateTime.UtcNow.AddHours(3),
                    NextExamDate = vm.NextExamDate,
                    GlassImagePath = uploadedImagePath,

                    ODSph = vm.ODSph,
                    ODCyl = vm.ODCyl,
                    ODAxis = vm.ODAxis,
                    ODAdd = vm.ODAdd,
                    ODVA = vm.ODVA,

                    OSSph = vm.OSSph,
                    OSCyl = vm.OSCyl,
                    OSAxis = vm.OSAxis,
                    OSAdd = vm.OSAdd,
                    OSVA = vm.OSVA,

                    IPD = vm.IPD,
                    Remarks = vm.Remarks,
                    PurchaseNotes = vm.PurchaseNotes,
                    ExaminerName = vm.ExaminerName
                };

                _context.Examinations.Add(exam);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم تسجيل الفحص الطبي بنجاح!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء حفظ فحص جديد.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 7. تعديل فحص موجود
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExaminationViewModel vm)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "البيانات غير صحيحة." });

            try
            {
                var existingExam = await _context.Examinations.FindAsync(vm.Id);
                if (existingExam == null)
                    return Json(new { success = false, message = "الفحص غير موجود." });

                if (vm.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(existingExam.GlassImagePath))
                    {
                        _imageService.DeleteImage(existingExam.GlassImagePath);
                    }

                    existingExam.GlassImagePath = await _imageService.UploadImageAsync(vm.ImageFile);
                }

                if (existingExam.NextExamDate != vm.NextExamDate)
                {
                    existingExam.IsReminderSent = false;
                }

                existingExam.PatientId = vm.PatientId;
                existingExam.NextExamDate = vm.NextExamDate;

                existingExam.ODSph = vm.ODSph;
                existingExam.ODCyl = vm.ODCyl;
                existingExam.ODAxis = vm.ODAxis;
                existingExam.ODAdd = vm.ODAdd;
                existingExam.ODVA = vm.ODVA;

                existingExam.OSSph = vm.OSSph;
                existingExam.OSCyl = vm.OSCyl;
                existingExam.OSAxis = vm.OSAxis;
                existingExam.OSAdd = vm.OSAdd;
                existingExam.OSVA = vm.OSVA;

                existingExam.IPD = vm.IPD;
                existingExam.Remarks = vm.Remarks;
                existingExam.PurchaseNotes = vm.PurchaseNotes;
                existingExam.ExaminerName = vm.ExaminerName;

                _context.Examinations.Update(existingExam);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم تحديث الفحص بنجاح." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تعديل الفحص رقم {Id}", vm.Id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingReminders()
        {
            try
            {
                var today = DateTime.UtcNow.AddHours(3).Date;

                var reminders = await _context.Examinations
                    .Include(e => e.Patient)
                    .Where(e => e.NextExamDate != null &&
                                e.IsReminderSent == false &&
                                e.NextExamDate >= today.AddDays(-14) &&
                                e.NextExamDate <= today.AddDays(14))
                    .OrderBy(e => e.NextExamDate)
                    .Select(e => new {
                        id = e.Id, 
                        patientId = e.PatientId, 
                        patientName = e.Patient.FirstName + " " + e.Patient.FamilyName,
                        phone = e.Patient.PhoneNumber,
                        secondaryPhone = e.Patient.SecondaryPhoneNumber,
                        secondaryPhoneOwner = e.Patient.SecondaryPhoneOwner,
                        dateStr = e.NextExamDate.Value.ToString("yyyy/MM/dd"),
                        daysDiff = (e.NextExamDate.Value.Date - today).Days
                    })
                    .ToListAsync();

                return Json(new { success = true, data = reminders });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "حدث خطأ" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsContacted(int id, bool renewForSixMonths = false)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam != null)
            {
                if (renewForSixMonths)
                {
                    exam.NextExamDate = DateTime.UtcNow.AddHours(3).AddMonths(6);
                    exam.IsReminderSent = false; 
                }
                else
                {
                    exam.IsReminderSent = true;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public async Task<IActionResult> ExportWeeklyRemindersExcel()
        {
            try
            {
                var today = DateTime.UtcNow.AddHours(3).Date;
                var nextWeek = today.AddDays(7); 

                var reminders = await _context.Examinations
                    .Include(e => e.Patient)
                    .Where(e => e.NextExamDate != null &&
                                e.IsReminderSent == false &&
                                e.NextExamDate >= today.AddDays(-14) &&
                                e.NextExamDate <= nextWeek)
                    .OrderBy(e => e.NextExamDate)
                    .ToListAsync();

                var builder = new StringBuilder();

                builder.AppendLine("اسم العميل,رقم الهاتف الأساسي,الهاتف الاحتياطي,لمن الرقم الاحتياطي,تاريخ الموعد,حالة الموعد");

                foreach (var item in reminders)
                {
                    var patientName = $"{item.Patient.FirstName} {item.Patient.FamilyName}".Replace(",", " ");
                    var phone = item.Patient.PhoneNumber ?? "لا يوجد";
                    var secPhone = item.Patient.SecondaryPhoneNumber ?? "لا يوجد";
                    var secOwner = item.Patient.SecondaryPhoneOwner ?? "-";
                    var examDate = item.NextExamDate.Value.ToString("yyyy/MM/dd");

                    var daysDiff = (item.NextExamDate.Value.Date - today).Days;
                    string status = "";
                    if (daysDiff < 0) status = $"متأخر ({Math.Abs(daysDiff)} أيام)";
                    else if (daysDiff == 0) status = "اليوم";
                    else status = $"بعد {daysDiff} أيام";

                    builder.AppendLine($"{patientName},{phone},{secPhone},{secOwner},{examDate},{status}");
                }

                var bom = new byte[] { 0xEF, 0xBB, 0xBF };
                var bytes = Encoding.UTF8.GetBytes(builder.ToString());
                var result = new byte[bom.Length + bytes.Length];
                Buffer.BlockCopy(bom, 0, result, 0, bom.Length);
                Buffer.BlockCopy(bytes, 0, result, bom.Length, bytes.Length);

                string fileName = $"مواعيد_المتابعة_{today:yyyy_MM_dd}.csv";
                return File(result, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تصدير المواعيد للإكسيل");
                return Content("حدث خطأ أثناء إعداد الملف، يرجى المحاولة لاحقاً.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UndoContacted(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam != null)
            {
                exam.IsReminderSent = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // 11. حذف فحص
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exam = await _context.Examinations.FindAsync(id);
                if (exam == null)
                    return Json(new { success = false, message = "الفحص محذوف أو غير موجود." });

                if (!string.IsNullOrEmpty(exam.GlassImagePath))
                {
                    _imageService.DeleteImage(exam.GlassImagePath);
                }

                _context.Examinations.Remove(exam);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم حذف الفحص." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "لا يمكن الحذف لارتباط الفحص بسجلات أخرى." });
            }
        }
    }
}