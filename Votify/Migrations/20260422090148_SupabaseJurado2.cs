using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class SupabaseJurado2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "fecha_fin_evaluacion_experta",
                table: "evento",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_fin_evaluacion_experta",
                table: "evento");
        }
    }
}
