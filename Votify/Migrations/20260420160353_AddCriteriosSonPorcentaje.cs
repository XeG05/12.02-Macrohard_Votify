using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class AddCriteriosSonPorcentaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "criterios_son_porcentaje",
                table: "categoria",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "criterios_son_porcentaje",
                table: "categoria");
        }
    }
}
