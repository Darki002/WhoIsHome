using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhoIsHome.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDateOnEventInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteDate",
                table: "event",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "event");
        }
    }
}
