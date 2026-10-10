using System.Net.Http;
using System.Threading.Tasks;
namespace CodeCollabFrontend.Services;
public class ApiService
{
    private readonly HttpClient _httpClient;
    public ApiService(HttpClient httpClient) { _httpClient = httpClient; }
    public async Task<bool> IsWebSocketServerAlive()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:8000/health");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
