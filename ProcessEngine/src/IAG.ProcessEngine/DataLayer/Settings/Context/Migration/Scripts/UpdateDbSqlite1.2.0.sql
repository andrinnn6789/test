BEGIN TRANSACTION;

CREATE TABLE "FollowUpJob" (
	"Id" BLOB NOT NULL CONSTRAINT "PK_FollowUpJob" PRIMARY KEY,
	"RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
	"Disabled" INTEGER NOT NULL,
	"MasterId" BLOB NOT NULL,
	"FollowUpId" BLOB NOT NULL,
    "ExecutionCondition" TEXT,
    "Description" TEXT
);

COMMIT;