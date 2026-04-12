using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VarejoCLIENT;
using VarejoCLIENT.Services;

// --- NOVOS USINGS PARA AUTENTICAÇÃO ---
using Microsoft.AspNetCore.Components.Authorization;
using VarejoCLIENT.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// IMPORTANTE: Barra "/" no final da URI
builder.Services.AddScoped(sp => new HttpClient
{
    // porta de consumo da API
    BaseAddress = new Uri("http://localhost:5018/")
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<RelatorioService>();

// --- LIGANDO A AUTORIZAÇÃO E O NOSSO "MAESTRO" (CUSTOM AUTH PROVIDER) ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();