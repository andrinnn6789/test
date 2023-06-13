BEGIN TRANSACTION;

CREATE TABLE "ConfigProcessEngine110" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_ConfigProcessEngine" PRIMARY KEY,
	"RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled" INTEGER NOT NULL,
	"Name" TEXT UNIQUE,
	"TemplateId" BLOB NOT NULL,
	"Data" TEXT,
	"Description" TEXT
);

INSERT INTO ConfigProcessEngine110
SELECT Id, RowVersion, CreatedOn, CreatedBy, ChangedOn, ChangedBy, Disabled, Name, Id, Data, ""
FROM ConfigProcessEngine;

DROP TABLE ConfigProcessEngine;

ALTER TABLE ConfigProcessEngine110 RENAME TO ConfigProcessEngine;

COMMIT;