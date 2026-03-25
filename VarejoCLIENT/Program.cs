using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VarejoCLIENT;
using VarejoCLIENT.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// IMPORTANTE: Barra "/" no final da URI
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7001/")
});

builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();