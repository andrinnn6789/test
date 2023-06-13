BEGIN TRANSACTION;

-- Remove FK to allow saving followup-jobs before parent job
DROP INDEX IF EXISTS "IX_JobState_ParentJobId";

CREATE TABLE "JobStateTemp" (
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
	"JobConfigId" BLOB NULL,
	"Acknowledged" INT NOT NULL DEFAULT 0,
	"ContextTenant" TEXT NULL
	);

INSERT INTO JobStateTemp SELECT * FROM JobState;

UPDATE JobState SET ParentJobId = NULL;

DROP TABLE JobState;

ALTER TABLE JobStateTemp RENAME TO JobState;

CREATE INDEX "IX_JobState_ParentJobId" ON "JobState" ("ParentJobId");

COMMIT;