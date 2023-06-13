BEGIN TRANSACTION;

DROP TABLE IF EXISTS "Translation";
DROP TABLE IF EXISTS "Resource";
DROP TABLE IF EXISTS "Culture";

CREATE TABLE "Resource" (
	"Id"	BLOB NOT NULL CONSTRAINT "PK_Resource" PRIMARY KEY,
	"RowVersion"	BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled"	INTEGER NOT NULL,
	"Name"	TEXT
);

CREATE TABLE "Culture" (
	"Id"	BLOB NOT NULL CONSTRAINT "PK_Culture" PRIMARY KEY,
	"RowVersion"	BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled"	INTEGER NOT NULL,
	"Name"	TEXT
);

CREATE TABLE "Translation" (
	"Id"	BLOB NOT NULL CONSTRAINT "PK_Translation" PRIMARY KEY,
	"RowVersion"	BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled"	INTEGER NOT NULL,
	"ResourceId"	BLOB NOT NULL,
	"CultureId"	BLOB NOT NULL,
	"Value"	TEXT,
	CONSTRAINT "FK_Translation_Resource_ResourceId" FOREIGN KEY("ResourceId") REFERENCES "Resource"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_Translation_Culture_CultureId" FOREIGN KEY("CultureId") REFERENCES "Culture"("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Translation_ResourceId" ON "Translation" (
	"ResourceId"
);

CREATE INDEX "IX_Translation_CultureId" ON "Translation" (
	"CultureId"
);

CREATE UNIQUE INDEX "IX_Resource_Name" ON "Resource" (
	"Name"
);

CREATE UNIQUE INDEX "IX_Culture_Name" ON "Culture" (
	"Name"
);

COMMIT;
