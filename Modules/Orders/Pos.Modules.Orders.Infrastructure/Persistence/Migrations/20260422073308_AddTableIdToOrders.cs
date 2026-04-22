using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Modules.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableIdToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TableId",
                schema: "orders",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                schema: "orders",
                table: "Orders",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableId",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TableNumber",
                schema: "orders",
                table: "Orders");
        }
    }
}
