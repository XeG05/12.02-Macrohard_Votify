using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class MiiMigracion44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagenProyecto_proyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "ImagenProyecto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImagenProyecto",
                table: "ImagenProyecto");

            migrationBuilder.DropColumn(
                name: "URLDescarga",
                table: "proyecto");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ImagenProyecto");

            migrationBuilder.RenameTable(
                name: "ImagenProyecto",
                newName: "imagen_proyecto");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "imagen_proyecto",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProyectoIdGrupo",
                table: "imagen_proyecto",
                newName: "proyecto_id_grupo");

            migrationBuilder.RenameColumn(
                name: "ProyectoIdCategoria",
                table: "imagen_proyecto",
                newName: "proyecto_id_categoria");

            migrationBuilder.RenameIndex(
                name: "IX_ImagenProyecto_ProyectoIdGrupo_ProyectoIdCategoria",
                table: "imagen_proyecto",
                newName: "IX_imagen_proyecto_proyecto_id_grupo_proyecto_id_categoria");

            migrationBuilder.AddColumn<byte[]>(
                name: "archivo",
                table: "proyecto",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "imagen_proyecto",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<byte[]>(
                name: "archivo",
                table: "imagen_proyecto",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_imagen_proyecto",
                table: "imagen_proyecto",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_imagen_proyecto_proyecto_proyecto_id_grupo_proyecto_id_cate~",
                table: "imagen_proyecto",
                columns: new[] { "proyecto_id_grupo", "proyecto_id_categoria" },
                principalTable: "proyecto",
                principalColumns: new[] { "id_grupo", "id_categoria" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imagen_proyecto_proyecto_proyecto_id_grupo_proyecto_id_cate~",
                table: "imagen_proyecto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_imagen_proyecto",
                table: "imagen_proyecto");

            migrationBuilder.DropColumn(
                name: "archivo",
                table: "proyecto");

            migrationBuilder.DropColumn(
                name: "archivo",
                table: "imagen_proyecto");

            migrationBuilder.RenameTable(
                name: "imagen_proyecto",
                newName: "ImagenProyecto");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ImagenProyecto",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "proyecto_id_grupo",
                table: "ImagenProyecto",
                newName: "ProyectoIdGrupo");

            migrationBuilder.RenameColumn(
                name: "proyecto_id_categoria",
                table: "ImagenProyecto",
                newName: "ProyectoIdCategoria");

            migrationBuilder.RenameIndex(
                name: "IX_imagen_proyecto_proyecto_id_grupo_proyecto_id_categoria",
                table: "ImagenProyecto",
                newName: "IX_ImagenProyecto_ProyectoIdGrupo_ProyectoIdCategoria");

            migrationBuilder.AddColumn<string>(
                name: "URLDescarga",
                table: "proyecto",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ImagenProyecto",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ImagenProyecto",
                type: "text",
                nullable: false,
                defaultValue: "");

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
    }
}
