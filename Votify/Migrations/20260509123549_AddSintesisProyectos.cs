using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class AddSintesisProyectos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sintesis_votantes",
                table: "proyecto",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sintesis_jurado_criterio",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proyecto = table.Column<long>(type: "bigint", nullable: false),
                    id_criterio = table.Column<long>(type: "bigint", nullable: false),
                    sintesis = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sintesis_jurado_criterio_pkey", x => x.id);
                    table.ForeignKey(
                        name: "id_proyecto_sintesis_juradoFK",
                        column: x => x.id_proyecto,
                        principalTable: "proyecto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sintesis_jurado_criterio_id_proyecto",
                table: "sintesis_jurado_criterio",
                column: "id_proyecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sintesis_jurado_criterio");

            migrationBuilder.DropColumn(
                name: "sintesis_votantes",
                table: "proyecto");
        }
    }
}
