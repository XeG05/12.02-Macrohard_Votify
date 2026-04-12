using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:Votify/Migrations/20260412150136_migracion15.cs
    public partial class migracion15 : Migration
========
    public partial class cambiosIdProyecto2 : Migration
>>>>>>>> Cambiode BD id:Votify/Migrations/20260412152600_cambiosIdProyecto2.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evento",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    fecha_fin_inscripciones = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_entrega = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_votaciones = table.Column<DateOnly>(type: "date", nullable: false),
                    tiene_grupos = table.Column<bool>(type: "boolean", nullable: false),
                    grupos_completos = table.Column<bool>(type: "boolean", nullable: false),
                    participantes_por_grupo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("evento_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contraseña = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuarios_pkey", x => x.correo);
                });

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    id_evento = table.Column<long>(type: "bigint", nullable: false),
                    ponderacion_popular = table.Column<long>(type: "bigint", nullable: false),
                    ponderacion_experto = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoria_pkey", x => x.id);
                    table.ForeignKey(
                        name: "id_eventoFK",
                        column: x => x.id_evento,
                        principalTable: "evento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notificacion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contenido = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notificacion_pkey", x => new { x.id, x.correo });
                    table.ForeignKey(
                        name: "correoFK",
                        column: x => x.correo,
                        principalTable: "usuario",
                        principalColumn: "correo");
                });

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    correo_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_evento = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rol_pkey", x => new { x.correo_usuario, x.id_evento });
                    table.ForeignKey(
                        name: "correo_usuarioFK",
                        column: x => x.correo_usuario,
                        principalTable: "usuario",
                        principalColumn: "correo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_eventoFK",
                        column: x => x.id_evento,
                        principalTable: "evento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "criterio",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    id_categoria = table.Column<long>(type: "bigint", nullable: false),
                    ponderacion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("criterio_pkey", x => x.id);
                    table.ForeignKey(
                        name: "id_categoriaFK",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grupo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    hoja_ruta_texto = table.Column<string>(type: "text", nullable: true),
                    id_categoria = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("grupo_pkey", x => x.id);
                    table.ForeignKey(
                        name: "id_categoriaFK",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "administrador",
                columns: table => new
                {
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrador_pkey", x => new { x.correo, x.id });
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo, x.id },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateTable(
                name: "jurado",
                columns: table => new
                {
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("jurado_pkey", x => new { x.correo, x.id });
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo, x.id },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateTable(
                name: "votante",
                columns: table => new
                {
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("votante_pkey", x => new { x.correo, x.id });
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo, x.id },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateTable(
                name: "reclamacion",
                columns: table => new
                {
                    id_criterio = table.Column<long>(type: "bigint", nullable: false),
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    correo_rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reclamacion_pkey", x => new { x.id_criterio, x.id_rol, x.correo_rol });
                    table.ForeignKey(
                        name: "id_criterio",
                        column: x => x.id_criterio,
                        principalTable: "criterio",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "id_rol",
                        columns: x => new { x.correo_rol, x.id_rol },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateTable(
                name: "invitacion_pendiente",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "participante",
                columns: table => new
                {
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false),
                    id_grupo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("participante_pkey", x => new { x.correo, x.id });
                    table.ForeignKey(
                        name: "id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "grupo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo, x.id },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateTable(
                name: "proyecto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_grupo = table.Column<long>(type: "bigint", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    DescripcionBreve = table.Column<string>(type: "text", nullable: false),
                    archivo = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("proyecto_pkey", x => x.id);
                    table.UniqueConstraint("AK_proyecto_id_grupo", x => x.id_grupo);
                    table.ForeignKey(
                        name: "id_grupoFK",
                        column: x => x.id_grupo,
                        principalTable: "grupo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comentario",
                columns: table => new
                {
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    correo_rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_proyecto_gru = table.Column<long>(type: "bigint", nullable: false),
                    contenido = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comentario_pkey", x => new { x.id_rol, x.correo_rol, x.id_proyecto_gru });
                    table.ForeignKey(
                        name: "id_proyectoFK",
                        column: x => x.id_proyecto_gru,
                        principalTable: "proyecto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo_rol, x.id_rol },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imagen_proyecto",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    archivo = table.Column<byte[]>(type: "bytea", nullable: false),
                    proyecto_id_grupo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagen_proyecto", x => x.id);
                    table.ForeignKey(
                        name: "FK_imagen_proyecto_proyecto_proyecto_id_grupo",
                        column: x => x.proyecto_id_grupo,
                        principalTable: "proyecto",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    id_criterio = table.Column<long>(type: "bigint", nullable: false),
                    id_proyecto_gru = table.Column<long>(type: "bigint", nullable: false),
                    nota = table.Column<long>(type: "bigint", nullable: true),
                    correo_rol = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("voto_pkey", x => new { x.id, x.id_rol, x.id_criterio, x.id_proyecto_gru });
                    table.ForeignKey(
                        name: "id_criterio",
                        column: x => x.id_criterio,
                        principalTable: "criterio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_proyectoFK",
                        column: x => x.id_proyecto_gru,
                        principalTable: "proyecto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_rolFK",
                        columns: x => new { x.correo_rol, x.id_rol },
                        principalTable: "rol",
                        principalColumns: new[] { "correo_usuario", "id_evento" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_categoria_id_evento",
                table: "categoria",
                column: "id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_comentario_correo_rol_id_rol",
                table: "comentario",
                columns: new[] { "correo_rol", "id_rol" });

            migrationBuilder.CreateIndex(
                name: "IX_comentario_id_proyecto_gru",
                table: "comentario",
                column: "id_proyecto_gru");

            migrationBuilder.CreateIndex(
                name: "IX_criterio_id_categoria",
                table: "criterio",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_grupo_id_categoria",
                table: "grupo",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_imagen_proyecto_proyecto_id_grupo",
                table: "imagen_proyecto",
                column: "proyecto_id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_pendiente_id_evento",
                table: "invitacion_pendiente",
                column: "id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_invitacion_pendiente_id_grupo",
                table: "invitacion_pendiente",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_notificacion_correo",
                table: "notificacion",
                column: "correo");

            migrationBuilder.CreateIndex(
                name: "IX_participante_id_grupo",
                table: "participante",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_reclamacion_correo_rol_id_rol",
                table: "reclamacion",
                columns: new[] { "correo_rol", "id_rol" });

            migrationBuilder.CreateIndex(
                name: "IX_rol_id_evento",
                table: "rol",
                column: "id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_voto_correo_rol_id_rol",
                table: "voto",
                columns: new[] { "correo_rol", "id_rol" });

            migrationBuilder.CreateIndex(
                name: "IX_voto_id_criterio",
                table: "voto",
                column: "id_criterio");

            migrationBuilder.CreateIndex(
                name: "IX_voto_id_proyecto_gru",
                table: "voto",
                column: "id_proyecto_gru");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrador");

            migrationBuilder.DropTable(
                name: "comentario");

            migrationBuilder.DropTable(
                name: "imagen_proyecto");

            migrationBuilder.DropTable(
                name: "invitacion_pendiente");

            migrationBuilder.DropTable(
                name: "jurado");

            migrationBuilder.DropTable(
                name: "notificacion");

            migrationBuilder.DropTable(
                name: "participante");

            migrationBuilder.DropTable(
                name: "reclamacion");

            migrationBuilder.DropTable(
                name: "votante");

            migrationBuilder.DropTable(
                name: "voto");

            migrationBuilder.DropTable(
                name: "criterio");

            migrationBuilder.DropTable(
                name: "proyecto");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "grupo");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "evento");
        }
    }
}
