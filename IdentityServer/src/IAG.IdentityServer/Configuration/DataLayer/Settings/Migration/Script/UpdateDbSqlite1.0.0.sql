BEGIN TRANSACTION;

CREATE TABLE "ConfigIdentity" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_ConfigIdentity" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "Name" TEXT NULL,
    "Data" TEXT NULL
);

COMMIT;
