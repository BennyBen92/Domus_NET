using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FDL.Infra.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowReferenceDossier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Nouvelle colonne, provisoirement nullable
            migrationBuilder.AddColumn<int>(name: "reference_dossier", table: "workflow",
                type: "integer", nullable: true);

            // 2. Recopier les données existantes
            migrationBuilder.Sql("UPDATE workflow SET reference_dossier = numero_dossier * 100 + sequence;");

            // 3. Rendre la colonne obligatoire
            migrationBuilder.AlterColumn<int>(name: "reference_dossier", table: "workflow",
                type: "integer", nullable: false, oldClrType: typeof(int), oldType: "integer", oldNullable: true);

            // 4. Supprimer les anciennes colonnes
            migrationBuilder.DropColumn(name: "numero_dossier", table: "workflow");
            migrationBuilder.DropColumn(name: "sequence", table: "workflow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(name: "numero_dossier", table: "workflow", type: "integer", nullable: true);
            migrationBuilder.AddColumn<int>(name: "sequence", table: "workflow", type: "integer", nullable: true);

            migrationBuilder.Sql("UPDATE workflow SET numero_dossier = reference_dossier / 100, sequence = reference_dossier % 100;");

            migrationBuilder.AlterColumn<int>(name: "numero_dossier", table: "workflow", type: "integer", nullable: false,
                oldClrType: typeof(int), oldType: "integer", oldNullable: true);
            migrationBuilder.AlterColumn<int>(name: "sequence", table: "workflow", type: "integer", nullable: false,
                oldClrType: typeof(int), oldType: "integer", oldNullable: true);

            migrationBuilder.DropColumn(name: "reference_dossier", table: "workflow");
        }
    }
}
