using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProductSizeToDetailAndCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "productSizeId",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "productSizeId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_productSizeId",
                table: "ShoppingCarts",
                column: "productSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_productSizeId",
                table: "OrderDetails",
                column: "productSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductSizes_productSizeId",
                table: "OrderDetails",
                column: "productSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_ProductSizes_productSizeId",
                table: "ShoppingCarts",
                column: "productSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductSizes_productSizeId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_ProductSizes_productSizeId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_productSizeId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_productSizeId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "productSizeId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "productSizeId",
                table: "OrderDetails");
        }
    }
}
