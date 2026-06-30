using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace SmartWaste.DTO.RecuclerDTOS
{
    public class UploadProfilePictureDTO
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
