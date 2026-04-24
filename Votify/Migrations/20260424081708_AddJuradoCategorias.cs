using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class AddJuradoCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "evento",
                type: "text",
                nullable: false,
                defaultValue: "Activo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado",
                table: "evento");
        }
    }
}
