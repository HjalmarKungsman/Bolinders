using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bolinders.Web.Data.Migrations
{
    public partial class ModelsUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Make");

            migrationBuilder.AddColumn<string>(
                name: "LogotypeFileName",
                table: "Make",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogotypeFileName",
                table: "Make");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Make",
                nullable: true);
        }
    }
}
