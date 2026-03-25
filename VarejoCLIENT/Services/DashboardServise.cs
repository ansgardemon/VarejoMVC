using System.Net.Http.Json;
using VarejoSHARED.DTO;

namespace VarejoCLIENT.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;
        public DashboardService(HttpClient http) => _http = http;

        public async Task<DashboardDTO?> GetDashboardAsync()
        {
            return await _http.GetFromJsonAsync<DashboardDTO>("api/Dashboard");
        }
    }
}