using FluentValidation.AspNetCore;
using ServicesApi.API.Middleware;
using ServicesApi.Application;
using ServicesApi.Application.Interfaces;
using ServicesApi.Infrastructure;
using ServicesApi.Infrastructure.Clients;
using ServicesApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var profilesUrl = builder.Configuration["Services:ProfilesApiUrl"] ?? throw new InvalidOperationException("ProfilesApiUrl is missing in configuration.");

builder.Services.AddHttpClient<IProfilesApiClient, ProfilesApiClient>(client =>
{
    client.BaseAddress = new Uri(profilesUrl);
});

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found");
DatabaseInitializer.Migrate(connectionString);

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();