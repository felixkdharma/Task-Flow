using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskFlow.src.Persistence;

#nullable disable

namespace TaskFlow.src.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260821120000_AddProjectOwnership")]
public partial class AddProjectOwnership : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "CreatedById",
            table: "Projects",
            type: "uniqueidentifier",
            nullable: false);

        migrationBuilder.CreateIndex(
            name: "IX_Projects_CreatedById",
            table: "Projects",
            column: "CreatedById");

        migrationBuilder.AddForeignKey(
            name: "FK_Projects_AspNetUsers_CreatedById",
            table: "Projects",
            column: "CreatedById",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Projects_AspNetUsers_CreatedById",
            table: "Projects");

        migrationBuilder.DropIndex(
            name: "IX_Projects_CreatedById",
            table: "Projects");

        migrationBuilder.DropColumn(
            name: "CreatedById",
            table: "Projects");
    }
}
