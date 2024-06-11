using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implementations;
using HomeBankingMindHub.Servicies;
using HomeBankingMindHub.Servicies.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Extra
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConnection")));

// Servicies
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IClientLoanRepository, ClientLoanRepository>();

// Add authentication to the container
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtScheme";
    options.DefaultChallengeScheme = "JwtScheme";
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("JwtScheme", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

    // Add authorization to the container
    builder.Services.AddAuthorization(
        options =>
        {
            options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
        });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    //todos los servicios que hay en la app
    var services = scope.ServiceProvider;
    try
    {
        //recuperamos el servicio de contexto de HomeBankingContext
        var context = services.GetRequiredService<HomeBankingContext>();
        DBInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
} else
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

// For authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
