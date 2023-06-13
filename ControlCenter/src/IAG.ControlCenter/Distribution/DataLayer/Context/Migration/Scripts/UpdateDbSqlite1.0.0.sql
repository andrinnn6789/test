BEGIN TRANSACTION;

CREATE TABLE "FileStore" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_FileStore" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NOT NULL,
    "Data" BLOB NULL,
    "FileVersion" TEXT NULL,
    "ProductVersion" TEXT NULL,
    "Checksum" BLOB NULL,
    "FileLastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_FileStore_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_FileStore_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Product" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Product" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "ProductType" INTEGER NOT NULL,
    "DependsOnProductId" TEXT NULL,
    CONSTRAINT "FK_Product_Product_DependsOnProductId" FOREIGN KEY ("DependsOnProductId") REFERENCES "Product" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Product_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Product_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Release" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Release" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "ProductId" TEXT NOT NULL,
    "ReleaseVersion" TEXT NOT NULL,
    "Platform" TEXT NOT NULL,
    "ReleaseDate" TEXT NULL,
    "Description" TEXT NULL,
    "ReleasePath" TEXT NULL,
    "ArtifactPath" TEXT NULL,
    CONSTRAINT "FK_Release_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Release_Product_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Product" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Release_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ReleaseFileStore" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_ReleaseFileStore" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "FileStoreId" TEXT NOT NULL,
    "ReleaseId" TEXT NOT NULL,
    CONSTRAINT "FK_ReleaseFileStore_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ReleaseFileStore_FileStore_FileStoreId" FOREIGN KEY ("FileStoreId") REFERENCES "FileStore" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ReleaseFileStore_Release_ReleaseId" FOREIGN KEY ("ReleaseId") REFERENCES "Release" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ReleaseFileStore_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Customer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Customer" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NULL,
    "CustomerCategoryId" INTEGER NOT NULL,
    "Description" TEXT NULL,
    CONSTRAINT "FK_Customer_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Customer_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ProductCustomer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_ProductCustomer" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "CustomerId" TEXT NOT NULL,
    "ProductId" TEXT NOT NULL,
    CONSTRAINT "FK_ProductCustomer_Customer_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customer" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProductCustomer_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ProductCustomer_Product_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Product" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProductCustomer_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

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
    "ProductId" TEXT NOT NULL,
    "ReleaseVersion" TEXT NOT NULL,
    "Platform" TEXT NOT NULL,
    "InstanceName" TEXT NULL,
    "Description" TEXT NULL,
    CONSTRAINT "FK_Installation_Customer_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customer" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Installation_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Installation_Product_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Product" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Installation_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE Link (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Link" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NOT NULL,
    "Url" TEXT NOT NULL,
    "Description" TEXT NULL
);

CREATE INDEX "IX_FileStore_DivisionId" ON "FileStore" ("DivisionId");

CREATE INDEX "IX_FileStore_TenantId" ON "FileStore" ("TenantId");

CREATE INDEX "IX_Product_DivisionId" ON "Product" ("DivisionId");

CREATE INDEX "IX_Product_TenantId" ON "Product" ("TenantId");

CREATE INDEX "IX_Release_DivisionId" ON "Release" ("DivisionId");

CREATE INDEX "IX_Release_ProductId" ON "Release" ("ProductId");

CREATE INDEX "IX_Release_TenantId" ON "Release" ("TenantId");

CREATE INDEX "IX_ReleaseFileStore_DivisionId" ON "ReleaseFileStore" ("DivisionId");

CREATE INDEX "IX_ReleaseFileStore_FileStoreId" ON "ReleaseFileStore" ("FileStoreId");

CREATE INDEX "IX_ReleaseFileStore_TenantId" ON "ReleaseFileStore" ("TenantId");

CREATE UNIQUE INDEX "IX_ReleaseFileStore_ReleaseId_FileStoreId" ON "ReleaseFileStore" ("ReleaseId", "FileStoreId");

CREATE INDEX "IX_Customer_DivisionId" ON "Customer" ("DivisionId");

CREATE INDEX "IX_Customer_TenantId" ON "Customer" ("TenantId");

CREATE INDEX "IX_ProductCustomer_CustomerId" ON "ProductCustomer" ("CustomerId");

CREATE INDEX "IX_ProductCustomer_DivisionId" ON "ProductCustomer" ("DivisionId");

CREATE INDEX "IX_ProductCustomer_TenantId" ON "ProductCustomer" ("TenantId");

CREATE UNIQUE INDEX "IX_ProductCustomer_ProductId_CustomerId" ON "ProductCustomer" ("ProductId", "CustomerId");

CREATE INDEX "IX_Installation_DivisionId" ON "Installation" ("DivisionId");

CREATE INDEX "IX_Installation_ReleaseId" ON "Installation" ("ReleaseId");

CREATE INDEX "IX_Installation_TenantId" ON "Installation" ("TenantId");

COMMIT;