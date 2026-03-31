using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class arreglarImagenProyecto2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagenesProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategor~",
                table: "ImagenesProyecto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImagenesProyecto",
                table: "ImagenesProyecto");

            migrationBuilder.RenameTable(
                name: "ImagenesProyecto",
                newName: "ImagenProyecto");

            migrationBuilder.RenameIndex(
                name: "IX_ImagenesProyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenProyecto",
                newName: "IX_ImagenProyecto_ProyectoIdGrupo_ProyectoIdCategoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImagenProyecto",
                table: "ImagenProyecto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagenProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenProyecto",
                columns: new[] { "ProyectoIdGrupo", "ProyectoIdCategoria" },
                principalTable: "proyecto",
                principalColumns: new[] { "id_grupo", "id_categoria" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagenProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenProyecto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImagenProyecto",
                table: "ImagenProyecto");

            migrationBuilder.RenameTable(
                name: "ImagenProyecto",
                newName: "ImagenesProyecto");

            migrationBuilder.RenameIndex(
                name: "IX_ImagenProyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenesProyecto",
                newName: "IX_ImagenesProyecto_ProyectoIdGrupo_ProyectoIdCategoria");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImagenesProyecto",
                table: "ImagenesProyecto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagenesProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategor~",
                table: "ImagenesProyecto",
                columns: new[] { "ProyectoIdGrupo", "ProyectoIdCategoria" },
                principalTable: "proyecto",
                principalColumns: new[] { "id_grupo", "id_categoria" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
