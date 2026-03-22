using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class Email : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitacion_pendiente",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    correo_destino = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_grupo = table.Column<long>(type: "bigint", nullable: false),
                    id_evento = table.Column<long>(type: "bigint", nullable: false),
                    aceptada = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    correo_creador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("invitacion_pendiente_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_invitacion_pendiente_evento_id_evento",
                        column: x => x.id_evento,
                        principalTable: "evento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invitacion_pendiente_grupo_id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "grupo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_pendiente_id_evento",
                table: "invitacion_pendiente",
                column: "id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_pendiente_id_grupo",
                table: "invitacion_pendiente",
                column: "id_grupo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitacion_pendiente");
        }
    }
}
