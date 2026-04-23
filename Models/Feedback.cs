using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartWaste.Models;

public partial class Feedback
{
    [Required]
    public int FeedbackId { get; set; }
    [Required]
    public int RequestId { get; set; }
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]

    public int? Rating { get; set; }
    [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]

    public string? Comment { get; set; }
    [DataType(DataType.DateTime)]
    [Display(Name = "Created At")]
    public DateTime? CreatedAt { get; set; }

    [Required]
    [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]

    public virtual PickupRequest Request { get; set; } = null!;
}
