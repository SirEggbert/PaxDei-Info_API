using api.Data;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secrets.json",false);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaxDei-Info API", Version = "v1" });
});

var useLocalDatabase = builder.Configuration.GetValue("UseLocalDatabase",false);

if (useLocalDatabase)
{
    builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString")));
}
else {
    var keyVaultURL = builder.Configuration["KeyVault:keyVaultURL"];
    var keyVaultClientId = builder.Configuration["KeyVault:ClientId"];
    var keyVaultClientSecret = builder.Configuration["KeyVault:ClientSecret"];
    var keyVaultDirectoryID = builder.Configuration["KeyVault:DirectoryID"];

    var credential = new ClientSecretCredential(keyVaultDirectoryID, keyVaultClientId, keyVaultClientSecret);

    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultURL), credential);

    var client = new SecretClient(new Uri(keyVaultURL), credential);
    var conStr = client.GetSecret("paxdei-db-server").Value.Value;

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(conStr);
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaxDei-Info API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
