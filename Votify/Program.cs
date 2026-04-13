using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Votify.Auth;
using Votify.Components;
using Votify.LógicaNegocio.Factory;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia;
using Votify.Persistencia.Interfaces;
using Votify.Persistencia.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<EventoCreator>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();




builder.Services.AddScoped<JwtService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// BlazorAuthStateProvider scoped al circuito SignalR
builder.Services.AddScoped<BlazorAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<BlazorAuthStateProvider>());

// Contexto de evento activo scoped al circuito
builder.Services.AddScoped<EventoContextService>();

// Soporte para [Authorize] en componentes Blazor
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// JWT Bearer (para validación, aunque el estado vive en el circuito)
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<VotifyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
