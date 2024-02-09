using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XXL_Market.Migrations
{
    public partial class migrimssdzdjjjeijjjjjsxssssaaaj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Orders",
                newName: "OrderiId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderDetails",
                newName: "OrderiId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderiId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderiId",
                table: "OrderDetails",
                column: "OrderiId",
                principalTable: "Orders",
                principalColumn: "OrderiId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderiId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderiId",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "OrderiId",
                table: "OrderDetails",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderiId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
