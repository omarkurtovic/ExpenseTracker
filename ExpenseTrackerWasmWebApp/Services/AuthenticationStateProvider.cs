using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Text.Json;

namespace ExpenseTrackerWasmWebApp.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(token);
        var expiry = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (IsTokenExpired(expiry))
        {
            await _localStorage.RemoveItemAsync("authToken");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
    }

    private List<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        var claims = new List<Claim>();
        foreach (var kvp in keyValuePairs!)
        {
            claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
        }
        
        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private bool IsTokenExpired(string? expiry)
    {
        if (string.IsNullOrEmpty(expiry)) return true;
        
        var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry));
        return expiryDateTime < DateTimeOffset.UtcNow;
    }
}