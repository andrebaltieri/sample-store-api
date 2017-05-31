using System;
using System.Linq;
using Sample.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Security;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Sample.Api.Models
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;

        public AccountController(IOptions<JwtIssuerOptions> jwtOptions, DataContext context)
        {
            _context = context;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [Route("v1/authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromForm] User user)
        {
            var identity = await GetClaimsIdentity(_context, user.Username, user.Password);
            if (identity == null)
                return BadRequest("Usuário ou senha inválidos");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Username),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.Username),
                identity.FindFirst("Store")
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims.AsEnumerable(),
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt,
                expireDate = (int)_jwtOptions.ValidFor.TotalSeconds,
                username = user.Username,
                otherData = "Valor custom"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("O período deve ser maior que zero", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private static Task<ClaimsIdentity> GetClaimsIdentity(DataContext context, string username, string password)
        {
            var user = context.Customers.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
            {
                return Task.FromResult(new ClaimsIdentity(
                    new GenericIdentity(username, "Token"),
                    new[]
                    {
                        new Claim("Store", user.Role)
                    }));
            }

            return Task.FromResult<ClaimsIdentity>(null);
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}