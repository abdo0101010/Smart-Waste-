using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class RequestItem
{
    [Required]

    public int ItemId { get; set; }
    [Required]
    public int RequestId { get; set; }
    [Required]

    public int CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }
    [Required]


    public string Source { get; set; } = null!;

    public virtual WasteCategory Category { get; set; } = null!;

    public virtual PickupRequest Request { get; set; } = null!;
}
