using LibriApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using LibriApi.Services;
using System.Text;
using LibriApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using LibriApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecurityTransformer>();
});
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<LibriDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});
builder.Services.AddScoped<ILibriService, LibriService>();
builder.Services.AddSingleton<IPasswordHasher<Utente>, PasswordHasher<Utente>>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseExceptionHandler();              
app.UseStatusCodePages();               

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "LibriApi v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }