using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using Votify.Auth;

public class BlazorAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser =
        new ClaimsPrincipal(new ClaimsIdentity());

    private readonly JwtService _jwtService;
    private readonly IJSRuntime _js;
    private bool _initialized = false;

    public BlazorAuthStateProvider(JwtService jwtService, IJSRuntime js)
    {
        _jwtService = jwtService;
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_initialized)
        {
            try
            {
                var token = await _js.InvokeAsync<string?>("authStorage.getToken");
                if (!string.IsNullOrEmpty(token))
                {
                    var principal = _jwtService.ValidateToken(token);
                    if (principal != null)
                        _currentUser = principal;
                }
                _initialized = true;
            }
            catch
            {
                
            }
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task MarcarAutenticado(string jwtToken)
    {
        var principal = _jwtService.ValidateToken(jwtToken);
        if (principal != null)
        {
            _currentUser = principal;
            _initialized = true; // ← evita que sobreescriba el estado tras login
            await _js.InvokeVoidAsync("authStorage.saveToken", jwtToken);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public async Task MarcarDesconectado()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        _initialized = true;
        await _js.InvokeVoidAsync("authStorage.removeToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public string? ObtenerCorreo()
        => _currentUser.FindFirst(ClaimTypes.Name)?.Value;

    public string? ObtenerNombre()
        => _currentUser.FindFirst("nombre")?.Value;

    public bool EstaAutenticado
        => _currentUser.Identity?.IsAuthenticated ?? false;
}