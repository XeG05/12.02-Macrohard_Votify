// Ejecutar UNA sola vez antes de desplegar el cambio de BCrypt
// dotnet run --project MigrarContraseñas

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Votify.Persistencia;

var connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.jqositjphwyqugwzlzcy;Password=VotifyManzana14;SSL Mode=Require;Trust Server Certificate=true;Keepalive=60;Command Timeout=60;Pooling=true;Maximum Pool Size=20;No Reset On Close=true;";

var options = new DbContextOptionsBuilder<VotifyContext>()
    .UseNpgsql(connectionString)
    .Options;

await using var context = new VotifyContext(options);

var usuarios = await context.Usuarios.ToListAsync();

foreach (var usuario in usuarios)
{
    // Idempotente: si ya empieza con $2 es BCrypt, se salta
    if (usuario.Contraseña.StartsWith("$2"))
        continue;

    usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
}

await context.SaveChangesAsync();
Console.WriteLine($"Migrados {usuarios.Count} usuarios.");