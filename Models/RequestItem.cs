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
    public string? RequestImageUrl { get; set; }

    // 2️⃣ مكان حفظ صورة الـ Hub Staff (صورة المخزن/الميزان عند الاستلام - file_after)
    public string? VerificationImageUrl { get; set; }

    // 3️⃣ العدد النهائي للزجاجات اللي الـ AI هيحسبه في الـ Hub Staff لـ تأكيد النقاط
    public int? FinalBottlesCount { get; set; }
    [Required]


    public string Source { get; set; } = null!;

    public virtual WasteCategory Category { get; set; } = null!;

    public virtual PickupRequest Request { get; set; } = null!;
}
