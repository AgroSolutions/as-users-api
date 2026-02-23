using AS.Users.API.Configurations;
using AS.Users.API.Observability;
using AS.Users.Application.Middleware;
using AS.Users.Application.Observability;
using AS.Users.Application.Services;
using AS.Users.Application.Services.Interfaces;
using AS.Users.Domain.Entities;
using AS.Users.Domain.Interfaces;
using AS.Users.Infra.Data.Seedings;
using AS.Users.Infra.Persistence.Data;
using AS.Users.Infra.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ✅ Connection String
builder.Services.AddDbContextFactory<ASDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    );
});

// ✅ Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ASDbContext>()
.AddDefaultTokenProviders();

// ✅ JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var chaveSecreta = jwtSettings["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta!)),
        ClockSkew = TimeSpan.Zero
    };
});


// ✅ Serviços
builder.Services.AddTransient<JwtService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddSingleton<IUserTelemetry, PrometheusUserTelemetry>();
builder.Services.AddSwaggerConfiguration();


// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();

// ✅ Controllers
builder.Services.AddControllers();

builder.Logging.AddJsonConsole();



var app = builder.Build();

// ✅ Pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwaggerConfiguration();
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ASDbContext>();
        if (dbContext.Database.EnsureCreated())
        {
            await RoleAndAdminSeeding.SeedAsync(scope.ServiceProvider);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar banco de dados: {ex.Message}");
    }
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseExceptionHandler("/Error");
app.UseHsts();



app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();
app.UseHttpMetrics();
app.MapMetrics();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();