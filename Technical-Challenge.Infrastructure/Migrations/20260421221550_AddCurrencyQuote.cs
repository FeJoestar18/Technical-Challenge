using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Technical_Challenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bid = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    ask = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    high = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    low = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    captured_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_quotes_captured_at",
                table: "quotes",
                column: "captured_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "idx_quotes_currency",
                table: "quotes",
                column: "currency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotes");
        }
    }
}
