using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class ProyectoDescripcionMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "proyecto",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "DescripcionBreve",
                table: "proyecto",
                newName: "descripcion_breve");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "proyecto",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "descripcion_breve",
                table: "proyecto",
                newName: "DescripcionBreve");
        }
    }
}
