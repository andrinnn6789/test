
CREATE TABLE "SystemLog" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_SystemLog" PRIMARY KEY,
    "RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
    "Disabled" INTEGER NOT NULL,
    "LogType" INTEGER NOT NULL,
    "Message" TEXT
);

CREATE TABLE "SchemaVersion" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_SchemaVersion" PRIMARY KEY,
    "RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
    "Disabled" INTEGER NOT NULL,
    "Module" TEXT,
    "Version" TEXT
);

CREATE TABLE "Tenant" (
    "Id" BLOB NOT NULL CONSTRAINT "PK_Tenant" PRIMARY KEY,
    "RowVersion" BLOB,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT,
    "Disabled" INTEGER NOT NULL,
    "ParentTenantId" BLOB,
    "Name" TEXT,
    "TenantContactId" BLOB,
    CONSTRAINT "FK_Tenant_Tenant_ParentTenantId" FOREIGN KEY ("ParentTenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);
