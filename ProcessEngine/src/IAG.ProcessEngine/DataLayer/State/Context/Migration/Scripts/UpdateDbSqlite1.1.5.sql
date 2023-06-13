BEGIN TRANSACTION;

DROP INDEX IF EXISTS IX_JobState_DateRunEnd;

CREATE INDEX IX_JobState_DateRunEnd ON JobState (DateRunEnd);

DROP INDEX IF EXISTS IX_JobState_DateDue;

CREATE INDEX IX_JobState_DateDue ON JobState (DateDue);

COMMIT;