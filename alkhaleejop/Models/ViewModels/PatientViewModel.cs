using System.ComponentModel.DataAnnotations;

namespace alkhaleejop.Models.ViewModels
{
    public class PatientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الأب مطلوب")]
        public string FatherName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        public string FamilyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "رقم الهاتف غير صالح، يجب أن يحتوي على أرقام فقط (من 9 إلى 15 رقم).")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "العمر مطلوب")]
        public int Age { get; set; }


        [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "رقم الهاتف غير صالح، يجب أن يحتوي على أرقام فقط (من 9 إلى 15 رقم)")]
        public string? SecondaryPhoneNumber { get; set; }
        public bool IsVIP { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public string? SecondaryPhoneOwner { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; } = string.Empty;

        // مسار اختياري
        public string? Job { get; set; }
    }
}
