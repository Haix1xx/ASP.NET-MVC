using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    public partial class FixProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_Product_PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategoryProducts_PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_Product_ProductId",
                table: "ProductCategoryProducts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_Product_ProductId",
                table: "ProductCategoryProducts");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "ProductCategoryProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_PostId",
                table: "ProductCategoryProducts",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_Product_PostId",
                table: "ProductCategoryProducts",
                column: "PostId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
