declare @start_date datetime, @end_date datetime, @final_date datetime, @output_msg varchar(max)
set @start_date = '2013-02-01 22:00:00.000'
set @end_date = '2013-02-01 22:15:00.000'
set @final_date = '2013-11-02 00:00:00.000'

while (@start_date < @final_date)
	begin
		set @output_msg = N'Setting null XML in tables from ' + CONVERT(varchar, @start_date, 121) + ' to ' + CONVERT(varchar, @end_date, 121);
		RAISERROR (@output_msg, 0, 1) WITH NOWAIT
		update LogEntry set DataObjectXml = null where LogDate between @start_date and @end_date
		set @start_date = DATEADD(minute, 15, @start_date)
		set @end_date = DATEADD(minute, 15, @end_date)
		WAITFOR DELAY '00:00:01';
	end