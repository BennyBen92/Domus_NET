using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FDL.Infra.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class CreationRegistre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registre",
                columns: table => new
                {
                    id_registre = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    reference_dossier = table.Column<int>(type: "integer", nullable: false),
                    id_user_creation = table.Column<int>(type: "integer", nullable: false),
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    statut = table.Column<int>(type: "integer", nullable: false),
                    date_statut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    nb_chambres_min = table.Column<int>(type: "integer", nullable: false),
                    nb_chambres_max = table.Column<int>(type: "integer", nullable: false),
                    liste_communes = table.Column<int[]>(type: "integer[]", nullable: false),
                    souhaite_ascenseur = table.Column<bool>(type: "boolean", nullable: false),
                    id_user_update = table.Column<int>(type: "integer", nullable: true),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registre", x => x.id_registre);
                });

            migrationBuilder.CreateIndex(
                name: "ix_registre_reference_dossier",
                table: "registre",
                column: "reference_dossier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registre");
        }
    }
}
