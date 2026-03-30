using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VarejoSHARED.DTO;

namespace VarejoCLIENT.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDTO?> GetDashboardAsync()
        {
            return await _httpClient.GetFromJsonAsync<DashboardDTO>("api/dashboard");
        }
    }
}