using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhoIsHome.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserModelId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_RepeatedEvent_User_UserModelId",
                table: "RepeatedEvent");

            migrationBuilder.RenameColumn(
                name: "UserModelId",
                table: "RepeatedEvent",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RepeatedEvent_UserModelId",
                table: "RepeatedEvent",
                newName: "IX_RepeatedEvent_UserId");

            migrationBuilder.RenameColumn(
                name: "UserModelId",
                table: "Event",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_UserModelId",
                table: "Event",
                newName: "IX_Event_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepeatedEvent_User_UserId",
                table: "RepeatedEvent",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_RepeatedEvent_User_UserId",
                table: "RepeatedEvent");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RepeatedEvent",
                newName: "UserModelId");

            migrationBuilder.RenameIndex(
                name: "IX_RepeatedEvent_UserId",
                table: "RepeatedEvent",
                newName: "IX_RepeatedEvent_UserModelId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Event",
                newName: "UserModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_UserId",
                table: "Event",
                newName: "IX_Event_UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserModelId",
                table: "Event",
                column: "UserModelId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepeatedEvent_User_UserModelId",
                table: "RepeatedEvent",
                column: "UserModelId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
