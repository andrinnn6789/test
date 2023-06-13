BEGIN TRANSACTION;

CREATE TABLE "MobileInstallation" (
                                      "Id" BLOB NOT NULL CONSTRAINT "PK_MobileInstallation" PRIMARY KEY,
                                      "RowVersion" BLOB NULL,
                                      "CreatedOn" TEXT NOT NULL,
                                      "CreatedBy" TEXT NULL,
                                      "ChangedOn" TEXT NOT NULL,
                                      "ChangedBy" TEXT NULL,
                                      "Disabled" INTEGER NOT NULL,
                                      "TenantId" BLOB NULL,
                                      "DivisionId" BLOB NULL,
                                      "Url" TEXT NULL,
                                      "Name" TEXT NULL,
                                      "Color" Text NULL,
                                      "SyncInterval" INTEGER NULL,
                                      CONSTRAINT "FK_MobileInstallation_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
                                      CONSTRAINT "FK_MobileInstallation_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "MobileLicence" (
                                 "Id" BLOB NOT NULL CONSTRAINT "PK_MobileLicence" PRIMARY KEY,
                                 "RowVersion" BLOB NULL,
                                 "CreatedOn" TEXT NOT NULL,
                                 "CreatedBy" TEXT NULL,
                                 "ChangedOn" TEXT NOT NULL,
                                 "ChangedBy" TEXT NULL,
                                 "Disabled" INTEGER NOT NULL,
                                 "TenantId" BLOB NULL,
                                 "DivisionId" BLOB NULL,
                                 "Licence" TEXT NULL,
                                 "LicenseStatus" INTEGER NOT NULL,
                                 "DeviceInfo" TEXT NULL,
                                 "DeviceId" TEXT NULL,
                                 CONSTRAINT "FK_MobileLicence_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
                                 CONSTRAINT "FK_MobileLicence_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_MobileInstallation_DivisionId" ON "MobileInstallation" ("DivisionId");

CREATE UNIQUE INDEX "IX_MobileInstallation_Name" ON "MobileInstallation" ("Name");

CREATE INDEX "IX_MobileInstallation_TenantId" ON "MobileInstallation" ("TenantId");

CREATE INDEX "IX_MobileLicence_DivisionId" ON "MobileLicence" ("DivisionId");

CREATE UNIQUE INDEX "IX_MobileLicence_Licence" ON "MobileLicence" ("Licence");

COMMIT;