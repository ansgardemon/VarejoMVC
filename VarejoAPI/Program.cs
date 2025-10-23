using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Configurar o DbContext
// ==========================
builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// Repositórios
// ==========================
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

// ==========================
// Adicionar CORS
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLanding", policy =>
    {
        policy.WithOrigins("http://localhost:5050") // URL da sua landing
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================
// Serviços do ASP.NET
// ==========================
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================
// Middleware
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS antes do MapControllers
app.UseCors("AllowLanding");

app.UseAuthorization();

app.MapControllers();

app.Run();
