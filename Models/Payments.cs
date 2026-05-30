using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWaste.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [Column("PaymentID")]
        public int PaymentId { get; set; }

        // 👇 ضيفي السطرين دول هنا عشان يقرأ الـ RequestID
        [Required]
        [Column("RequestID")]
        public int RequestID { get; set; }

        [Required]
        [Column("Amount", TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(100)]
        [Column("TransactionId")]
        public string TransactionId { get; set; } = null!;

        [Column("PaymentDate")]
        public DateTime? PaymentDate { get; set; } = DateTime.Now;

        // 👇 عمل العلاقة مع جدول طلبات التجميع الأساسي عندك
        [ForeignKey("RequestID")]
        public virtual PickupRequest? PickupRequest { get; set; }
    }
}