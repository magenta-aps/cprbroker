/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('<StagingTableName>')
            and   type = 'U')
   drop table T_<SystemName>UpdateStaging
go

if exists (select 1
            from  sysobjects
           where  id = object_id('<TablesTableName>')
            and   type = 'U')
   drop table <TablesTableName>
go

/*==============================================================*/
/* Table: T_<SystemName>UpdateStaging                                    */
/*==============================================================*/
create table <StagingTableName> (
   <IdColumnName>                   int                  identity,
   <PnrColumnName>                  decimal(11)          not null,
   <TableColumnName>             varchar(120)         not null,
   CreateTS             datetime             not null,
   constraint PK_T_<SystemName>UPDATESTAGING primary key (<IdColumnName>)
)
go

/*==============================================================*/
/* Table: <TablesTableName>                           */
/*==============================================================*/
create table <TablesTableName> (
   <IdColumnName>                   int                  identity,
   <TableColumnName>             varchar(120)         not null,
   constraint PK_<TablesTableName> primary key (Id)
)
go

<InsertTableNames>

GO
