using System.Text.Json;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Microsoft.Extensions.Options;
using Chima.Application.Common;

namespace Chima.Infrastructure.Authentication;

public class GoogleAuthService : IGoogleAuthService
{
    private const string AuthBase = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string UserInfoEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";

    private readonly GoogleSettings _settings;
    private readonly HttpClient _http;

    public GoogleAuthService(IOptions<GoogleSettings> settings, HttpClient http)
    {
        _settings = settings.Value;
        _http = http;
    }

    public string BuildAuthorizationUrl(string state)
    {
        var q = new Dictionary<string, string?>
        {
            ["client_id"] = _settings.ClientId,
            ["redirect_uri"] = _settings.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["access_type"] = "offline",
            ["prompt"] = "consent",
            ["state"] = state
        };
        var qs = string.Join("&", q.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? "")}"));
        return $"{AuthBase}?{qs}";
    }

    public async Task<GoogleUserInfo> ExchangeCodeAsync(string code, CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["redirect_uri"] = _settings.RedirectUri,
            ["grant_type"] = "authorization_code"
        };

        using var tokenResp = await _http.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
        var tokenJson = await tokenResp.Content.ReadAsStringAsync(ct);
        if (!tokenResp.IsSuccessStatusCode)
            throw new UnauthorizedException($"Google token exchange failed: {tokenJson}");

        using var tokenDoc = JsonDocument.Parse(tokenJson);
        var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString()
            ?? throw new UnauthorizedException("Missing access_token from Google.");

        using var req = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        using var userResp = await _http.SendAsync(req, ct);
        var userJson = await userResp.Content.ReadAsStringAsync(ct);
        if (!userResp.IsSuccessStatusCode)
            throw new UnauthorizedException($"Google userinfo fetch failed: {userJson}");

        using var userDoc = JsonDocument.Parse(userJson);
        var root = userDoc.RootElement;

        var sub = root.GetProperty("sub").GetString()!;
        var email = root.GetProperty("email").GetString()!;
        var verified = root.TryGetProperty("email_verified", out var v) && v.GetBoolean();
        var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;
        var picture = root.TryGetProperty("picture", out var p) ? p.GetString() : null;

        return new GoogleUserInfo(sub, email, verified, name, picture);
    }
}