using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FDL.Infra.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RegistreReferenceUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_registre_reference_dossier",
                table: "registre");

            migrationBuilder.CreateIndex(
                name: "ux_registre_reference_dossier",
                table: "registre",
                column: "reference_dossier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_registre_reference_dossier",
                table: "registre");

            migrationBuilder.CreateIndex(
                name: "ix_registre_reference_dossier",
                table: "registre",
                column: "reference_dossier");
        }
    }
}
