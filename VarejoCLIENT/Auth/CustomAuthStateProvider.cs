using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace VarejoCLIENT.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 1. Pega o token do LocalStorage via JSInterop
            var token = await _jsRuntime.InvokeAsync<string>("authInterop.lerToken");

            // Se não tem token, devolve um usuário anônimo
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // Se tem token, lê as informações dele e devolve o usuário logado
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        // Método chamado quando o usuário faz LOGIN
        public void NotificarLogin(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // Método chamado quando o usuário faz LOGOUT
        public void NotificarLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // --- MÉTODOS AUXILIARES PARA LER O JWT ---
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}