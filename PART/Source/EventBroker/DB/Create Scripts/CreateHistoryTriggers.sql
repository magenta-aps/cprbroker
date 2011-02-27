/*
	Creates history monitoring triggers for some tables
	The main idea is to get a list of all the tables that have matching history tables
	For each table, get all the matching columns
	Then create a trigger for each operation type
	Trigger creation is made by building a SQL statement (Dynamic SQL) and executing it
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'CreateHistoryTriggers')
	BEGIN
		DROP  Procedure  CreateHistoryTriggers
	END

GO

CREATE Procedure CreateHistoryTriggers
	
AS

	DECLARE @HistoryPrefix VARCHAR(50);
	SET @HistoryPrefix = 'History'
	
	DECLARE @SQL VARCHAR(MAX)
	SET @SQL = ''
	DECLARE @Tables TABLE (
		[Name] VARCHAR(250)
		)
	INSERT INTO @Tables 
	SELECT T.Name
	FROM sys.Tables AS T 
	INNER JOIN sys.Tables AS HT ON HT.Name = T.Name + @HistoryPrefix
	
	DECLARE @TableName VARCHAR(250)
	DECLARE TableCursor CURSOR FOR SELECT [Name] FROM @Tables
	
	-- Loop over all tables
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @TableName
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		DECLARE @OperationTypeId INT
		DECLARE @OperationName VARCHAR(50)
		DECLARE @SourceTableName VARCHAR(50)
		DECLARE @OldTableName VARCHAR(50)
		
		DECLARE @HistoryTableName VARCHAR(250)
		SET @HistoryTableName = @TableName + @HistoryPrefix 
		
		DECLARE @Columns TABLE ([Name] VARCHAR(250) )
		
		-- Get a list of columns that are in both the main table and the history table
		INSERT INTO 
			@Columns ([Name])
		SELECT 
			C.Name 
		FROM sys.Tables AS T,
			sys.Columns AS C,
			sys.Tables AS HT,
			sys.Columns AS HC		
		WHERE 
				T.Name = @TableName
			AND C.object_id = T.object_id
			AND HT.Name = @HistoryTableName
			AND HC.object_id = HT.object_id
			AND HC.Name = C.Name
			
		
		-- Now create a cursor to loop over operations from OperationType	
		DECLARE OperationCursor CURSOR FOR 
		SELECT OperationTypeId, OperationName, SourceTableName, OldTableName
		FROM OperationType		
		
		OPEN OperationCursor
		FETCH NEXT FROM OperationCursor INTO @OperationTypeId, @OperationName, @SourceTableName, @OldTableName
		-- Loop over all operations 
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Set trigger name
			DECLARE @TriggerName VARCHAR(250)
			SET @TriggerName = @TableName + '_' + @OperationName ;
			
			-- Drop existing trigger (if any)
			SET @SQL = '' 
			+ 'IF EXISTS (SELECT * FROM sys.Triggers WHERE Name = ''' + @TriggerName +''')' + CHAR(10)
			+ ' DROP TRIGGER ' + @TriggerName + CHAR(10)
			
			EXEC (@SQL)
			
			-- Create new trigger
			SET @SQL = '' 			
			+ 'CREATE TRIGGER [' + @TriggerName + ']' + CHAR(10)
			+ ' ON [' + @TableName + ']'
			+ ' FOR ' + @OperationName + CHAR(10)
			+ ' AS ' + CHAR(10)
			
			-- If there are two tables (incase of update), only insert if at least one column has a new value
			IF @OldTableName IS NOT NULL
			BEGIN
				SET @SQL = @SQL + ' IF ( ' + CHAR(10)
				SELECT @SQL = @SQL + ' dbo.Equals( '
				+'(SELECT TOP 1 ['+ Name + '] FROM '+ @SourceTableName +') , (SELECT TOP 1 ['+ Name + '] FROM '+ @OldTableName + ')) = 0 OR ' + CHAR(10)
				FROM @Columns
				SET @SQL = CHAR(10) + SUBSTRING(@SQL, 1,  LEN(@SQL) - 4 ) + CHAR(10) + ' ) ' + CHAR(10)
			END
			
			-- Insert operation related columns
			SET @SQL = @SQL
			+ ' INSERT INTO [' + @HistoryTableName + ']' + CHAR(10)
			+ ' (OperationTypeId, ChangeDate'
			
			-- Insert data columns
			SELECT @SQL = @SQL + ', ' + [Name]
			FROM @Columns
					
			-- Operation column values
			SET @SQL = @SQL + ')' + CHAR(10)
			+ ' SELECT ' + CHAR(10)
			+ CAST (@OperationTypeId AS VARCHAR(1) ) + ', '
			+ 'getdate()'
			
			-- Data column values
			SELECT @SQL = @SQL + ',' + [Name]
			FROM @Columns
			
			SET @SQL = @SQL  + CHAR(10)
			+ ' FROM ' + @SourceTableName + CHAR(10) 
			+ CHAR(10)
			
			PRINT @SQL
			EXEC (@SQL)
			
			FETCH NEXT FROM OperationCursor INTO @OperationTypeId, @OperationName, @SourceTableName, @OldTableName
		END
		
		CLOSE OperationCursor
		DEALLOCATE OperationCursor
		
		DELETE @Columns
		FETCH NEXT FROM TableCursor	INTO @TableName
	END
	CLOSE TableCursor
	DEALLOCATE TableCursor
	
GO


