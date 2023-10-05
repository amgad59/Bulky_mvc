using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changeNamesInOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "OrderHeaders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
