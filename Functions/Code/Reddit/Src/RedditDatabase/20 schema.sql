IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='reddit')
BEGIN
    EXEC ('CREATE SCHEMA reddit AUTHORIZATION dbo'); -- Avoid batch error
END;
