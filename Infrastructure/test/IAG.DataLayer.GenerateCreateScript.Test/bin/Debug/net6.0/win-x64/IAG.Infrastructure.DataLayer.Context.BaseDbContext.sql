CREATE TABLE "SchemaVersion" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_SchemaVersion" PRIMARY KEY,
    "Module" TEXT NULL,
    "Version" TEXT NULL,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL
);


CREATE TABLE "SystemLog" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_SystemLog" PRIMARY KEY,
    "LogType" INTEGER NOT NULL,
    "Message" TEXT NULL,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL
);


CREATE TABLE "Tenant" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Tenant" PRIMARY KEY,
    "ParentTenantId" TEXT NULL,
    "Name" TEXT NULL,
    "TenantContactId" TEXT NULL,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    CONSTRAINT "FK_Tenant_Tenant_ParentTenantId" FOREIGN KEY ("ParentTenantId") REFERENCES "Tenant" ("Id")
);


CREATE TABLE "Division" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Division" PRIMARY KEY,
    "TenantId" TEXT NULL,
    "Name" TEXT NULL,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    CONSTRAINT "FK_Division_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id")
);


CREATE UNIQUE INDEX "IX_Division_TenantId_Name" ON "Division" ("TenantId", "Name");


CREATE UNIQUE INDEX "IX_Tenant_Name" ON "Tenant" ("Name");


CREATE INDEX "IX_Tenant_ParentTenantId" ON "Tenant" ("ParentTenantId");


