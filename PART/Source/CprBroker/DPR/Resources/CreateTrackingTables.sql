/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     23-03-2011 07:18:47                          */
/*==============================================================*/


/*==============================================================*/
/* Table: T_DPRUpdateStaging                                    */
/*==============================================================*/
if not exists (select 1
            from  sysobjects
           where  id = object_id('T_DPRUpdateStaging')
            and   type = 'U')
  create table T_DPRUpdateStaging (
   Id                   int                  identity,
   PNR                  decimal(11)          not null,
   DPRTable             varchar(120)         not null,
   CreateTS             datetime             not null,
   constraint PK_T_DPRUPDATESTAGING primary key (Id)
)
go

/*==============================================================*/
/* Table: T_DPRUpdateStaging_DPRTable                           */
/*==============================================================*/

if not exists (select 1
            from  sysobjects
           where  id = object_id('T_DPRUpdateStaging_DPRTable')
            and   type = 'U')
BEGIN
  create table T_DPRUpdateStaging_DPRTable (
   Id                   int                  identity,
   DPRTable             varchar(120)         not null,
   constraint PK_T_DPRUpdateStaging_DPRTable primary key (Id)
);
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTBESKYT')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTBOERN')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTCIV')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTFORALDREMYND')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTFORSV')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTKADR')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTNAVNE')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTPERS')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTPERSBO')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTSEPARATION')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTSTAT')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTTOTAL')
INSERT INTO T_DPRUpdateStaging_DPRTable (DPRTable) VALUES ('DTUDRIND')

END

go

