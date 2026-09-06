using System.ComponentModel.DataAnnotations;

namespace alkhaleejop.Models.Entity
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم الأب مطلوب")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        public string FamilyName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "العمر مطلوب")]
        public int Age { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }


        // تاريخ زيارة العميل (اختياري)
        public DateTime? RegistrationDate { get; set; }
        public bool IsVIP { get; set; } = false;


        // رقم هاتف احتياطي (اختياري)
        public string? SecondaryPhoneNumber { get; set; }

        // ملاحظة لمين الرقم (مثلاً: رقم الزوجة، رقم الابن) (اختياري)
        public string? SecondaryPhoneOwner { get; set; }

        // صار اختياري (لأنه كان فاضي بالصورة)
        public string? Job { get; set; }

        public ICollection<Examination> Examinations { get; set; }
    }
}
