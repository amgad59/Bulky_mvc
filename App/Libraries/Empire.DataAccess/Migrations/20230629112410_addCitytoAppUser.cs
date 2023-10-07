using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCitytoAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeaders",
                newName: "City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "OrderHeaders",
                newName: "State");

#pragma warning disable SA1122 // Use string.Empty for empty strings
            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
#pragma warning restore SA1122 // Use string.Empty for empty strings
        }
    }
}
