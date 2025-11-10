using Api.Data;
using Api.Domain.Enums;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// --- Neon / Npgsql enum mapping for user_role ---

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection missing.");
var dsb = new Npgsql.NpgsqlDataSourceBuilder(connString);
dsb.MapEnum<UserRole>("user_role");
var dataSource = dsb.Build();

builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseNpgsql(dataSource));

builder.Services.AddControllers();


// --- JWT auth ---
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = key
    };
});


// Role policies (optional convenience)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", p => p.RequireRole("ADMIN"));
    options.AddPolicy("IsManagerOrAbove", p => p.RequireRole("MANAGER", "HEAD", "ADMIN"));
});


builder.Services.AddScoped<JwtTokenService>();


var app = builder.Build();


// Auto-migrate + seed (dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();