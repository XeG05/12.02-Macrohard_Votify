using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class MigracionXD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "participante_pkey",
                table: "participante");

            migrationBuilder.DropIndex(
                name: "IX_participante_correo_id",
                table: "participante");

            migrationBuilder.AddPrimaryKey(
                name: "participante_pkey",
                table: "participante",
                columns: new[] { "correo", "id", "id_grupo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "participante_pkey",
                table: "participante");

            migrationBuilder.AddPrimaryKey(
                name: "participante_pkey",
                table: "participante",
                columns: new[] { "id", "correo", "id_grupo" });

            migrationBuilder.CreateIndex(
                name: "IX_participante_correo_id",
                table: "participante",
                columns: new[] { "correo", "id" });
        }
    }
}
