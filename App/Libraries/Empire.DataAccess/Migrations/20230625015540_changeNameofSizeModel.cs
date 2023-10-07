using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
#pragma warning disable SA1300 // Element should begin with upper-case letter
    public partial class changeNameofSizeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductSize_SizeModels_ProductSizesId",
                table: "ProductProductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SizeModels",
                table: "SizeModels");

            migrationBuilder.RenameTable(
                name: "SizeModels",
                newName: "ProductSizes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductSize_ProductSizes_ProductSizesId",
                table: "ProductProductSize",
                column: "ProductSizesId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductSize_ProductSizes_ProductSizesId",
                table: "ProductProductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes");

            migrationBuilder.RenameTable(
                name: "ProductSizes",
                newName: "SizeModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SizeModels",
                table: "SizeModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductSize_SizeModels_ProductSizesId",
                table: "ProductProductSize",
                column: "ProductSizesId",
                principalTable: "SizeModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
