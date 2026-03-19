using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Votify.Modelos;

namespace Votify.Auth
{
    // Scoped: vive mientras dure la conexión SignalR del usuario
    public class BlazorAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser =
            new ClaimsPrincipal(new ClaimsIdentity());

        private readonly JwtService _jwtService;

        public BlazorAuthStateProvider(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(_currentUser));

        public void MarcarAutenticado(string jwtToken)
        {
            var principal = _jwtService.ValidateToken(jwtToken);
            if (principal != null)
            {
                _currentUser = principal;
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public void MarcarDesconectado()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public string? ObtenerCorreo()
            => _currentUser.FindFirst(ClaimTypes.Name)?.Value;

        public string? ObtenerNombre()
            => _currentUser.FindFirst("nombre")?.Value;

        public bool EstaAutenticado
            => _currentUser.Identity?.IsAuthenticated ?? false;
    }
}