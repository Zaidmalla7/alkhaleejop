using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alkhaleejop.Models.Entity
{
    public class Examination
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "تاريخ الفحص مطلوب")]
        public DateTime ExamDate { get; set; }

        public DateTime? NextExamDate { get; set; }

        // === الحقل الجديد لصورة النظارة ===
        public string? GlassImagePath { get; set; }
        // ---- قياسات العين اليمنى (O.D) ----
        public string? ODSph { get; set; }
        public string? ODCyl { get; set; }
        public string? ODAxis { get; set; }
        public string? ODAdd { get; set; } // كان فاضي بالصورة
        public string? ODVA { get; set; }

        // ---- قياسات العين اليسرى (O.S) ----
        public string? OSSph { get; set; }
        public string? OSCyl { get; set; }
        public string? OSAxis { get; set; }
        public string? OSAdd { get; set; } // كان فاضي بالصورة
        public string? OSVA { get; set; }


        public bool IsReminderSent { get; set; } = false; // هل تم إرسال التذكير؟
        public string? IPD { get; set; }

        public string? Remarks { get; set; } // كان فاضي بالصورة

        public string? PurchaseNotes { get; set; }
        public string? ExaminerName { get; set; }
    }
}
