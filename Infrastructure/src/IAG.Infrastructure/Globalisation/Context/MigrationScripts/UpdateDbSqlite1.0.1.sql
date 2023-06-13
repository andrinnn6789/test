PRAGMA foreign_keys=false;

BEGIN TRANSACTION;

UPDATE "Culture"
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

UPDATE "Resource"
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

UPDATE "Translation"
SET Id = hex(substr(Id, 4, 1)) ||
                 hex(substr(Id, 3, 1)) ||
                 hex(substr(Id, 2, 1)) ||
                 hex(substr(Id, 1, 1)) || '-' ||
                 hex(substr(Id, 6, 1)) ||
                 hex(substr(Id, 5, 1)) || '-' ||
                 hex(substr(Id, 8, 1)) ||
                 hex(substr(Id, 7, 1)) || '-' ||
                 hex(substr(Id, 9, 2)) || '-' ||
                 hex(substr(Id, 11, 6)),
"ResourceId" = hex(substr("ResourceId", 4, 1)) ||
                 hex(substr("ResourceId", 3, 1)) ||
                 hex(substr("ResourceId", 2, 1)) ||
                 hex(substr("ResourceId", 1, 1)) || '-' ||
                 hex(substr("ResourceId", 6, 1)) ||
                 hex(substr("ResourceId", 5, 1)) || '-' ||
                 hex(substr("ResourceId", 8, 1)) ||
                 hex(substr("ResourceId", 7, 1)) || '-' ||
                 hex(substr("ResourceId", 9, 2)) || '-' ||
                 hex(substr("ResourceId", 11, 6)),                 
"CultureId" = hex(substr("CultureId", 4, 1)) ||
                 hex(substr("CultureId", 3, 1)) ||
                 hex(substr("CultureId", 2, 1)) ||
                 hex(substr("CultureId", 1, 1)) || '-' ||
                 hex(substr("CultureId", 6, 1)) ||
                 hex(substr("CultureId", 5, 1)) || '-' ||
                 hex(substr("CultureId", 8, 1)) ||
                 hex(substr("CultureId", 7, 1)) || '-' ||
                 hex(substr("CultureId", 9, 2)) || '-' ||
                 hex(substr("CultureId", 11, 6));

COMMIT;

PRAGMA foreign_keys=true;
