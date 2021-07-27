CREATE SCHEMA sfraw
CREATE TABLE [sfraw].[CrouterOutput]
(
	[SubjectId] [bigint] NOT NULL,
	[DeviceId] [nvarchar](130) NOT NULL,
	[SettingsId] [uniqueidentifier] NOT NULL,
	[TimestampUtc] [datetime] NOT NULL,
	[CutPointBucketVectorMagnitude] [nvarchar](200) NOT NULL,
	[CutPointBucketVerticalAxis] [nvarchar](200) NOT NULL
)
WITH
(
	DISTRIBUTION = HASH ( [SubjectId] ),
	CLUSTERED COLUMNSTORE INDEX
);

CREATE TABLE [sfraw].[DustinTracyOutput]
(
	[SubjectId] [bigint] NOT NULL,
	[DeviceId] [nvarchar](130) NOT NULL,
	[SettingsId] [uniqueidentifier] NOT NULL,
	[SleepPeriodStartUtc] [datetime] NOT NULL,
	[SleepPeriodEndUtc] [datetime] NOT NULL
)
WITH
(
	DISTRIBUTION = HASH ( [SubjectId] ),
	CLUSTERED COLUMNSTORE INDEX
);

CREATE TABLE [sfraw].[ChoiOutput]
(
	[SubjectId] [bigint] NOT NULL,
	[DeviceId] [nvarchar](130) NOT NULL,
	[SettingsId] [uniqueidentifier] NOT NULL,
	[WearPeriodStartUtc] [datetime] NOT NULL,
	[WearPeriodEndUtc] [datetime] NOT NULL
	
)
WITH
(
	DISTRIBUTION = HASH ( [SubjectId] ),
	CLUSTERED COLUMNSTORE INDEX
);

CREATE TABLE [sfraw].[CountsOutput]
(
	[SubjectId] [bigint] NOT NULL,
	[DeviceId] [nvarchar](130) NOT NULL,
	[SettingsId] [uniqueidentifier] NOT NULL,
	[TimestampUtc] [datetime] NOT NULL,
	[XAxis] [int] NOT NULL,
	[YAxis] [int] NOT NULL,
	[ZAxis] [int] NOT NULL
	
)
WITH
(
	DISTRIBUTION = HASH ( [SubjectId] ),
	CLUSTERED COLUMNSTORE INDEX
);