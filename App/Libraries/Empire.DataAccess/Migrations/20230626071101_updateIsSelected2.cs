using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
    public partial class updateIsSelected2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isSelected",
                table: "ProductSizes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isSelected",
                table: "ProductSizes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ProductSizes",
                keyColumn: "Id",
                keyValue: 1,
                column: "isSelected",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProductSizes",
                keyColumn: "Id",
                keyValue: 2,
                column: "isSelected",
                value: false);

            migrationBuilder.UpdateData(
                table: "ProductSizes",
                keyColumn: "Id",
                keyValue: 3,
                column: "isSelected",
                value: false);
        }
    }
}
