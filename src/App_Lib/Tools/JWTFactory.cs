using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace src.App_Lib.Tools;

// https://codeburst.io/jwt-auth-in-asp-net-core-148fb72bed03

// dotnet add package Microsoft.IdentityModel.Tokens
// dotnet add package System.IdentityModel.Tokens.Jwt

// Internals
public partial class JWTFactory
{
    private readonly ClaimsIdentity? _claimsIdentity;
    private readonly string _issuer;
    private readonly string _signKey;
    private readonly string? _encryptKey;

    private SecurityKey GetSecurityKey(string key)
    {
        // return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key.ToBase64()));
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }

    private SigningCredentials GetSigningCredentials()
    {
        SigningCredentials signCredentials = new SigningCredentials(
            key: GetSecurityKey(_signKey),
            algorithm: SecurityAlgorithms.HmacSha256Signature
        );

        return signCredentials;
    }

    private EncryptingCredentials GetEncryptingCredentials(string key)
    {
        SecurityKey encSecKey = GetSecurityKey(key);

        var (al, en) = encSecKey.KeySize switch
        {
            128 => (SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256),
            192 => (SecurityAlgorithms.Aes192KeyWrap, SecurityAlgorithms.Aes192CbcHmacSha384),
            256 => (SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512),
            _ => throw new ArgumentException("Size of encryption key can be 128, 192, or 256 bits")
        };

        EncryptingCredentials encCredentials = new EncryptingCredentials(key: encSecKey, alg: al, enc: en);

        return encCredentials;
    }

    private SecurityTokenDescriptor GetSecurityTokenDescriptor(string audience, int timeout)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Subject = _claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(timeout),
            Issuer = _issuer,
            Audience = audience,
            SigningCredentials = GetSigningCredentials()
        };

        if (!string.IsNullOrEmpty(_encryptKey)) tokenDescriptor.EncryptingCredentials = GetEncryptingCredentials(_encryptKey);

        return tokenDescriptor;
    }

    private TokenValidationParameters GetTokenValidationParameters(string validAudience = "*", int clockSkew = 1)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = validAudience,
            IssuerSigningKey = GetSecurityKey(_signKey),
            ClockSkew = TimeSpan.FromMinutes(clockSkew)
        };

        if (!string.IsNullOrEmpty(_encryptKey))
            tokenValidationParameters.TokenDecryptionKey = GetSecurityKey(_encryptKey);

        return tokenValidationParameters;
    }
}

// Externals
public partial class JWTFactory
{
    /// <summary>
    /// JWT Token Factory
    /// </summary>
    /// <param name="claimsIdentity">If NOT provided, only validation is available.</param>
    /// <param name="issuer">If NOT provided, assembly manifest module name will be used.</param>
    /// <param name="signKey">If NOT provided, assembly manifest module stream version will be used.</param>
    /// <param name="encryptKey">If provided, token will be encrypted. Key size can be 128,192 or 256 bits.</param>
    public JWTFactory(ClaimsIdentity? claimsIdentity = null, string? issuer = null, string? signKey = null, string? encryptKey = null)
    {
#if DEBUG
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif

        _claimsIdentity = claimsIdentity;
        _issuer = issuer ?? Assembly.GetExecutingAssembly().ManifestModule.Name;
        _signKey = signKey ?? Assembly.GetExecutingAssembly().ManifestModule.MDStreamVersion.ToString();
        _encryptKey = encryptKey;
    }

    public string CreateToken(string audience = "*", int timeout = 20)
    {
        //var jwtToken = new JwtSecurityToken(
        //    issuer: _issuer,
        //    audience: audience,
        //    claims: _claimsIdentity.Claims,
        //    notBefore: DateTime.UtcNow,
        //    expires: DateTime.Now.AddMinutes(timeout),
        //    signingCredentials: GetSigningCredentials()                
        //);
        //string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        // if (_claimsIdentity is null) throw new ArgumentException($"{nameof(ClaimsIdentity)} is null.");

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        // SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor());
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptor(audience, timeout));
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string tokenStr, out string message, out SecurityToken? token, string validAudience = "*", int clockSkew = 1)
    {
        try
        {
            TokenValidationParameters tvp = GetTokenValidationParameters(validAudience, clockSkew);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal user = tokenHandler.ValidateToken(tokenStr, tvp, out token);
            message = "SUCCESS: Token is validated.";
            return true;
        }
        catch (Exception ex)
        {
            message = $"FAIL: {ex.Message}";
            token = null;
            return false;
        }
    }
}
