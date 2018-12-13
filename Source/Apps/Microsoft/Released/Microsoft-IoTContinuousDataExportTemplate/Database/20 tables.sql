SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE [analytics].[Devices](
	[deviceId] [nvarchar](200) NOT NULL,
	[connectionDeviceId] [nvarchar](200) NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[simulated] [bit] NOT NULL,
	[timestamp] [datetime] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[deviceId] ASC
	)
);

ALTER TABLE [analytics].[Devices]
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = OFF);

CREATE TYPE dbo.DevicesTableType AS TABLE
(
	[deviceId] [nvarchar](200) NOT NULL,
	[connectionDeviceId] [nvarchar](200) NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[simulated] [bit] NOT NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[deviceId] ASC
	)
);

CREATE TABLE [analytics].[MeasurementDefinitions](
	[id] [nvarchar](357) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[kind] [nvarchar](50) NOT NULL,
	[dataType] [nvarchar](100) NULL,
	[name] [nvarchar](200) NOT NULL,
	[category] [nvarchar](100) NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TYPE dbo.MeasurementDefinitionsTableType AS TABLE
(
	[id] [nvarchar](357) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[kind] [nvarchar](50) NOT NULL,
	[dataType] [nvarchar](100) NULL,
	[name] [nvarchar](200) NOT NULL,
	[category] [nvarchar](100) NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

IF EXISTS(select * from sys.database_service_objectives WHERE service_objective = 'S3' OR service_objective LIKE 'P%')	
BEGIN
	CREATE TABLE [analytics].[Measurements](
		[id] [int] IDENTITY(1,1) NOT NULL,
		[messageId] UNIQUEIDENTIFIER NOT NULL,
		[deviceId] [nvarchar](200) NOT NULL,
		[connectionDeviceId] [nvarchar](200) NULL,
		[deviceTemplate] [nvarchar](101) NULL,
		[measurementDefinition] [nvarchar](357) NULL,
		[timestamp] [datetime] NOT NULL,
		[numericValue] [decimal](30, 10) NULL,
		[stringValue] [nvarchar](max) NULL,
		[booleanValue] [bit] NULL,
	);

	EXEC('CREATE CLUSTERED COLUMNSTORE INDEX CCI_Measurements  ON [analytics].[Measurements]');
END
ELSE
BEGIN

	CREATE TABLE [analytics].[Measurements](
		[id] [int] IDENTITY(1,1) NOT NULL,
		[messageId] UNIQUEIDENTIFIER NOT NULL,
		[deviceId] [nvarchar](200) NOT NULL,
		[connectionDeviceId] [nvarchar](200) NULL,
		[deviceTemplate] [nvarchar](101) NULL,
		[measurementDefinition] [nvarchar](357) NULL,
		[timestamp] [datetime] NOT NULL,
		[numericValue] [decimal](30, 10) NULL,
		[stringValue] [nvarchar](max) NULL,
		[booleanValue] [bit] NULL,
		PRIMARY KEY CLUSTERED
		(
			[id] ASC
		)
	);

	CREATE INDEX IX_Measurements_DeviceTemplate
	ON analytics.Measurements
	(
		deviceTemplate
	);

END;

CREATE TABLE [analytics].[DeviceTemplates](
	[id] [nvarchar](101) NOT NULL,
	[deviceTemplateId] [nvarchar](50) NOT NULL,
	[deviceTemplateVersion] [nvarchar](50) NOT NULL,
	[name] [nvarchar](1000) NOT NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TYPE dbo.DeviceTemplatesTableType AS TABLE
(
	[id] [nvarchar](101) NOT NULL,
	[deviceTemplateId] [nvarchar](50) NOT NULL,
	[deviceTemplateVersion] [nvarchar](50) NOT NULL,
	[name] [nvarchar](1000) NOT NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TABLE [analytics].[Properties](
	[id] [nvarchar](507) NOT NULL,
	[deviceId] [nvarchar](200) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[propertyDefinition] [nvarchar](408) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[numericValue] [decimal](30, 10) NULL,
	[stringValue] [nvarchar](max) NULL,
	[booleanValue] [bit] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TYPE [dbo].[PropertiesTableType] AS TABLE
(
	[id] [nvarchar](507) NOT NULL,
	[deviceId] [nvarchar](200) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[propertyDefinition] [nvarchar](408) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[numericValue] [decimal](30, 10) NULL,
	[stringValue] [nvarchar](max) NULL,
	[booleanValue] [bit] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TABLE [analytics].[PropertyDefinitions](
	[id] [nvarchar](408) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[kind] [nvarchar](50) NOT NULL,
	[dataType] [nvarchar](100) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TYPE dbo.PropertyDefinitionsTableType AS TABLE
(
	[id] [nvarchar](408) NOT NULL,
	[deviceTemplate] [nvarchar](101) NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[kind] [nvarchar](50) NOT NULL,
	[dataType] [nvarchar](100) NOT NULL,
	[name] [nvarchar](200) NOT NULL,
	[timestamp] [DATETIME] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

CREATE TABLE [stage].[Measurements](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[messageId] UNIQUEIDENTIFIER NOT NULL,
	[deviceId] [nvarchar](200) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[numericValue] [decimal](30, 10) NULL,
	[stringValue] [nvarchar](max) NULL,
	[booleanValue] [bit] NULL,
	CONSTRAINT [Pk_Fact_Measurements] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
);

ALTER TABLE [stage].[Measurements]
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = OFF);


CREATE TYPE dbo.MeasurementsTableType AS TABLE
(
	[messageId] UNIQUEIDENTIFIER NOT NULL,
	[deviceId] [nvarchar](200) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[field] [nvarchar](255) NOT NULL,
	[numericValue] [decimal](30, 10) NULL,
	[stringValue] [nvarchar](max) NULL,
	[booleanValue] [bit] NULL
);
GO

CREATE TABLE [dbo].[ChangeTracking](
	[TABLE_NAME] NVARCHAR(255),
	[SYS_CHANGE_VERSION] [bigint] NULL
);

CREATE TABLE [dbo].[date](
	[date_key] [int] NOT NULL,
	[full_date] [date] NOT NULL,
	[day_of_week] [tinyint] NOT NULL,
	[day_num_in_month] [tinyint] NOT NULL,
	[day_name] [nvarchar](50) NOT NULL,
	[day_abbrev] [nvarchar](10) NOT NULL,
	[weekday_flag] [char](1) NOT NULL,
	[week_num_in_year] [tinyint] NOT NULL,
	[week_begin_date] [date] NOT NULL,
	[month] [tinyint] NOT NULL,
	[month_name] [nvarchar](50) NOT NULL,
	[month_abbrev] [nvarchar](10) NOT NULL,
	[quarter] [tinyint] NOT NULL,
	[year] [smallint] NOT NULL,
	[yearmo] [int] NOT NULL,
	[same_day_year_ago_date] [date] NOT NULL,
	CONSTRAINT [pk_dim_date] PRIMARY KEY CLUSTERED 
	(
		[date_key] ASC
	)
);

IF EXISTS(select * from sys.database_service_objectives WHERE service_objective = 'S3' OR service_objective LIKE 'P%')	
BEGIN
	CREATE TABLE [analytics].[Messages](
		[id] [uniqueidentifier] NOT NULL,
		[deviceId] [nvarchar](200) NOT NULL,
		[Timestamp] [datetime] NOT NULL,
		[Size] [int] NOT NULL
	);

	EXEC('CREATE CLUSTERED COLUMNSTORE INDEX CCI_Messages  ON [analytics].[Messages]');
END
ELSE
BEGIN
	CREATE TABLE [analytics].[Messages](
		[id] UNIQUEIDENTIFIER NOT NULL,
		[deviceId] [nvarchar](200) NOT NULL,
		[Timestamp] [DATETIME] NOT NULL,
		[Size] INT NOT NULL,
		CONSTRAINT [pk_analytics_Message] PRIMARY KEY NONCLUSTERED 
		(
			[id] ASC
		)
	);
END;

CREATE TYPE dbo.MessagesTableType AS TABLE
(
	[id] UNIQUEIDENTIFIER NOT NULL,
	[deviceId] [nvarchar](200) NOT NULL,
	[Timestamp] [DATETIME] NOT NULL,
	[Size] INT NOT NULL,
	PRIMARY KEY NONCLUSTERED 
	(
		[id] ASC
	)
);