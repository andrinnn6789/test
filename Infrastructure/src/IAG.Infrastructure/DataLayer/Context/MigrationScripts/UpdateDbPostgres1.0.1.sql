BEGIN;

CREATE TABLE "Division" (
    "Id" uuid NOT NULL,
    "CreatedOn" timestamp without time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp without time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    "TenantId" uuid NULL,
    "Name" character varying(32) NULL,
    CONSTRAINT "PK_Division" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Division_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_Division_TenantId_Name" ON "Division" ("TenantId", "Name");

CREATE UNIQUE INDEX "IX_Tenant_Name" ON "Tenant" ("Name");

COMMIT;