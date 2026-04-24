using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriaJurado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriumJurado");

            migrationBuilder.CreateTable(
                name: "categoria_jurado",
                columns: table => new
                {
                    id_categoria = table.Column<long>(type: "bigint", nullable: false),
                    correo_jurado = table.Column<string>(type: "character varying(50)", nullable: false),
                    id_evento_jurado = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria_jurado", x => new { x.id_categoria, x.correo_jurado, x.id_evento_jurado });
                    table.ForeignKey(
                        name: "FK_categoria_jurado_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_categoria_jurado_jurado_correo_jurado_id_evento_jurado",
                        columns: x => new { x.correo_jurado, x.id_evento_jurado },
                        principalTable: "jurado",
                        principalColumns: new[] { "correo_usuario", "id_evento" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categoria_jurado_correo_jurado_id_evento_jurado",
                table: "categoria_jurado",
                columns: new[] { "correo_jurado", "id_evento_jurado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categoria_jurado");

            migrationBuilder.CreateTable(
                name: "CategoriumJurado",
                columns: table => new
                {
                    CategoriasId = table.Column<long>(type: "bigint", nullable: false),
                    JuradosCorreoUsuario = table.Column<string>(type: "character varying(50)", nullable: false),
                    JuradosIdEvento = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriumJurado", x => new { x.CategoriasId, x.JuradosCorreoUsuario, x.JuradosIdEvento });
                    table.ForeignKey(
                        name: "FK_CategoriumJurado_categoria_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriumJurado_jurado_JuradosCorreoUsuario_JuradosIdEvento",
                        columns: x => new { x.JuradosCorreoUsuario, x.JuradosIdEvento },
                        principalTable: "jurado",
                        principalColumns: new[] { "correo_usuario", "id_evento" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriumJurado_JuradosCorreoUsuario_JuradosIdEvento",
                table: "CategoriumJurado",
                columns: new[] { "JuradosCorreoUsuario", "JuradosIdEvento" });
        }
    }
}
