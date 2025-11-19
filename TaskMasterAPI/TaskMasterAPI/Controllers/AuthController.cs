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

    }
}
