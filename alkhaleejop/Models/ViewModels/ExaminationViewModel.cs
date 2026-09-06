using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace alkhaleejop.Models.ViewModels
{
    public class ExaminationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المريض")]
        public int PatientId { get; set; }

        public DateTime? NextExamDate { get; set; }

        // ==========================================
        // حقول الصورة الجديدة (موجودة فقط بالـ ViewModel)
        // ==========================================

        // 1. لاستقبال الملف من الفورم (الواجهة الأمامية)
        public IFormFile? ImageFile { get; set; }

        // 2. لعرض مسار الصورة الحالية (في حالة التعديل عشان نعرضها للموظف)
        public string? GlassImagePath { get; set; }

        // ==========================================
        // قياسات العين اليمنى (O.D)
        // ==========================================
        public string? ODSph { get; set; }
        public string? ODCyl { get; set; }
        public string? ODAxis { get; set; }
        // ونفس الإشي للعين اليسرى

        public string? ODAdd { get; set; }
        public string? ODVA { get; set; }

        // ==========================================
        // قياسات العين اليسرى (O.S)
        // ==========================================
        public string? OSSph { get; set; }
        public string? OSCyl { get; set; } 
        public string? OSAxis { get; set; } 

        public string? OSAdd { get; set; }
        public string? OSVA { get; set; }

        // ==========================================
        // تفاصيل إضافية
        // ==========================================
        public string? IPD { get; set; }
        public string? Remarks { get; set; }
        public string? PurchaseNotes { get; set; }
        public string? ExaminerName { get; set; }
    }
}