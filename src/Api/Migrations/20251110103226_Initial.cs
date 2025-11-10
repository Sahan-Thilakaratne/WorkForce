using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.user_role", "employee,manager,head,admin");

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empcode = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    departmentid = table.Column<long>(type: "bigint", nullable: true),
                    userrole = table.Column<string>(type: "text", nullable: false),
                    datejoined = table.Column<DateOnly>(type: "date", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_department_departmentid",
                        column: x => x.departmentid,
                        principalTable: "department",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "appuser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appuser", x => x.Id);
                    table.ForeignKey(
                        name: "fk_appuser_employee",
                        column: x => x.EmployeeId,
                        principalTable: "employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appuser_EmployeeId",
                table: "appuser",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_appuser_username",
                table: "appuser",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_name",
                table: "department",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_departmentid",
                table: "employee",
                column: "departmentid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appuser");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "department");
        }
    }
}
