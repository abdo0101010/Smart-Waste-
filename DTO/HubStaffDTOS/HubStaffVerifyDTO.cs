using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.HubStaffDTOS
{
    public class HubStaffVerifyDTO
    {
        [Required(ErrorMessage = "برجاء إدخال رقم العملية")]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "برجاء رفع صورة الاستلام وفحص الـ AI")]
        public IFormFile FileAfter { get; set; }
    }
}
