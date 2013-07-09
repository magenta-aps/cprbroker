/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('<StagingTableName>')
            and   type = 'U')
   drop table <StagingTableName>
go

if exists (select 1
            from  sysobjects
           where  id = object_id('<TablesTableName>')
            and   type = 'U')
   drop table <TablesTableName>
go
