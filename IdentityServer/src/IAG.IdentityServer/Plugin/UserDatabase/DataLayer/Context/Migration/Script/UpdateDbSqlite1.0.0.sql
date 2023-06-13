BEGIN TRANSACTION;

CREATE TABLE "Role" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Role" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NULL,
    CONSTRAINT "FK_Role_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Role_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Scope" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Scope" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NULL,
    CONSTRAINT "FK_Scope_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Scope_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

DROP TABLE IF EXISTS "User";

CREATE TABLE "User" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_User" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "Name" TEXT NULL,
    "Salt" TEXT NULL,
    "Password" TEXT NULL,
    "ChangePasswordAfterLogin" INTEGER NOT NULL,
    "EMail" TEXT NULL,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    CONSTRAINT "FK_User_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_User_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Claim" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Claim" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "TenantId" TEXT NULL,
    "DivisionId" TEXT NULL,
    "ScopeId" TEXT NOT NULL,
    "Name" TEXT NULL,
    "AvailablePermissions" INTEGER NOT NULL,
    CONSTRAINT "FK_Claim_Division_DivisionId" FOREIGN KEY ("DivisionId") REFERENCES "Division" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Claim_Scope_ScopeId" FOREIGN KEY ("ScopeId") REFERENCES "Scope" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Claim_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenant" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "UserRole" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_UserRole" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "FK_UserRole_Role_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Role" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserRole_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RoleClaim" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_RoleClaim" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "RoleId" TEXT NOT NULL,
    "ClaimId" TEXT NOT NULL,
    "AllowedPermissions" INTEGER NOT NULL,
    CONSTRAINT "FK_RoleClaim_Claim_ClaimId" FOREIGN KEY ("ClaimId") REFERENCES "Claim" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RoleClaim_Role_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Role" ("Id") ON DELETE CASCADE
);

CREATE TABLE "UserClaim" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_UserClaim" PRIMARY KEY,
    "RowVersion" BLOB NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "ChangedOn" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "Disabled" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "ClaimId" TEXT NOT NULL,
    "AllowedPermissions" INTEGER NOT NULL,
    CONSTRAINT "FK_UserClaim_Claim_ClaimId" FOREIGN KEY ("ClaimId") REFERENCES "Claim" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserClaim_User_UserId" FOREIGN KEY ("UserId") REFERENCES "User" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Claim_DivisionId" ON "Claim" ("DivisionId");

CREATE INDEX "IX_Claim_TenantId" ON "Claim" ("TenantId");

CREATE UNIQUE INDEX "IX_Claim_ScopeId_Name" ON "Claim" ("ScopeId", "Name");

CREATE INDEX "IX_Role_DivisionId" ON "Role" ("DivisionId");

CREATE UNIQUE INDEX "IX_Role_TenantId_Name" ON "Role" ("TenantId", "Name");

CREATE INDEX "IX_RoleClaim_ClaimId" ON "RoleClaim" ("ClaimId");

CREATE INDEX "IX_RoleClaim_RoleId" ON "RoleClaim" ("RoleId");

CREATE INDEX "IX_Scope_DivisionId" ON "Scope" ("DivisionId");

CREATE UNIQUE INDEX "IX_Scope_TenantId_Name" ON "Scope" ("TenantId", "Name");

CREATE INDEX "IX_User_DivisionId" ON "User" ("DivisionId");

CREATE UNIQUE INDEX "IX_User_TenantId_Name" ON "User" ("TenantId", "Name");

CREATE INDEX "IX_UserClaim_ClaimId" ON "UserClaim" ("ClaimId");

CREATE INDEX "IX_UserClaim_UserId" ON "UserClaim" ("UserId");

CREATE INDEX "IX_UserRole_RoleId" ON "UserRole" ("RoleId");

CREATE INDEX "IX_UserRole_UserId" ON "UserRole" ("UserId");

COMMIT;
