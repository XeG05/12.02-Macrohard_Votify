
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Votify.Auth;
using Votify.Components;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia;
using Votify.Persistencia.Interfaces;
using Votify.Persistencia.Repositories;
using Votify.LógicaNegocio.Observador.Observers;
using Votify.LógicaNegocio.Observador.Sujetos;
using Votify.LógicaNegocio.Observador.Interfaces;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(
builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AdministradorCreator>();
builder.Services.AddScoped<VotanteCreator>();
builder.Services.AddScoped<JuradoCreator>();
builder.Services.AddScoped<ParticipanteCreator>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEventoAdministradorService, EventoAdministradorService>();
builder.Services.AddScoped<IEventoParticipanteService, EventoParticipanteService>();
builder.Services.AddScoped<IEventoVotanteService, EventoVotanteService>();
builder.Services.AddScoped<IEventoJuradoService, EventoJuradoService>();
builder.Services.AddScoped<IInvitacionService, InvitacionService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<IEventoSujeto, EventoSujeto>();

builder.Services.AddScoped<INotificacionService, NotificacionService>();

builder.Services.AddScoped<IObservadorEvento, ObservadorParticipantes>();
builder.Services.AddScoped<IObservadorEvento, ObservadorVotantes>();
builder.Services.AddScoped<IObservadorEvento, ObservadorJurado>();
builder.Services.AddScoped<IObservadorEvento, ObservadorFinalizacion>();
builder.Services.AddScoped<IObservadorEvento, ObservadorCertificados>();


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

var geminiApiKey = builder.Configuration["Gemini:ApiKey"];

builder.Services.AddHttpClient<Votify.LógicaNegocio.Services.SíntesisFeedbackService>(client =>
{
    // Configuraciones del cliente (si es necesario)
})
.AddTypedClient<Votify.LógicaNegocio.Services.SíntesisFeedbackService>(httpClient =>
    new Votify.LógicaNegocio.Services.SíntesisFeedbackService(httpClient, geminiApiKey));
var app = builder.Build();

// Resincronizar la secuencia de la tabla Proyecto automáticamente
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<VotifyContext>();
        // Esto le dice a PostgreSQL: "arregla auto-incremento para que empiece después del ID más alto que exista"
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('proyecto', 'id'), coalesce(max(id), 1), max(id) IS NOT null) FROM proyecto;");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"No se pudo sincronizar la secuencia de Postgres: {ex.Message}");
    }
}

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = ex switch
    {
        NotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedException => StatusCodes.Status401Unauthorized,
        ConflictException => StatusCodes.Status409Conflict,
        ValidationException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };
    await context.Response.WriteAsJsonAsync(new { error = ex?.Message ?? "Error interno" });
}));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
