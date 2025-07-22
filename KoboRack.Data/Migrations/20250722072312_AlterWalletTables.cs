using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoboRack.Data.Migrations
{
    public partial class AlterWalletTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LockedAmount",
                table: "Wallets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Savings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedAmount",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Savings");
        }
    }
}
