using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MISBack.Data;
using MISBack.Services;
using MISBack.Services.Interfaces;
using MISBack.Services.ValidateTokenPolicy;
using MISBack.Configs;
using MISBack.Services.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Services
builder.Services.AddScoped<IDoctorsService, DoctorsService>();

// Auth
builder.Services.AddSingleton<IAuthorizationHandler, ValidateTokenRequirementHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "ValidateToken",
        policy => policy.Requirements.Add(new ValidateTokenRequirement()));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtConfigurations.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtConfigurations.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

// DB
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Exceptions
app.UseExceptionHandlerMiddleware();

// Auto Migration
using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
context?.Database.Migrate();

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
