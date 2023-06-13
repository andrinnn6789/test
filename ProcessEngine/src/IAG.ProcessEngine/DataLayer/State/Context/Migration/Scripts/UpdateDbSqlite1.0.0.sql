BEGIN TRANSACTION;

CREATE TABLE "JobData" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_JobData" PRIMARY KEY,
	"RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled" INTEGER NOT NULL,
	"Data" TEXT,
	"Name" TEXT
);

DROP TABLE IF EXISTS "JobState";
CREATE TABLE "JobState" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_JobState" PRIMARY KEY,
	"RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled" INTEGER NOT NULL,
    "ParentJobId" BLOB NULL,
    "TemplateId" BLOB NULL,
    "DateCreation" TEXT NOT NULL,
    "DateDue" TEXT NOT NULL,
    "IsBlocking" INTEGER NOT NULL,
    "MetadataId" BLOB NULL,
    "Owner" TEXT NULL,
    "RetryCounter" INTEGER NOT NULL,
    "ParameterType" TEXT NULL,
    "Parameter" TEXT NULL,
    "DateRunStart" TEXT NULL,
    "Progress" REAL NOT NULL,
    "ExecutionState" INTEGER NOT NULL,
    "DateRunEnd" TEXT NULL,
    "Messages" TEXT NULL,
    "ResultType" TEXT NULL,
    "Result" TEXT NULL,
    CONSTRAINT "FK_JobState_JobState_ParentJobId" FOREIGN KEY ("ParentJobId") REFERENCES "JobState" ("Id") ON DELETE RESTRICT
);

DROP INDEX IF EXISTS "IX_JobState_ParentJobId";
CREATE INDEX "IX_JobState_ParentJobId" ON "JobState" ("ParentJobId");

COMMIT;