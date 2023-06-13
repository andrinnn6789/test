BEGIN TRANSACTION;

CREATE TABLE "ConfigProcessEngine" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_ConfigProcessEngine" PRIMARY KEY,
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