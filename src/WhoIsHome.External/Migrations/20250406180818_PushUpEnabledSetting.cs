using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhoIsHome.External.Migrations
{
    /// <inheritdoc />
    public partial class PushUpEnabledSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "PushUpSettings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "PushUpSettings");
        }
    }
}
