using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoboRack.Data.Migrations
{
    public partial class AddAdditionColumnToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Withrawal",
                table: "FundingAnalytics",
                newName: "Withdrawal");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GroupsTransaction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalContributed",
                table: "GroupSavingsMembers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppUserTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WalletId",
                table: "AppUserTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_AppUserId",
                table: "Wallets",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserTransactions_WalletId",
                table: "AppUserTransactions",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserTransactions_Wallets_WalletId",
                table: "AppUserTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_AppUserId",
                table: "Wallets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserTransactions_Wallets_WalletId",
                table: "AppUserTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_AppUserId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_AppUserId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_AppUserTransactions_WalletId",
                table: "AppUserTransactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GroupsTransaction");

            migrationBuilder.DropColumn(
                name: "TotalContributed",
                table: "GroupSavingsMembers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppUserTransactions");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "AppUserTransactions");

            migrationBuilder.RenameColumn(
                name: "Withdrawal",
                table: "FundingAnalytics",
                newName: "Withrawal");
        }
    }
}
