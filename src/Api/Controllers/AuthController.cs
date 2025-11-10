using Api.Data;
using Api.Domain.Entities;
using Api.Domain.Enums;
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
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.AppUsers.AnyAsync(u => u.Username == dto.Username))
                return Conflict(new { message = "Username already exists" });


            // Create employee (minimal)
            var emp = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserRole = Enum.TryParse<UserRole>(dto.Role, true, out var r) ? r : UserRole.EMPLOYEE,
                DateJoined = DateOnly.FromDateTime(DateTime.UtcNow),
                IsActive = true
            };
            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();


            var user = new AppUser
            {
                EmployeeId = emp.Id,
                Username = dto.Username,
                Role = emp.UserRole,
                IsActive = true
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();


            return Created($"api/auth/users/{user.Id}", new { user.Id, user.Username, Role = user.Role.ToString() });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _db.AppUsers.Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid credentials" });


            var token = _jwt.Create(user, user.Role.ToString());
            return Ok(new
            {
                token,
                user = new { id = user.Id, username = user.Username, role = user.Role.ToString() }
            });
        }




        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();


            var user = await _db.AppUsers.Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();


            return Ok(new { id = user.Id, username = user.Username, role = user.Role.ToString() });
        }


        [HttpGet("admin-only")] // sample protected endpoint
        [Authorize(Policy = "IsAdmin")]
        public IActionResult AdminOnly() => Ok(new { message = "Hello, Admin!" });




    }

}
