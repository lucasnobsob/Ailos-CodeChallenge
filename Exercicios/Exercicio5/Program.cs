using Exercicio5.Application.Movimentacao.Services;
using Exercicio5.Domain.Interfaces;
using MediatR;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Database;
using Questao5.Infrastructure.Repositories;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "Data Source=database.sqlite";

// Database
builder.Services.AddSingleton<IDatabaseConnection>(provider => new DatabaseConnection(connectionString));
builder.Services.AddScoped<IDbConnection>(provider => provider.GetService<IDatabaseConnection>()!.CreateConnection());

// Repositories
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IMovimentacaoQueryService, MovimentacaoQueryService>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
DatabaseBootstrap.InitializeDatabase(connectionString);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


