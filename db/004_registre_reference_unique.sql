START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927173843_RegistreReferenceUnique') THEN
    DROP INDEX ix_registre_reference_dossier;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927173843_RegistreReferenceUnique') THEN
    CREATE UNIQUE INDEX ux_registre_reference_dossier ON registre (reference_dossier);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927173843_RegistreReferenceUnique') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260927173843_RegistreReferenceUnique', '10.0.12');
    END IF;
END $EF$;
COMMIT;

