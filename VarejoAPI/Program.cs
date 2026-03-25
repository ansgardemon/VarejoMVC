using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Banco de Dados
builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
// ... adicione os outros Scoped aqui conforme necessário ...

// 3. Configurar Política de CORS (Agressiva para Desenvolvimento)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// MIDDLEWARE PIPELINE (A ORDEM É TUDO AQUI)
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// O CORS deve ser o primeiro após o Swagger
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();