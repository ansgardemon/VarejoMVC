using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Microsoft.Extensions.DependencyInjection;
using Varejo.Interfaces;
using Varejo.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<VarejoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VarejoContext") ?? throw new InvalidOperationException("Connection string 'VarejoContext' not found.")));

builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IProdutoEmbalagemRepository, ProdutoEmbalagemRepository>();
builder.Services.AddScoped<ITipoEmbalagemRepository, TipoEmbalagemRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
