using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "GetToken")]
        public async Task<IResult> GetToken(User user)
        {

            var issuer = _configuration.GetValue<string>("Jwt:Issuer");// builder.Configuration["Jwt:Issuer"];
            var audience = _configuration.GetValue<string>("Jwt:Audience");// builder.Configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));//builder.Configuration["Jwt:Key"]
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                      new Claim("Id", Guid.NewGuid().ToString()),
                      new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                      new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                      new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);
            return Results.Ok(stringToken);

        }

        [HttpPost]
        [Authorize]
        public string GetData()
        {
            return "it's ok";
        }

        [HttpPost]
        [AllowAnonymous]
        public string ParameterTest(Test user)
        {
            return "it's ok";
        }

        ///Test user
        ///[FromForm] Test user
        ///[FromForm] string UserName, [FromForm] string Password

    }
}