PRAGMA foreign_keys=false;

BEGIN TRANSACTION;

UPDATE "JobData"
SET Id = hex(substr(Id, 4, 1)) ||
                 hex(substr(Id, 3, 1)) ||
                 hex(substr(Id, 2, 1)) ||
                 hex(substr(Id, 1, 1)) || '-' ||
                 hex(substr(Id, 6, 1)) ||
                 hex(substr(Id, 5, 1)) || '-' ||
                 hex(substr(Id, 8, 1)) ||
                 hex(substr(Id, 7, 1)) || '-' ||
                 hex(substr(Id, 9, 2)) || '-' ||
                 hex(substr(Id, 11, 6));

UPDATE "JobState"
SET Id = hex(substr(Id, 4, 1)) ||
                 hex(substr(Id, 3, 1)) ||
                 hex(substr(Id, 2, 1)) ||
                 hex(substr(Id, 1, 1)) || '-' ||
                 hex(substr(Id, 6, 1)) ||
                 hex(substr(Id, 5, 1)) || '-' ||
                 hex(substr(Id, 8, 1)) ||
                 hex(substr(Id, 7, 1)) || '-' ||
                 hex(substr(Id, 9, 2)) || '-' ||
                 hex(substr(Id, 11, 6)),
"ParentJobId" = hex(substr("ParentJob", 4, 1)) ||
                 hex(substr("ParentJobId", 3, 1)) ||
                 hex(substr("ParentJobId", 2, 1)) ||
                 hex(substr("ParentJobId", 1, 1)) || '-' ||
                 hex(substr("ParentJobId", 6, 1)) ||
                 hex(substr("ParentJobId", 5, 1)) || '-' ||
                 hex(substr("ParentJobId", 8, 1)) ||
                 hex(substr("ParentJobId", 7, 1)) || '-' ||
                 hex(substr("ParentJobId", 9, 2)) || '-' ||
                 hex(substr("ParentJobId", 11, 6)),
"TemplateId" = hex(substr("TemplateId", 4, 1)) ||
                 hex(substr("TemplateId", 3, 1)) ||
                 hex(substr("TemplateId", 2, 1)) ||
                 hex(substr("TemplateId", 1, 1)) || '-' ||
                 hex(substr("TemplateId", 6, 1)) ||
                 hex(substr("TemplateId", 5, 1)) || '-' ||
                 hex(substr("TemplateId", 8, 1)) ||
                 hex(substr("TemplateId", 7, 1)) || '-' ||
                 hex(substr("TemplateId", 9, 2)) || '-' ||
                 hex(substr("TemplateId", 11, 6)),                 
"MetadataId" = hex(substr("MetadataId", 4, 1)) ||
                 hex(substr("MetadataId", 3, 1)) ||
                 hex(substr("MetadataId", 2, 1)) ||
                 hex(substr("MetadataId", 1, 1)) || '-' ||
                 hex(substr("MetadataId", 6, 1)) ||
                 hex(substr("MetadataId", 5, 1)) || '-' ||
                 hex(substr("MetadataId", 8, 1)) ||
                 hex(substr("MetadataId", 7, 1)) || '-' ||
                 hex(substr("MetadataId", 9, 2)) || '-' ||
                 hex(substr("MetadataId", 11, 6));

COMMIT;

PRAGMA foreign_keys=true;

