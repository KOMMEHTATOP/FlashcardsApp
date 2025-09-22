using System.Security.Claims;
using System.Text.Json;

namespace FlashcardsBlazorUI.Helpers
{
    public static class JwtHelper
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
            return claims;
        }

        public static DateTime? GetTokenExpiry(string jwt)
        {
            try
            {
                var claims = ParseClaimsFromJwt(jwt);
                var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
                
                if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                }
            }
            catch
            {
                // Игнорируем ошибки парсинга
            }
            
            return null;
        }

        public static bool IsTokenExpired(string jwt)
        {
            var expiry = GetTokenExpiry(jwt);
            return expiry.HasValue && expiry.Value <= DateTime.UtcNow;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
