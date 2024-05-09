using APIKeyValidatorMiddelware.Services;
using Microsoft.EntityFrameworkCore;
using TestEquipment_Test;
using TestEquipment_Test.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//***************** agrega servicio de api key ***************************/

builder.Services.AddScoped<ApiKeyService>(x=> new ApiKeyService(builder.Configuration.GetConnectionString("DefaultConnection")));

//********************************************/

builder.Services.AddControllers();
builder.Services.AddDbContext<BDContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
