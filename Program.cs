using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext
builder.Services.AddDbContext<MentalHealth.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Identity
builder.Services.AddIdentity<MentalHealth.Data.Models.Korisnik, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MentalHealth.Data.AppDbContext>()
.AddDefaultTokenProviders();

// 3. Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key nije podešen. Lokalno: 'dotnet user-secrets set \"Jwt:Key\" \"<vrednost>\"'. " +
        "Hostovano: podesi environment varijablu Jwt__Key.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// 4. Add CORS
// Dozvoljeni origin-i se čitaju iz konfiguracije (Cors:AllowedOrigins) — lokalno su već
// podešeni u appsettings.json, a za hostovanu verziju se dodaju u appsettings.Production.json
// ili preko environment varijable Cors__AllowedOrigins__0, Cors__AllowedOrigins__1, ...
var dozvoljeniOrigini = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200", "https://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(dozvoljeniOrigini)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 5. Dependency Injection Services & Repositories
builder.Services.AddScoped<MentalHealth.Interfejsi.IAuthRepository, MentalHealth.Repository.AuthRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IKorisnikRepository, MentalHealth.Repository.KorisnikRepository>();
builder.Services.AddScoped<MentalHealth.Token.ITokenService, MentalHealth.Token.TokenService>();
builder.Services.AddScoped<MentalHealth.Data.Interfaces.IRecenzijaRepository, MentalHealth.Data.Repositories.RecenzijaRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IRaspolozenjeRepository, MentalHealth.Repository.RaspolozenjeRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IDnevnikMisliRepository, MentalHealth.Repository.DnevnikMisliRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.ISesijaRepository, MentalHealth.Repository.SesijaRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IZakazivanjeRepository, MentalHealth.Repository.ZakazivanjeRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IPorukaRepository, MentalHealth.Repository.PorukaRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IPodsetnikRepository, MentalHealth.Repository.PodsetnikRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.ISOSKontaktRepository, MentalHealth.Repository.SOSKontaktRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.IEdukativniSadrzajRepository, MentalHealth.Repository.EdukativniSadrzajRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.ICiljRepository, MentalHealth.Repository.CiljRepository>();
builder.Services.AddScoped<MentalHealth.Interfejsi.INavikaRepository, MentalHealth.Repository.NavikaRepository>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MentalHealth.Mappers.AutoMapperProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
    string[] roles = { "Klijent", "Terapeut", "Administrator" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

@app.Run();
