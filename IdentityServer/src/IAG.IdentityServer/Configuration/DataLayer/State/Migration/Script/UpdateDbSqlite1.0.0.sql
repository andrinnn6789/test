BEGIN TRANSACTION;

CREATE TABLE "FailedRequest" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_FailedRequest" PRIMARY KEY,
    "Realm" TEXT NULL,
    "User" TEXT NULL,
    "Timestamp" TEXT NOT NULL,
    "Request" TEXT NULL
);

COMMIT;
