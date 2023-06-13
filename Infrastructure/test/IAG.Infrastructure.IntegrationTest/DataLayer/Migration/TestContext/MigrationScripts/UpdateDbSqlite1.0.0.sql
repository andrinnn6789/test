
CREATE TABLE TestEntity (
    "Id" BLOB NOT NULL CONSTRAINT "PK_TestEntity" PRIMARY KEY,
    "TestNumber" INTEGER NOT NULL,
    "TestString" TEXT NULL,
	"CheckCreatedByScript" INTEGER DEFAULT 1
);
