        using System.Net.Http.Json;
using SmartWaste.DTO.HubStaffDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class HubStaffService : IHubStaffService
    {
        IHubStaffRepository _hubStaffRepository;
        public HubStaffService(IHubStaffRepository hubStaffRepository)
        {
            _hubStaffRepository = hubStaffRepository;
        }
        public void AddHubStaff(HubstaffCreationsDto hubStaff)
        {
            if (hubStaff != null)
            {
                _hubStaffRepository.AddHubStaff(hubStaff);
               

            }
            else           {
                throw new ArgumentNullException(nameof(hubStaff), "HubStaff cannot be null.");
            }


        }
public HubStaff GetHubStaffByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _hubStaffRepository.GetHubStaffByName(name);
            }
            return null;
        }

        public void DeleteHubStaff(int id)
        {
            if(id > 0)
            {
                _hubStaffRepository.DeleteHubStaff(id);
            }
        }

        public IEnumerable<HubStaff> GetAllHubStaff()
        {
            return _hubStaffRepository.GetAllHubStaff();
        }

        public List<HubStaff> GetAllHubStaffWithPickupRequests()
        {
            return _hubStaffRepository.GetAllHubStaffWithPickupRequests();
        }

        public HubStaff GetHubStaffById(int id)
        {
            if(id > 0)
            {
                return _hubStaffRepository.GetHubStaffById(id);
            }
            return null;
        }

        public void SaveChanges()
        {
            _hubStaffRepository.SaveChanges();
        }

        public void UpdateHubStaff(HubStaff hubStaff)
        {
            if(hubStaff != null)
            {
                _hubStaffRepository.UpdateHubStaff(hubStaff);
            }
        }

public async Task<bool> VerifyShipmentWithAIAsync(IFormFile fileBefore, IFormFile fileAfter, int transactionId)
    {
        using var httpClient = new HttpClient();
        string aiApiUrl = "https://badass-ecosystem-hazy.ngrok-free.dev/verify-shipment/";

        using var content = new MultipartFormDataContent();

        // 1. تجهيز وإضافة الصورة الأولى (file_before)
        using var streamBefore = fileBefore.OpenReadStream();
        using var contentBefore = new StreamContent(streamBefore);
        content.Add(contentBefore, "file_before", fileBefore.FileName);

        // 2. تجهيز وإضافة الصورة الثانية (file_after)
        using var streamAfter = fileAfter.OpenReadStream();
        using var contentAfter = new StreamContent(streamAfter);
        content.Add(contentAfter, "file_after", fileAfter.FileName);

        // 3. إضافة الـ transaction_id كـ StringContent حسب طلب الموديل
        content.Add(new StringContent(transactionId.ToString()), "transaction_id");

        // 🚀 ضرب الـ Request للموديل لايف
        var response = await httpClient.PostAsync(aiApiUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI Verification Failed. Status: {response.StatusCode}, Details: {errorContent}");
        }

        // 4. قراءة الـ Response المرن
        var jsonResult = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (jsonResult != null && jsonResult.TryGetValue("status", out var statusObj))
        {
            string status = statusObj.ToString();
            if (status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                // 🔥 هنا تقدر تحدث حالة الطلب في الداتابيز بتاعتك لـ Verified أو Approved
                // await _pickupRequestRepository.UpdateStatusToVerifiedAsync(transactionId);
                return true;
            }
        }

        return false;
    }
}
}
