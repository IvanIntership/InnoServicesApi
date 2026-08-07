using FluentValidation.AspNetCore;
using ServicesApi.Application;
using ServicesApi.Application.Interfaces;
using ServicesApi.Infrastructure;
using ServicesApi.Infrastructure.Http;
using ServicesApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddFluentValidationAutoValidation();

var profilesUrl = builder.Configuration["Services:ProfilesApiUrl"] ?? throw new InvalidOperationException("ProfilesApiUrl is missing in configuration.");

builder.Services.AddHttpClient<IProfilesApiClient, ProfilesApiClient>(client =>
{
    client.BaseAddress = new Uri(profilesUrl);
});

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found");
DatabaseInitializer.Migrate(connectionString);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();