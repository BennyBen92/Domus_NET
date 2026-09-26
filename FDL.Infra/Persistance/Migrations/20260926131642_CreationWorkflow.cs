using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FDL.Infra.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class CreationWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workflow",
                columns: table => new
                {
                    id_workflow = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    numero_dossier = table.Column<int>(type: "integer", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    id_user_assigne = table.Column<int>(type: "integer", nullable: true),
                    id_groupe_assigne = table.Column<int>(type: "integer", nullable: true),
                    id_user_expediteur = table.Column<int>(type: "integer", nullable: false),
                    date_expedition = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_user_terminaison = table.Column<int>(type: "integer", nullable: true),
                    date_terminaison = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_document = table.Column<int>(type: "integer", nullable: false),
                    id_user_update = table.Column<int>(type: "integer", nullable: true),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow", x => x.id_workflow);
                    table.CheckConstraint("ck_workflow_assignation", "id_user_assigne IS NOT NULL OR id_groupe_assigne IS NOT NULL");
                    table.CheckConstraint("ck_workflow_document", "type <> 1 OR id_document > 0");
                });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_id_groupe_assigne",
                table: "workflow",
                column: "id_groupe_assigne",
                filter: "id_user_terminaison IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_id_user_assigne",
                table: "workflow",
                column: "id_user_assigne",
                filter: "id_user_terminaison IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workflow");
        }
    }
}
