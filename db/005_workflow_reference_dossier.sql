START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    ALTER TABLE workflow ADD reference_dossier integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    UPDATE workflow SET reference_dossier = numero_dossier * 100 + sequence;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    ALTER TABLE workflow ALTER COLUMN reference_dossier SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    ALTER TABLE workflow DROP COLUMN numero_dossier;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    ALTER TABLE workflow DROP COLUMN sequence;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927181848_WorkflowReferenceDossier') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260927181848_WorkflowReferenceDossier', '10.0.12');
    END IF;
END $EF$;
COMMIT;

