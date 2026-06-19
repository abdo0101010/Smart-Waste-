using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWaste.Models
{
    [Table("WalletRedemptions")] // لضمان إنشاء الجدول بنفس الاسم في الداتا بيز
    public class WalletRedemption
    {
        [Key]
        [Column("RedemptionID")]
        public int RedemptionId { get; set; }

        [Required]
        [Column("UserID")]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("WalletNumber")]
        public string WalletNumber { get; set; } = null!;

        [Required]
        [Column("PointsRedeemed", TypeName = "decimal(10, 2)")]
        public decimal PointsRedeemed { get; set; }

        [Required]
        [Column("AmountEGP", TypeName = "decimal(10, 2)")]
        public decimal AmountEgp { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        [Column("TransactionID")]
        public string? TransactionId { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}