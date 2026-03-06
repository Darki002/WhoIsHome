using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhoIsHome.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableToCorrectNameLol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_vent_group_EventGroupId",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_vent_group_user_UserId",
                table: "vent_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vent_group",
                table: "vent_group");

            migrationBuilder.RenameTable(
                name: "vent_group",
                newName: "event_group");

            migrationBuilder.RenameIndex(
                name: "IX_vent_group_UserId",
                table: "event_group",
                newName: "IX_event_group_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_event_group",
                table: "event_group",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_event_group_EventGroupId",
                table: "event",
                column: "EventGroupId",
                principalTable: "event_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_group_user_UserId",
                table: "event_group",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_event_group_EventGroupId",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_group_user_UserId",
                table: "event_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_event_group",
                table: "event_group");

            migrationBuilder.RenameTable(
                name: "event_group",
                newName: "vent_group");

            migrationBuilder.RenameIndex(
                name: "IX_event_group_UserId",
                table: "vent_group",
                newName: "IX_vent_group_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vent_group",
                table: "vent_group",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_vent_group_EventGroupId",
                table: "event",
                column: "EventGroupId",
                principalTable: "vent_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vent_group_user_UserId",
                table: "vent_group",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
