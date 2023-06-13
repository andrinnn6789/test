BEGIN TRANSACTION;

ALTER TABLE "MobileLicence" RENAME COLUMN "LicenseStatus" TO "LicenceStatus";

COMMIT;