using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class manytomanyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productSizeModel_Products_ProductIds",
                table: "productSizeModel");

            migrationBuilder.DropIndex(
                name: "IX_productSizeModel_ProductIds",
                table: "productSizeModel");

            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "productSizeModel");

            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductproductSizeModel",
                columns: table => new
                {
                    ProductSizesId = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductproductSizeModel", x => new { x.ProductSizesId, x.productsId });
                    table.ForeignKey(
                        name: "FK_ProductproductSizeModel_Products_productsId",
                        column: x => x.productsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductproductSizeModel_productSizeModel_ProductSizesId",
                        column: x => x.ProductSizesId,
                        principalTable: "productSizeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductproductSizeModel_productsId",
                table: "ProductproductSizeModel",
                column: "productsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductproductSizeModel");

            migrationBuilder.AddColumn<int>(
                name: "ProductIds",
                table: "productSizeModel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductIds",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductIds",
                value: "[1,2,3]");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductIds",
                value: "[1,2,3]");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProductIds",
                value: "[1,2]");

            migrationBuilder.UpdateData(
                table: "productSizeModel",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductIds",
                value: null);

            migrationBuilder.UpdateData(
                table: "productSizeModel",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductIds",
                value: null);

            migrationBuilder.UpdateData(
                table: "productSizeModel",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProductIds",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_productSizeModel_ProductIds",
                table: "productSizeModel",
                column: "ProductIds");

            migrationBuilder.AddForeignKey(
                name: "FK_productSizeModel_Products_ProductIds",
                table: "productSizeModel",
                column: "ProductIds",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
