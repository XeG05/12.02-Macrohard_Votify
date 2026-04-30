using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class JuradoEventoCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitacion_jurado",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correo_destino = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_categoria = table.Column<long>(type: "bigint", nullable: false),
                    id_evento = table.Column<long>(type: "bigint", nullable: false),
                    aceptada = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    correo_creador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("invitacion_jurado_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_invitacion_jurado_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invitacion_jurado_evento_id_evento",
                        column: x => x.id_evento,
                        principalTable: "evento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_jurado_id_categoria",
                table: "invitacion_jurado",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_jurado_id_evento",
                table: "invitacion_jurado",
                column: "id_evento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitacion_jurado");
        }
    }
}
