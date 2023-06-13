BEGIN TRANSACTION;

CREATE UNIQUE INDEX "IX_Translation_ResourceId_CultureId" ON "Translation" (
	"ResourceId", "CultureId"
);

COMMIT;
