using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class UserRedemption
{
    [Required]
    public int RedemptionId { get; set; }

    [Required]

    public int UserId { get; set; }
    [Required]
    public int RewardId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Redemption Date")]
    public DateTime? RedeemedAt { get; set; }


    public string? VoucherCode { get; set; }

    public virtual Reward Reward { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
