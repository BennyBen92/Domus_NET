using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FDL.Infra.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AuditTechniqueDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_maj_db",
                table: "workflow",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "role_maj_db",
                table: "workflow",
                type: "text",
                nullable: false,
                defaultValueSql: "current_user");

            migrationBuilder.Sql(@"
                CREATE FUNCTION audit_technique() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    NEW.date_maj_db := now();
                    NEW.role_maj_db := current_user;
                    RETURN NEW;
                END $$;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_workflow_audit_technique
                BEFORE INSERT OR UPDATE ON workflow
                FOR EACH ROW EXECUTE FUNCTION audit_technique();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER trg_workflow_audit_technique ON workflow;
                DROP FUNCTION audit_technique();
                ALTER TABLE workflow DROP COLUMN date_maj_db, DROP COLUMN role_maj_db;
            ");
        }
    }
}
