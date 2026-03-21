using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class AñadirIdVoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "voto_pkey",
                table: "voto");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "voto",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "voto_pkey",
                table: "voto",
                columns: new[] { "id", "id_rol", "id_criterio", "id_proyecto_cat", "id_proyecto_gru" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "voto_pkey",
                table: "voto");

            migrationBuilder.DropColumn(
                name: "id",
                table: "voto");

            migrationBuilder.AddPrimaryKey(
                name: "voto_pkey",
                table: "voto",
                columns: new[] { "id_rol", "id_criterio", "id_proyecto_cat", "id_proyecto_gru" });
        }
    }
}
