using Api.Domain.Entities;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Ensure user_role enum exists (created by migration). Create some seed data.
            if (!await db.Departments.AnyAsync())
            {
                db.Departments.AddRange(new Department { Name = "IT" }, new Department { Name = "HR" });
                await db.SaveChangesAsync();
            }


            if (!await db.Employees.AnyAsync())
            {
                db.Employees.Add(new Employee
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@example.com",
                    UserRole = UserRole.ADMIN,
                    DateJoined = DateOnly.FromDateTime(DateTime.UtcNow),
                    IsActive = true
                });
                await db.SaveChangesAsync();
            }


            if (!await db.AppUsers.AnyAsync())
            {
                var adminEmp = await db.Employees.OrderBy(e => e.Id).FirstAsync();
                var hasher = new PasswordHasher<AppUser>();
                var admin = new AppUser
                {
                    EmployeeId = adminEmp.Id,
                    Username = "admin",
                    Role = UserRole.ADMIN,
                    IsActive = true
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");
                db.AppUsers.Add(admin);
                await db.SaveChangesAsync();
            }
        }
    }
}
