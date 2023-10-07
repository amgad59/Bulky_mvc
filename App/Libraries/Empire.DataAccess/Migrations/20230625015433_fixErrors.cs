using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
#pragma warning disable SA1300 // Element should begin with upper-case letter
    public partial class fixErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductproductSize_Products_productsId",
                table: "ProductproductSize");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductproductSize_SizeModels_ProductSizesId",
                table: "ProductproductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductproductSize",
                table: "ProductproductSize");

            migrationBuilder.RenameTable(
                name: "ProductproductSize",
                newName: "ProductProductSize");

            migrationBuilder.RenameIndex(
                name: "IX_ProductproductSize_productsId",
                table: "ProductProductSize",
                newName: "IX_ProductProductSize_productsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductProductSize",
                table: "ProductProductSize",
                columns: new[] { "ProductSizesId", "productsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductSize_Products_productsId",
                table: "ProductProductSize",
                column: "productsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductSize_SizeModels_ProductSizesId",
                table: "ProductProductSize",
                column: "ProductSizesId",
                principalTable: "SizeModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductSize_Products_productsId",
                table: "ProductProductSize");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductSize_SizeModels_ProductSizesId",
                table: "ProductProductSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductProductSize",
                table: "ProductProductSize");

            migrationBuilder.RenameTable(
                name: "ProductProductSize",
                newName: "ProductproductSize");

            migrationBuilder.RenameIndex(
                name: "IX_ProductProductSize_productsId",
                table: "ProductproductSize",
                newName: "IX_ProductproductSize_productsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductproductSize",
                table: "ProductproductSize",
                columns: new[] { "ProductSizesId", "productsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductproductSize_Products_productsId",
                table: "ProductproductSize",
                column: "productsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductproductSize_SizeModels_ProductSizesId",
                table: "ProductproductSize",
                column: "ProductSizesId",
                principalTable: "SizeModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
