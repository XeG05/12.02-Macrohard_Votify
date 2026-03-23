using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class MIGRACION4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "id_eventoFK",
                table: "categoria");

            migrationBuilder.DropForeignKey(
                name: "id_proyectoFK",
                table: "comentario");

            migrationBuilder.DropForeignKey(
                name: "id_categoriaFK",
                table: "criterio");

            migrationBuilder.DropForeignKey(
                name: "id_grupo",
                table: "participante");

            migrationBuilder.DropForeignKey(
                name: "id_categoria",
                table: "proyecto");

            migrationBuilder.DropForeignKey(
                name: "id_grupoFK",
                table: "proyecto");

            migrationBuilder.DropForeignKey(
                name: "id_criterio",
                table: "reclamacion");

            migrationBuilder.DropForeignKey(
                name: "correo_usuarioFK",
                table: "rol");

            migrationBuilder.DropForeignKey(
                name: "id_eventoFK",
                table: "rol");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "proyecto",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescripcionBreve",
                table: "proyecto",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "URLDescarga",
                table: "proyecto",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImagenProyecto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IdProyecto = table.Column<long>(type: "bigint", nullable: false),
                    ProyectoIdGrupo = table.Column<long>(type: "bigint", nullable: false),
                    ProyectoIdCategoria = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenProyecto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                        columns: x => new { x.ProyectoIdGrupo, x.ProyectoIdCategoria },
                        principalTable: "proyecto",
                        principalColumns: new[] { "id_grupo", "id_categoria" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenProyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenProyecto",
                columns: new[] { "ProyectoIdGrupo", "ProyectoIdCategoria" });

            migrationBuilder.AddForeignKey(
                name: "id_eventoFK",
                table: "categoria",
                column: "id_evento",
                principalTable: "evento",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_categoriaFK",
                table: "criterio",
                column: "id_categoria",
                principalTable: "categoria",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_grupo",
                table: "participante",
                column: "id_grupo",
                principalTable: "grupo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_categoria",
                table: "proyecto",
                column: "id_categoria",
                principalTable: "categoria",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_grupoFK",
                table: "proyecto",
                column: "id_grupo",
                principalTable: "grupo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "correo_usuarioFK",
                table: "rol",
                column: "correo_usuario",
                principalTable: "usuario",
                principalColumn: "correo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_eventoFK",
                table: "rol",
                column: "id_evento",
                principalTable: "evento",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_criterio",
                table: "voto",
                column: "id_criterio",
                principalTable: "criterio",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "id_proyectoFK",
                table: "voto",
                columns: new[] { "id_proyecto_gru", "id_proyecto_cat" },
                principalTable: "proyecto",
                principalColumns: new[] { "id_grupo", "id_categoria" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "id_eventoFK",
                table: "categoria");

            migrationBuilder.DropForeignKey(
                name: "id_categoriaFK",
                table: "criterio");

            migrationBuilder.DropForeignKey(
                name: "id_grupo",
                table: "participante");

            migrationBuilder.DropForeignKey(
                name: "id_categoria",
                table: "proyecto");

            migrationBuilder.DropForeignKey(
                name: "id_grupoFK",
                table: "proyecto");

            migrationBuilder.DropForeignKey(
                name: "correo_usuarioFK",
                table: "rol");

            migrationBuilder.DropForeignKey(
                name: "id_eventoFK",
                table: "rol");

            migrationBuilder.DropForeignKey(
                name: "id_criterio",
                table: "voto");

            migrationBuilder.DropForeignKey(
                name: "id_proyectoFK",
                table: "voto");

            migrationBuilder.DropTable(
                name: "ImagenProyecto");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "proyecto");

            migrationBuilder.DropColumn(
                name: "DescripcionBreve",
                table: "proyecto");

            migrationBuilder.DropColumn(
                name: "URLDescarga",
                table: "proyecto");

            migrationBuilder.AddForeignKey(
                name: "id_eventoFK",
                table: "categoria",
                column: "id_evento",
                principalTable: "evento",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_proyectoFK",
                table: "comentario",
                columns: new[] { "id_proyecto_gru", "id_proyecto_cat" },
                principalTable: "proyecto",
                principalColumns: new[] { "id_grupo", "id_categoria" });

            migrationBuilder.AddForeignKey(
                name: "id_categoriaFK",
                table: "criterio",
                column: "id_categoria",
                principalTable: "categoria",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_grupo",
                table: "participante",
                column: "id_grupo",
                principalTable: "grupo",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_categoria",
                table: "proyecto",
                column: "id_categoria",
                principalTable: "categoria",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_grupoFK",
                table: "proyecto",
                column: "id_grupo",
                principalTable: "grupo",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "id_criterio",
                table: "reclamacion",
                column: "id_criterio",
                principalTable: "criterio",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "correo_usuarioFK",
                table: "rol",
                column: "correo_usuario",
                principalTable: "usuario",
                principalColumn: "correo");

            migrationBuilder.AddForeignKey(
                name: "id_eventoFK",
                table: "rol",
                column: "id_evento",
                principalTable: "evento",
                principalColumn: "id");
        }
    }
}
