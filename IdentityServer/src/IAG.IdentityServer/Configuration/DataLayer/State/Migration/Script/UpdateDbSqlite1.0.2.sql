CREATE TABLE RefreshToken (
    "Id" BLOB NOT NULL CONSTRAINT "PK_RefreshTokent" PRIMARY KEY,
    "Timestamp" TEXT NOT NULL,
    "PreviousRefreshToken" TEXT NULL,
    "User" TEXT NOT NULL,
    "Tenant" BLOB NULL,
    AuthenticationToken TEXT NOT NULL
);
