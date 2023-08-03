using CSharp_Challenge.Controllers.Authentication;
using CSharp_Core.Repository;
using CSharp_Data;
using CSharp_Data.DataInitializer;
using CSharp_Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var Connect = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(Connect.GetConnectionString("MyDb"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>() // Replace YourDbContext with your actual DbContext
        .AddDefaultTokenProviders();
builder.Services.AddScoped<AuthController>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<List<Fruits>>(); // Add the Fruits service here
//Authentication registration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:JWTSigningkey").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Adding Documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Csharp_Challenge", Version = "v1.0.0" });
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    };
    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Use authentication before authorization
app.UseAuthorization();

// Get the service scope and obtain the necessary services
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var context = serviceProvider.GetRequiredService<AppDbContext>();
var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var fruits = serviceProvider.GetRequiredService<List<Fruits>>(); // Get the Fruits service

UserandRolesInitializedData.SeedData(context, userManager, roleManager, fruits).Wait();

app.MapControllers();

app.Run();
