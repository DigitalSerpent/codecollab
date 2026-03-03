using System.Net.Http;
using System.Threading.Tasks;

namespace CodeCollabFrontend.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetHello()
    {
        return await _httpClient.GetStringAsync("http://localhost:8000");
    }
}