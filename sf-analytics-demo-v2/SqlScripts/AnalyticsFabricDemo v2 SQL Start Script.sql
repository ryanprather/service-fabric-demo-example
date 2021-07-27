CREATE SCHEMA ServiceFabric
GO
CREATE TABLE ServiceFabric.StudySubject
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
StudyId bigint not null,
SubjectId bigint not null,
)
GO
CREATE TABLE ServiceFabric.SubjectDevice
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
StudySubjectId UNIQUEIDENTIFIER not null,
DeviceSerial varchar(50) not null,
FOREIGN KEY (StudySubjectId) REFERENCES ServiceFabric.StudySubject(Id)
)
GO
CREATE TABLE ServiceFabric.SubjectDeviceUpload
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
SubjectDeviceId UNIQUEIDENTIFIER not null,
UploadFileId bigint not null,
BeginTimestampUtc DateTime NOT NULL,
EndTimestampUtc DateTime NOT NULL,
FOREIGN KEY (SubjectDeviceId) REFERENCES ServiceFabric.SubjectDevice(Id)
)
CREATE TABLE ServiceFabric.UploadProcessingJob
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
SubjectDeviceUploadId UNIQUEIDENTIFIER not null,
CreatedDateTimeUtc DateTime not null,
ProcessingStartedUtc DateTime NULL,
CompletedDateTimeUtc DateTime NULL,
IsError BIT DEFAULT 0,
ErrorReason nvarchar(max)
FOREIGN KEY (SubjectDeviceUploadId) REFERENCES ServiceFabric.SubjectDeviceUpload(Id)
)
GO


CREATE TABLE ServiceFabric.AlgorithmTask
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
UploadProcessingJobId UNIQUEIDENTIFIER not null,
AlgorithmSettingId UNIQUEIDENTIFIER not null,
CreatedDateTimeUtc DateTime not null,
ProcessingStartedUtc DateTime NULL,
ProcessingCompletedDateTimeUtc DateTime NULL,
StorageStartedUtc DateTime NULL,
StorageCompletedUtc DateTime NULL,
AdjustedBeginTimestampUtc DateTime not NULL,
AdjustedEndTimestampUtc DateTime not NULL,
ItemsComputed bigint DEFAULT 0,
ItemsStored bigint DEFAULT 0,
IsError BIT DEFAULT 0,
ErrorReason nvarchar(max)
FOREIGN KEY (UploadProcessingJobId) REFERENCES ServiceFabric.UploadProcessingJob(Id),
FOREIGN KEY (AlgorithmSettingId) REFERENCES Analytics.AlgorithmSettings(Id)
)
GO

CREATE TABLE ServiceFabric.ProcessingJobEpochRetrieval
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
UploadProcessingJobId UNIQUEIDENTIFIER not null,
ProcessingStartedUtc DateTime NULL,
ProcessingCompletedDateTimeUtc DateTime NULL,
AdjustedBeginTimestampUtc DateTime NULL,
AdjustedEndTimestampUtc DateTime NULL,
IsError BIT DEFAULT 0,
ErrorReason nvarchar(max)
FOREIGN KEY (UploadProcessingJobId) REFERENCES ServiceFabric.UploadProcessingJob(Id)
)
GO

CREATE TABLE ServiceFabric.DustinTracyAlgorithmStates
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
SubjectId bigint not NULL,
AlgorithmSettingId UNIQUEIDENTIFIER not null,
DataStartTimestamp DateTime not null,
FOREIGN KEY (AlgorithmSettingId) REFERENCES Analytics.AlgorithmSettings(Id)
)
GO

CREATE TABLE ServiceFabric.ChoiAlgorithmStates
(
Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
SubjectId bigint not NULL,
AlgorithmSettingId UNIQUEIDENTIFIER not null,
DataStartTimestamp DateTime not null,
FOREIGN KEY (AlgorithmSettingId) REFERENCES Analytics.AlgorithmSettings(Id)
)
GO

SELECT * FROM ServiceFabric.AlgorithmTask
SELECT * FROM ServiceFabric.ProcessingJobEpochRetrieval
SELECT * FROM ServiceFabric.UploadProcessingJob

DELETE FROM ServiceFabric.AlgorithmTask
DELETE FROM  ServiceFabric.ProcessingJobEpochRetrieval
DELETE FROM ServiceFabric.UploadProcessingJob





