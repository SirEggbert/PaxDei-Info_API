using api.Data;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Secrets.json", false, true);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaxDei-Info API", Version = "v1" });
});

if (builder.Environment.IsDevelopment())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault:keyVaultURL");
    var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId");
    var keyVaultClientSecret = builder.Configuration.GetSection("DBSECRET");
    //var keyVaultClientSecret = builder.Configuration["ClientSecret"];
    var keyVaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID");

    var credential = new ClientSecretCredential(keyVaultDirectoryID.Value!.ToString(), keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString());

    builder.Configuration.AddAzureKeyVault(
       new Uri(keyVaultURL.Value), credential
       );

    var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), credential);
    var conStr = client.GetSecret("paxdei-db-server").Value.Value.ToString();

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(conStr);
    });
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString")));
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
