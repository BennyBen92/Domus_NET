START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926133834_AuditTechniqueDb') THEN
    ALTER TABLE workflow ADD date_maj_db timestamptz NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926133834_AuditTechniqueDb') THEN
    ALTER TABLE workflow ADD role_maj_db text NOT NULL DEFAULT (current_user);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926133834_AuditTechniqueDb') THEN

                    CREATE FUNCTION audit_technique() RETURNS trigger LANGUAGE plpgsql AS $$
                    BEGIN
                        NEW.date_maj_db := now();
                        NEW.role_maj_db := current_user;
                        RETURN NEW;
                    END $$;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926133834_AuditTechniqueDb') THEN

                    CREATE TRIGGER trg_workflow_audit_technique
                    BEFORE INSERT OR UPDATE ON workflow
                    FOR EACH ROW EXECUTE FUNCTION audit_technique();
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926133834_AuditTechniqueDb') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260926133834_AuditTechniqueDb', '10.0.12');
    END IF;
END $EF$;
COMMIT;

