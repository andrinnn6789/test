BEGIN TRANSACTION;

DROP TABLE "Installation";

CREATE TABLE "Installation" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Installation" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "CustomerId" TEXT NOT NULL,
    "ProductId" TEXT NULL,
    "ReleaseVersion" TEXT NULL,
    "Platform" TEXT NULL,
    "InstanceName" TEXT NULL,
    "Description" TEXT NULL,
    CONSTRAINT "FK_Installation_Customer_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customer" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Installation_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Installation_Product_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Product" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Installation_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Installation_DivisionId" ON "Installation" ("DivisionId");

CREATE INDEX "IX_Installation_ReleaseId" ON "Installation" ("ReleaseId");

CREATE INDEX "IX_Installation_TenantId" ON "Installation" ("TenantId");

COMMIT;