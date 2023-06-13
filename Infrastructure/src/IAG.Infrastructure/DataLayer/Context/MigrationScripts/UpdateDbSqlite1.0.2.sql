BEGIN TRANSACTION;

DROP TABLE IF EXISTS "Division";
DROP INDEX IF EXISTS "IX_Division_TenantId_Name";
DROP INDEX IF EXISTS "IX_Tenant_Name";

CREATE TABLE "Division" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_Division" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" BLOB NULL,
    "Name" TEXT NULL,
    CONSTRAINT "FK_Division_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_Division_TenantId_Name" ON "Division" ("TenantId", "Name");

CREATE UNIQUE INDEX "IX_Tenant_Name" ON "Tenant" ("Name");

COMMIT;
