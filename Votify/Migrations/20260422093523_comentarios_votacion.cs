using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Migrations
{
    /// <inheritdoc />
    public partial class comentarios_votacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "calificar_numericamente",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "comentario_obligatorio",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "comentario_opcional",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "fecha_fin_evaluacion_experta",
                table: "evento",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "participante_votan",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "puntos_a_repartir",
                table: "evento",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "repartir_puntos",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "sin_comentario",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "votar_proyecto",
                table: "evento",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "votar_x_proyecto",
                table: "evento",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "calificar_numericamente",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "comentario_obligatorio",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "comentario_opcional",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "fecha_fin_evaluacion_experta",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "participante_votan",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "puntos_a_repartir",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "repartir_puntos",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "sin_comentario",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "votar_proyecto",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "votar_x_proyecto",
                table: "evento");
        }
    }
}
