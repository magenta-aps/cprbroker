-- Create table DataProviderCall to keep track of calls made to dataproviders
create table DataProviderCall (ID uniqueidentifier primary key, ActivityID uniqueidentifier not null, Time datetime, ProviderType varchar, Cost decimal, Operation varchar, Input varchar, Success bit)

-- Add the field ActivityID to LogEntry in order to link it to rows in DataProviderCall
alter table LogEntry add ActivityID uniqueidentifier

-- Create the relation between the two tables
ALTER TABLE [dbo].[LogEntry]  WITH CHECK ADD  CONSTRAINT [FK_LogEntry_DataProviderCall] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[DataProviderCall] ([ActivityID])
