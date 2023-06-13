BEGIN TRANSACTION;

CREATE TABLE "ConfigCoreServer" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_ConfigCoreServer" PRIMARY KEY,
	"RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled" INTEGER NOT NULL,
	"Data" TEXT,
	"Name" TEXT
);

COMMIT;
