BEGIN TRANSACTION;

CREATE INDEX "IX_FileStore_Product" ON "FileStore" (
	"Name",
	"FileVersion",
	"ProductVersion"
);

COMMIT;