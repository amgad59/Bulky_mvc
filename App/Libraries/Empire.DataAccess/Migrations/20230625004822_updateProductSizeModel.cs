using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Empire.DataAccess.Migrations
{
    /// <inheritdoc />
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
    public partial class updateProductSizeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sizes",
                table: "Products",
                newName: "ProductIds");

            migrationBuilder.CreateTable(
                name: "productSizeModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductIds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productSizeModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productSizeModel_Products_ProductIds",
                        column: x => x.ProductIds,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

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

            migrationBuilder.InsertData(
                table: "productSizeModel",
                columns: new[] { "Id", "DisplayName", "Name", "ProductIds" },
                values: new object[,]
                {
                    { 1, "S", "Small", null },
                    { 2, "M", "Medium", null },
                    { 3, "L", "Large", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_productSizeModel_ProductIds",
                table: "productSizeModel",
                column: "ProductIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productSizeModel");

            migrationBuilder.RenameColumn(
                name: "ProductIds",
                table: "Products",
                newName: "Sizes");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Sizes",
                value: "[0,1,2]");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Sizes",
                value: "[0,1,2]");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Sizes",
                value: "[0,1,3]");
        }
    }
}
