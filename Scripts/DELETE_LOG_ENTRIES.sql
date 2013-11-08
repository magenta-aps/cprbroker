declare @latest_time datetime, @cur_time datetime, @output_msg varchar(max)
select top 1 @latest_time = LogDate from LogEntry order by LogDate asc

while (@latest_time < '2013-01-01 00:00:00.000')
	begin
		set @output_msg = N'Deleting 1000 records from ' + CONVERT(varchar, @latest_time, 121);
		RAISERROR (@output_msg, 0, 1) WITH NOWAIT
		delete from LogEntry where LogDate in (
			select top(1000) LogDate from LogEntry where LogDate < '2013-01-01 00:00:00.000' order by LogDate asc
		)
		select top 1 @latest_time = LogDate from LogEntry order by LogDate asc
		set @cur_time = CURRENT_TIMESTAMP
		set @output_msg = N'Finished at ' + CONVERT(varchar, @cur_time, 121);
		RAISERROR (@output_msg, 0, 1) WITH NOWAIT
		if (@cur_time < '2013-10-31 22:00:00.000' or @cur_time > '2013-11-01 03:00:00.000')
			begin
				-- Hvis ikke i Acadre-vinduet venter vi 1 minut...
				RAISERROR ('Waiting for 5 seconds', 0, 1) WITH NOWAIT
				WAITFOR DELAY '00:00:05';
			end
		else
			begin
				-- ... ellers venter vi 5 timer, så Acadre har fred.
				RAISERROR ('Waiting for Acadre - 5 hours delay', 0, 1) WITH NOWAIT
				WAITFOR DELAY '05:00';
			end
	end