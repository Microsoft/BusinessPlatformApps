SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database
DECLARE @stmt AS VARCHAR(500), @p1 AS VARCHAR(100), @p2 AS VARCHAR(100);
DECLARE @cr CURSOR;

-- Must be executed inside the target database

-- Drop Tables
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='BASE TABLE' AND
                    TABLE_SCHEMA='cc' AND
                    TABLE_NAME IN ('configuration');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP TABLE cc.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;

-- Drop Stored Procedures
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES
              WHERE ROUTINE_TYPE='PROCEDURE' AND 
                    ( (ROUTINE_SCHEMA='cc' AND ROUTINE_NAME IN ('sp_get_replication_counts', 'sp_get_prior_content', 'sp_get_last_updatetime','sp_get_pull_status'))
                    );
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1, @p2;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP PROCEDURE ' + QuoteName(@p1) + '.' + QuoteName(@p2);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1, @p2;
END;
CLOSE @cr;
DEALLOCATE @cr;

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='cc')
BEGIN
    EXEC ('CREATE SCHEMA cc AUTHORIZATION dbo'); -- Avoid batch error
END;

