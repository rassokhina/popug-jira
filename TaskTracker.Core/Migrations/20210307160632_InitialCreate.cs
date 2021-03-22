using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTracker.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Popugs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Popugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Finished = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PopugId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Popugs_PopugId",
                        column: x => x.PopugId,
                        principalTable: "Popugs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PopugId",
                table: "Tasks",
                column: "PopugId");

            // add initial popug users

            migrationBuilder.Sql
           (
               @"
                    INSERT INTO [dbo].[Popugs] ([Id], [Created], [Name], [Role]) VALUES ('4D4F5682-9AC0-E811-A2D6-00155DB24300', GETDATE(), N'Admin', 0);
                    INSERT INTO [dbo].[Popugs] ([Id], [Created], [Name], [Role]) VALUES ('4E4F5682-9AC0-E811-A2D6-00155DB24300', GETDATE(), N'Employee', 2);
               "
           );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Popugs");
        }
    }
}
