using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixingMtoMErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductproductSizeModel");

            migrationBuilder.DropTable(
                name: "productSizeModel");

            migrationBuilder.CreateTable(
                name: "SizeModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductproductSize",
                columns: table => new
                {
                    ProductSizesId = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductproductSize", x => new { x.ProductSizesId, x.productsId });
                    table.ForeignKey(
                        name: "FK_ProductproductSize_Products_productsId",
                        column: x => x.productsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductproductSize_SizeModels_ProductSizesId",
                        column: x => x.ProductSizesId,
                        principalTable: "SizeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SizeModels",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "S", "Small" },
                    { 2, "M", "Medium" },
                    { 3, "L", "Large" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductproductSize_productsId",
                table: "ProductproductSize",
                column: "productsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductproductSize");

            migrationBuilder.DropTable(
                name: "SizeModels");

            migrationBuilder.CreateTable(
                name: "productSizeModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productSizeModel", x => x.Id);
                });

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

            migrationBuilder.InsertData(
                table: "productSizeModel",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "S", "Small" },
                    { 2, "M", "Medium" },
                    { 3, "L", "Large" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductproductSizeModel_productsId",
                table: "ProductproductSizeModel",
                column: "productsId");
        }
    }
}
