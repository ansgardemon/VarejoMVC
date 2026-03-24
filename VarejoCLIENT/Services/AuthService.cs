using System.Net.Http.Json;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> Login(string email, string senha)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            email,
            senha
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var token = await response.Content.ReadAsStringAsync();

        // TODO: salvar token (localStorage depois)

        return true;
    }
}