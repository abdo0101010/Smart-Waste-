using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace SmartWaste.DTO.UserDTOS
{
    public class UserPhotoUploadDTO
    {
        [Required(ErrorMessage = "برجاء اختيار صورة أولاً.")]
        public IFormFile ProfilePicture { get; set; } = null!;
    }
}
