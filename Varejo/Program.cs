using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Microsoft.Extensions.DependencyInjection;
using Varejo.Interfaces;
using Varejo.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//DB
builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VarejoContext") ?? throw new InvalidOperationException("Connection string 'VarejoContext' not found.")));

builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//REPOSITORIOS
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IProdutoEmbalagemRepository, ProdutoEmbalagemRepository>();
builder.Services.AddScoped<ITipoEmbalagemRepository, TipoEmbalagemRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IFamiliaRepository, FamiliaRepository>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();




//data protection
var dpBase = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitrinoKeys");
Directory.CreateDirectory(dpBase);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dpBase));

// antiForgery (exigido em POST-PUT-DELETE)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "Vitrino.AntiCsrf";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.HeaderName = "X-CSRF-TOKEN";
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Vitrino.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.LoginPath = "/Usuario/Login";
        options.AccessDeniedPath = "/Usuario/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// MVC + JSON + AutoValidateAntiForgeryToken em métodos de escrita
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// (Opcional) CORS, se for usar front separado
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("PermitirTudo");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Rotas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();