using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class PickupRequest
{
    [Required]
    public int RequestId { get; set; }
    [Required]
    public int UserId { get; set; }

    public int? RecyclerId { get; set; }

    public int? HubStaffId { get; set; }
    [DataType(DataType.DateTime)]
    [Display(Name = "Request Date")]
    public DateTime? RequestDate { get; set; } = DateTime.Now;
    [DataType(DataType.DateTime)]
    [Display(Name = "Pickup Date")]
    public DateTime? PickupDate { get; set; }= DateTime.Now;
    [DataType(DataType.DateTime)]
    [Display(Name = "Verification Date")]
    public DateTime? VerificationDate { get; set; }=DateTime.Now;   
    [Required]
    public string Status { get; set; }
    [Required]

    public string? Qrcode { get; set; }
    [Required]
    [Range(10, 1000, ErrorMessage = "Estimated points must be between 10 and 1000.")]
        
    public decimal? EstimatedPoints { get; set; }
    [Required]
    [Range(10, 1000, ErrorMessage = "Final points must be between 10 and 1000.")]

    public decimal? FinalPoints { get; set; }

    public virtual ICollection<Feedback> ?Feedbacks { get; set; } = new List<Feedback>();

    public virtual HubStaff? HubStaff { get; set; }

    public virtual Recycler? Recycler { get; set; }

    public virtual ICollection<RequestItem> RequestItems { get; set; } = new List<RequestItem>();

    public virtual User User { get; set; } = null!;
}
