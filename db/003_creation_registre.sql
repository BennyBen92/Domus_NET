START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927160753_CreationRegistre') THEN
    CREATE TABLE registre (
        id_registre integer GENERATED ALWAYS AS IDENTITY,
        reference_dossier integer NOT NULL,
        id_user_creation integer NOT NULL,
        date_creation timestamp with time zone NOT NULL,
        type integer NOT NULL,
        statut integer NOT NULL,
        date_statut timestamp with time zone NOT NULL,
        commentaire text,
        nb_chambres_min integer NOT NULL,
        nb_chambres_max integer NOT NULL,
        liste_communes integer[] NOT NULL,
        souhaite_ascenseur boolean NOT NULL,
        id_user_update integer,
        date_update timestamp with time zone,
        CONSTRAINT pk_registre PRIMARY KEY (id_registre)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927160753_CreationRegistre') THEN
    CREATE INDEX ix_registre_reference_dossier ON registre (reference_dossier);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260927160753_CreationRegistre') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260927160753_CreationRegistre', '10.0.12');
    END IF;
END $EF$;
COMMIT;

