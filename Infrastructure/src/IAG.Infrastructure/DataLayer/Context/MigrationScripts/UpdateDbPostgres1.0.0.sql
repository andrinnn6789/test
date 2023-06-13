
CREATE TABLE "SystemLog" (
    "Id" uuid NOT NULL,
    "CreatedOn" timestamp without time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp without time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    "LogType" integer NOT NULL,
    "Message" text NULL,
    CONSTRAINT "PK_SystemLog" PRIMARY KEY ("Id")
);

CREATE TABLE "SchemaVersion" (
    "Id" uuid NOT NULL,
    "CreatedOn" timestamp without time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp without time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    "Module" character varying(64) NULL,
    "Version" character varying(16) NULL,
    CONSTRAINT "PK_SchemaVersion" PRIMARY KEY ("Id")
);

CREATE TABLE "Tenant" (
    "Id" uuid NOT NULL,
    "CreatedOn" timestamp without time zone NOT NULL,
    "CreatedBy" character varying(32) NULL,
    "ChangedOn" timestamp without time zone NOT NULL,
    "ChangedBy" character varying(32) NULL,
    "Disabled" boolean NOT NULL,
    "ParentTenantId" uuid NULL,
    "Name" character varying(32) NULL,
    "TenantContactId" uuid NULL,
    CONSTRAINT "PK_Tenant" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tenant_Tenant_ParentTenantId" FOREIGN KEY ("ParentTenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);
