using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Milos_Djukic_PR_21_2018.Migrations
{
    public partial class orderArticleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderArticles",
                table: "OrderArticles");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderArticles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderArticles",
                table: "OrderArticles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderArticles_OrderId",
                table: "OrderArticles",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderArticles",
                table: "OrderArticles");

            migrationBuilder.DropIndex(
                name: "IX_OrderArticles_OrderId",
                table: "OrderArticles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderArticles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderArticles",
                table: "OrderArticles",
                columns: new[] { "OrderId", "ArticleId" });
        }
    }
}
