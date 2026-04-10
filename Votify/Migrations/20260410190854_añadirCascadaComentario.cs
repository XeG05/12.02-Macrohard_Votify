using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class añadirCascadaComentario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "id_proyectoFK",
                table: "comentario");

            migrationBuilder.DropForeignKey(
                name: "id_rolFK",
                table: "comentario");

            migrationBuilder.AddForeignKey(
                name: "id_proyectoFK",
                table: "comentario",
                column: "id_proyecto_gru",
                principalTable: "proyecto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_rolFK",
                table: "comentario",
                columns: new[] { "correo_rol", "id_rol" },
                principalTable: "rol",
                principalColumns: new[] { "correo_usuario", "id_evento" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "id_proyectoFK",
                table: "comentario");

            migrationBuilder.DropForeignKey(
                name: "id_rolFK",
                table: "comentario");

            migrationBuilder.AddForeignKey(
                name: "id_proyectoFK",
                table: "comentario",
                column: "id_proyecto_gru",
                principalTable: "proyecto",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_rolFK",
                table: "comentario",
                columns: new[] { "correo_rol", "id_rol" },
                principalTable: "rol",
                principalColumns: new[] { "correo_usuario", "id_evento" });
        }
    }
}
