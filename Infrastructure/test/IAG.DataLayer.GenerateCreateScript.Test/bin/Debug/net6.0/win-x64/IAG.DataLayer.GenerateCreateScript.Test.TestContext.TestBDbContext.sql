CREATE TABLE "SchemaVersion" (
    "Id" uuid NOT NULL,
    "Module" character varying(64) NULL,
    "Version" character varying(16) NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp with time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    CONSTRAINT "PK_SchemaVersion" PRIMARY KEY ("Id")
);


CREATE TABLE "SystemLog" (
    "Id" uuid NOT NULL,
    "LogType" integer NOT NULL,
    "Message" text NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp with time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    CONSTRAINT "PK_SystemLog" PRIMARY KEY ("Id")
);


CREATE TABLE "Tenant" (
    "Id" uuid NOT NULL,
    "ParentTenantId" uuid NULL,
    "Name" character varying(32) NULL,
    "TenantContactId" uuid NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp with time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    CONSTRAINT "PK_Tenant" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tenant_Tenant_ParentTenantId" FOREIGN KEY ("ParentTenantId") REFERENCES "Tenant" ("Id")
);


CREATE TABLE "TestEntryA" (
    "Id" uuid NOT NULL,
    "TestStringAColumn" text NULL,
    "TestNumberAColumn" integer NOT NULL,
    "TestBooleanAColumn" boolean NOT NULL,
    CONSTRAINT "PK_TestEntryA" PRIMARY KEY ("Id")
);


CREATE TABLE "TestEntryB" (
    "Id" uuid NOT NULL,
    "TestStringBColumn" text NULL,
    "TestNumberBColumn" integer NOT NULL,
    "TestBooleanBColumn" boolean NOT NULL,
    CONSTRAINT "PK_TestEntryB" PRIMARY KEY ("Id")
);


CREATE TABLE "Division" (
    "Id" uuid NOT NULL,
    "TenantId" uuid NULL,
    "Name" character varying(32) NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp with time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    CONSTRAINT "PK_Division" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Division_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id")
);


CREATE UNIQUE INDEX "IX_Division_TenantId_Name" ON "Division" ("TenantId", "Name");


CREATE UNIQUE INDEX "IX_Tenant_Name" ON "Tenant" ("Name");


CREATE INDEX "IX_Tenant_ParentTenantId" ON "Tenant" ("ParentTenantId");


