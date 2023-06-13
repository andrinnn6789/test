BEGIN TRANSACTION;

UPDATE "FailedRequest"
SET Id = hex(substr(Id, 4, 1)) ||
                 hex(substr(Id, 3, 1)) ||
                 hex(substr(Id, 2, 1)) ||
                 hex(substr(Id, 1, 1)) || '-' ||
                 hex(substr(Id, 6, 1)) ||
                 hex(substr(Id, 5, 1)) || '-' ||
                 hex(substr(Id, 8, 1)) ||
                 hex(substr(Id, 7, 1)) || '-' ||
                 hex(substr(Id, 9, 2)) || '-' ||
                 hex(substr(Id, 11, 6));

COMMIT;