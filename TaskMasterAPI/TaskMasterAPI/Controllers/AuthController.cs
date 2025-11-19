using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskMasterAPI.Data;
using TaskMasterAPI.DTOs;
using TaskMasterAPI.Models;
namespace TaskMasterAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDtos dto)
        {
            bool userNameExist = await _context.Users.AnyAsync(u => u.UserName == dto.UserName || u.Email == dto.Email);

            if (userNameExist)
            {
                return BadRequest("User already exists");
            }

            var user = new UserModel
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)

            };

                 _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User Registered Successfully!");

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDtos dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if(user == null)
            {
                return Unauthorized("Invalid Credentials!");
            }

            if(!BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash))
            {
                return Unauthorized("Invalid Credentials!");
            }

            string token = GenerateJwt(user);

            return Ok(new {token});
        }

        private string GenerateJwt(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
             
    }
}
