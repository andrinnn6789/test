BEGIN TRANSACTION;

CREATE TABLE "MobileModule" (
         "Id" BLOB NOT NULL CONSTRAINT "PK_MobileModule" PRIMARY KEY,
         "RowVersion" BLOB NULL,
         "CreatedOn" TEXT NOT NULL,
         "CreatedBy" TEXT NULL,
         "ChangedOn" TEXT NOT NULL,
         "ChangedBy" TEXT NULL,
         "Disabled" INTEGER NOT NULL,
         "TenantId" BLOB NULL,
         "DivisionId" BLOB NULL,
         "Licence" TEXT NULL,
         "ModuleName" TEXT NULL,
         "LicencedUntil" DATE NULL,         
         CONSTRAINT "FK_MobileModule_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
         CONSTRAINT "FK_MobileModule_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
        );

        CREATE INDEX "IX_MobileModule_DivisionId" ON "MobileModule" ("DivisionId");

        CREATE INDEX "IX_MobileModule_Licence" ON "MobileModule" ("Licence");

        CREATE INDEX "IX_MobileModule_ModuleName" ON "MobileModule" ("ModuleName");
        
        CREATE INDEX "IX_MobileModule_TenantId" ON "MobileModule" ("TenantId");
                     
COMMIT;