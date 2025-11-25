using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Interfaces.Repository;
using Misa.Core.Interfaces.Service;
using Misa.Core.Services;
using Misa.Infrastructure.DatabaseConnectionProvider;
using Misa.Infrastructure.Repositories;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new MySqlConnectionProvider(connectionString));
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBaseService<Customer, string, string[], CustomerDto>, CustomerService>();



var app = builder.Build();
app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
