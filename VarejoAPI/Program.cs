using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 1. Configurar o DbContext
// ==========================
builder.Services.AddDbContext<VarejoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// 2. Repositórios (Lista Completa)
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
builder.Services.AddScoped<IValidadeRepository, ValidadeRepository>();
builder.Services.AddScoped<ITipoMovimentoRepository, TipoMovimentoRepository>();
builder.Services.AddScoped<IFormaPagamentoRepository, FormaPagamentoRepository>();
builder.Services.AddScoped<IPrazoPagamentoRepository, PrazoPagamentoRepository>();
builder.Services.AddScoped<IEspecieTituloRepository, EspecieTituloRepository>();
builder.Services.AddScoped<ITituloFinanceiroRepository, TituloFinanceiroRepository>();
builder.Services.AddScoped<IPagamentoTituloRepository, PagamentoTituloRepository>();



// ==========================
// 3. Configurar CORS (Permissivo para facilitar o TCC)
// ==========================
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==========================
// 4. Serviços do ASP.NET
// ==========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necessário para o Swagger moderno
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================
// 5. Middleware Pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// O CORS DEVE vir antes de quase tudo para evitar erros no navegador
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();