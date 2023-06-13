BEGIN TRANSACTION;

CREATE TABLE "ConfigCommon" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_ConfigCommon" PRIMARY KEY,
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
