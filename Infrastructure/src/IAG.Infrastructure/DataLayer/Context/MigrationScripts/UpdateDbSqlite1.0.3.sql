BEGIN TRANSACTION;

UPDATE "SchemaVersion" SET Module = 'Infrastructure.CoreServer' WHERE Module = 'CoreServer.CoreServer';

COMMIT;
