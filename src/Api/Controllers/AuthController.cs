using Api.Data;
using Api.Domain.Entities;
using Api.Dtos.Auth;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtTokenService _jwt;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AuthController(AppDbContext db, JwtTokenService jwt)
        {
            _db = db; _jwt = jwt;
        }


        [HttpPost("register")]
        [Authorize(Policy ="IsAdmin")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(await _db.AppUsers.AnyAsync(u => u.Username == registerDto.Username)) {
                return Conflict(new
                {
                    message = "Username already exists"
                });
        }


    }
}
