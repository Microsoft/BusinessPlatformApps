-- Enable database change tracking 
IF NOT EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = db_id('continuousDataExportDB'))
BEGIN
	ALTER DATABASE [continuousDataExportDB]  
	SET CHANGE_TRACKING = ON  
	(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)
END