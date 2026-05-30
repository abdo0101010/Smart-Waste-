using System.Threading.Tasks;
using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IPaymentService
    {
        // 1. عملية الدفع العادية (من العميل للسيستم)
        Task<(Payment Payment, string RedirectUrl)> ProcessPaymentAsync(int requestId, int userId, string paymentMethod, decimal amount);
        Task<Payment> UpdateOrderSuccess(string specialReference);
        Task<Payment> UpdateOrderFailed(string specialReference);

        // 2. العملية العكسية (تحويل النقط لكاش على رقم المحفظة)
        Task<bool> TransferPointsToWalletAsync(int userId, string walletNumber, decimal pointsToRedeem);

        // 3. دالة التشفير والحماية لـ Paymob
        string ComputeHmacSHA512(string data, string secret);
    }
}