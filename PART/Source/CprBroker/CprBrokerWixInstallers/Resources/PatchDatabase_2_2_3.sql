-- If the new columns are not there, drop the cache table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PersonSearchCache') AND name = 'MunicipalityCode' )
BEGIN
    -- Only if the table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PersonSearchCache')
        DROP TABLE PersonSearchCache
END

GO