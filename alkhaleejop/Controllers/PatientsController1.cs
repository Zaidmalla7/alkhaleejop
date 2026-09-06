using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using alkhaleejop.Data;
using alkhaleejop.Models.Entity;
using alkhaleejop.Models.ViewModels;
using System.Collections.Generic;

namespace alkhaleejop.Controllers
{
    [Authorize]
    public class PatientsController1 : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientsController1> _logger;

        public PatientsController1(ApplicationDbContext context, ILogger<PatientsController1> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientsData(string searchValue = "")
        {
            try
            {
                var query = _context.Patients.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.Trim();
                    query = query.Where(p =>
                        p.PhoneNumber.Contains(searchValue) ||
                        (p.SecondaryPhoneNumber != null && p.SecondaryPhoneNumber.Contains(searchValue)) ||
                        p.FirstName.Contains(searchValue) ||
                        p.FamilyName.Contains(searchValue));
                }

                var patients = await query.OrderByDescending(p => p.Id)
                    .Select(p => new
                    {
                        id = p.Id,
                        firstName = p.FirstName,
                        fatherName = p.FatherName,
                        familyName = p.FamilyName,
                        phoneNumber = p.PhoneNumber,
                        secondaryPhoneNumber = p.SecondaryPhoneNumber,
                        secondaryPhoneOwner = p.SecondaryPhoneOwner,
                        age = p.Age,
                        address = p.Address,
                        job = p.Job,
                        isVIP = p.IsVIP,
                        registrationDate = p.RegistrationDate
                    })
                    .ToListAsync();

                return Json(new { data = patients });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب بيانات العملاء.");
                return StatusCode(500, new { message = "حدث خطأ داخلي في الخادم." });
            }
        }

        [HttpGet]
        [Route("PatientsController1/GetPatientById/{id?}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            if (id <= 0 && Request.Query.ContainsKey("id"))
            {
                int.TryParse(Request.Query["id"], out id);
            }

            var patient = await _context.Patients.AsNoTracking()
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    FatherName = p.FatherName,
                    FamilyName = p.FamilyName,
                    PhoneNumber = p.PhoneNumber,
                    SecondaryPhoneNumber = p.SecondaryPhoneNumber,
                    SecondaryPhoneOwner = p.SecondaryPhoneOwner,
                    Age = p.Age,
                    Address = p.Address,
                    Job = p.Job,
                    IsVIP = p.IsVIP,
                    RegistrationDate = p.RegistrationDate
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return Json(new { success = false, message = "لم يتم العثور على ملف العميل." });
            }
            return Json(new { success = true, data = patient });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientHistory(int id)
        {
            try
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == id);
                if (!patientExists)
                {
                    return Json(new { success = false, message = "العميل غير موجود." });
                }

                var history = await _context.Examinations
                    .AsNoTracking()
                    .Where(e => e.PatientId == id)
                    .OrderByDescending(e => e.ExamDate)
                    .Select(e => new {
                        examId = e.Id,
                        examDate = e.ExamDate,
                        glassImagePath = e.GlassImagePath,

                        odSph = e.ODSph,
                        odCyl = e.ODCyl,
                        odAxis = e.ODAxis,
                        odAdd = e.ODAdd,
                        odVa = e.ODVA,

                        osSph = e.OSSph,
                        osCyl = e.OSCyl,
                        osAxis = e.OSAxis,
                        osAdd = e.OSAdd,
                        osVa = e.OSVA,

                        ipd = e.IPD,
                        remarks = e.Remarks,
                        purchaseNotes = e.PurchaseNotes,
                        examinerName = e.ExaminerName
                    })
                    .ToListAsync();

                if (!history.Any())
                {
                    return Json(new { success = true, hasHistory = false, message = "لا يوجد سجل فحوصات أو مشتريات لهذا العميل حتى الآن." });
                }

                return Json(new { success = true, hasHistory = true, data = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء جلب السجل الطبي للعميل رقم {Id}", id);
                return Json(new { success = false, message = "حدث خطأ أثناء جلب السجل الطبي، يرجى المحاولة لاحقاً." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "الرجاء التأكد من تعبئة الحقول الإجبارية.", errors });
            }

            try
            {
                var patientEntity = new Patient
                {
                    FirstName = model.FirstName.Trim(),
                    FatherName = model.FatherName?.Trim(),
                    FamilyName = model.FamilyName.Trim(),
                    PhoneNumber = model.PhoneNumber.Trim(),
                    SecondaryPhoneNumber = model.SecondaryPhoneNumber?.Trim(),
                    SecondaryPhoneOwner = model.SecondaryPhoneOwner?.Trim(),
                    Age = model.Age,
                    Address = model.Address?.Trim(),
                    Job = model.Job?.Trim(),
                    IsVIP = model.IsVIP,
                    RegistrationDate = model.RegistrationDate
                };

                _context.Patients.Add(patientEntity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم تسجيل العميل بنجاح!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إضافة عميل جديد. رقم الهاتف: {Phone}", model.PhoneNumber);
                return Json(new { success = false, message = "حدث خطأ أثناء حفظ البيانات، يرجى المحاولة لاحقاً." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "البيانات المدخلة غير صحيحة." });
            }

            try
            {
                var existingPatient = await _context.Patients.FindAsync(model.Id);
                if (existingPatient == null)
                {
                    return Json(new { success = false, message = "العميل غير موجود." });
                }

                existingPatient.FirstName = model.FirstName.Trim();
                existingPatient.FatherName = model.FatherName?.Trim();
                existingPatient.FamilyName = model.FamilyName.Trim();
                existingPatient.PhoneNumber = model.PhoneNumber.Trim();
                existingPatient.SecondaryPhoneNumber = model.SecondaryPhoneNumber?.Trim();
                existingPatient.SecondaryPhoneOwner = model.SecondaryPhoneOwner?.Trim();
                existingPatient.Age = model.Age;
                existingPatient.Address = model.Address?.Trim();
                existingPatient.Job = model.Job?.Trim();
                existingPatient.IsVIP = model.IsVIP;
                existingPatient.RegistrationDate = model.RegistrationDate;

                _context.Patients.Update(existingPatient);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم تحديث بيانات العميل بنجاح." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, message = "حدث تعارض في البيانات، يرجى تحديث الصفحة." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تعديل بيانات العميل رقم {Id}", model.Id);
                return Json(new { success = false, message = "حدث خطأ غير متوقع." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVIP(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return Json(new { success = false, message = "العميل غير موجود." });
                }

                patient.IsVIP = !patient.IsVIP;
                await _context.SaveChangesAsync();

                return Json(new { success = true, isVIP = patient.IsVIP, message = patient.IsVIP ? "تم تمييز العميل كـ VIP 🌟" : "تم إلغاء تمييز العميل" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تعديل حالة VIP للعميل رقم {Id}", id);
                return Json(new { success = false, message = "حدث خطأ أثناء تنفيذ العملية." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return Json(new { success = false, message = "العميل غير موجود أو تم حذفه مسبقاً." });
                }

                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم حذف العميل نهائياً." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء محاولة حذف العميل رقم {Id}", id);
                return Json(new { success = false, message = "لا يمكن الحذف. قد يكون هناك فحوصات مرتبطة بهذا العميل." });
            }
        }
    }
}