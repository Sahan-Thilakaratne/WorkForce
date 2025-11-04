using Api.Domain.Entities;
using Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<UserRole>(schema: "public", name: "user_role");

            modelBuilder.Entity<Department>(e =>
            {
                e.ToTable("department");
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).HasColumnName("name").IsRequired();
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("employee");
                e.Property(x => x.EmpCode).HasColumnName("empcode");
                e.Property(x => x.FirstName).HasColumnName("firstname").IsRequired();
                e.Property(x => x.LastName).HasColumnName("lastname").IsRequired();
                e.Property(x => x.Email).HasColumnName("email");
                e.Property(x => x.UserRole).HasColumnName("userrole").HasConversion<string>();
                e.Property(x => x.DateJoined).HasColumnName("datejoined");
                e.Property(x => x.IsActive).HasColumnName("isactive");


                e.Property<long?>("departmentid"); // shadow for FK casing
                e.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey("departmentid");
            });

            modelBuilder.Entity<AppUser>(e =>
            {
                e.ToTable("appuser");
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Username).HasColumnName("username").IsRequired();
                e.Property(x => x.PasswordHash).HasColumnName("passwordhash").IsRequired();
                e.Property(x => x.Role).HasColumnName("role").HasConversion<string>();
                e.Property(x => x.IsActive).HasColumnName("isactive");


                e.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .HasConstraintName("fk_appuser_employee");
            });


        }
    }
}
