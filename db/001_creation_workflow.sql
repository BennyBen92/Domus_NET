CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926131642_CreationWorkflow') THEN
    CREATE TABLE workflow (
        id_workflow integer GENERATED ALWAYS AS IDENTITY,
        numero_dossier integer NOT NULL,
        sequence integer NOT NULL,
        type integer NOT NULL,
        action integer NOT NULL,
        message text NOT NULL,
        id_user_assigne integer,
        id_groupe_assigne integer,
        id_user_expediteur integer NOT NULL,
        date_expedition timestamp with time zone NOT NULL,
        id_user_terminaison integer,
        date_terminaison timestamp with time zone,
        id_document integer NOT NULL,
        id_user_update integer,
        date_update timestamp with time zone,
        CONSTRAINT pk_workflow PRIMARY KEY (id_workflow),
        CONSTRAINT ck_workflow_assignation CHECK (id_user_assigne IS NOT NULL OR id_groupe_assigne IS NOT NULL),
        CONSTRAINT ck_workflow_document CHECK (type <> 1 OR id_document > 0)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926131642_CreationWorkflow') THEN
    CREATE INDEX ix_workflow_id_groupe_assigne ON workflow (id_groupe_assigne) WHERE id_user_terminaison IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926131642_CreationWorkflow') THEN
    CREATE INDEX ix_workflow_id_user_assigne ON workflow (id_user_assigne) WHERE id_user_terminaison IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260926131642_CreationWorkflow') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260926131642_CreationWorkflow', '10.0.12');
    END IF;
END $EF$;
COMMIT;

