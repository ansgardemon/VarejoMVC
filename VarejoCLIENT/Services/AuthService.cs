using System.Net.Http.Json;
using System.Text.Json;
using VarejoSHARED.DTO;

namespace VarejoCLIENT.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public string? NomeUsuario { get; private set; }
    public int? UsuarioId { get; private set; }
    public bool IsAutenticado => !string.IsNullOrEmpty(NomeUsuario);

    public AuthService(HttpClient http) => _http = http;

    public async Task<bool> LoginAsync(string user, string password)
    {
        try
        {
            var loginDto = new LoginDTO { nomeUsuario = user, senha = password };

            // Sem barra inicial aqui pois o BaseAddress já tem barra no final
            var response = await _http.PostAsJsonAsync("api/Usuario/Login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var dados = await response.Content.ReadFromJsonAsync<LoginResult>(_options);

                if (dados != null)
                {
                    NomeUsuario = dados.nomeUsuario;
                    UsuarioId = dados.IdUsuario;
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro crítico no login: {ex.Message}");
            return false;
        }
    }

    private class LoginResult
    {
        public int IdUsuario { get; set; }
        public string nomeUsuario { get; set; } = string.Empty;
    }

    public void Logout()
    {
        NomeUsuario = null;
        UsuarioId = null;
    }
}