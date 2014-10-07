/****** Object:  Table [dbo].[dtadgrupper]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[dtadgrupper](
	[systemnvn] [varchar](20) NULL,
	[gruppenvn] [varchar](60) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTAJOUR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTAJOUR](
	[DPRAJDTO] [decimal](9, 0) NOT NULL,
	[ANTAL] [decimal](9, 0) NOT NULL,
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[KOMNVN] [varchar](20) NULL,
	[SOEGMRK] [varchar](1) NULL,
	[HISMDR] [decimal](3, 0) NOT NULL,
	[CICSID] [varchar](20) NULL,
	[KUNDENR] [decimal](5, 0) NULL,
	[LU62MRK] [varchar](1) NULL,
	[LUNAVN] [varchar](8) NULL,
	[DB_VERSION] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTAKTVEJ]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTAKTVEJ](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[VEJADNVN] [varchar](20) NULL,
	[SVEJADRNVN] [char](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBEFOLKDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBEFOLKDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[BEFOLKKOD] [char](4) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBESKYT]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBESKYT](
	[PNR] [decimal](11, 0) NOT NULL,
	[BESKYT_TYP] [decimal](4, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[STARTDATE] [smalldatetime] NOT NULL,
	[SLETDATE] [datetime] NULL,
	[INDRAP] [varchar](3) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBOERN]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBOERN](
	[PNR] [decimal](11, 0) NOT NULL,
	[PNRBARN] [decimal](11, 0) NULL,
	[DOK] [varchar](3) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBOLIG]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBOLIG](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNR] [char](4) NULL,
	[ETAGE] [char](2) NULL,
	[SIDEDOER] [char](4) NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[BOLIGTYPKOD] [char](1) NOT NULL,
	[HAENSTART] [decimal](13, 0) NOT NULL,
	[SLETDTO] [decimal](13, 0) NULL,
	[LOKALITET] [varchar](34) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBYFORNYDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBYFORNYDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[BYFORNYKOD] [char](6) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTBYNAVN]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTBYNAVN](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[BYNVN] [varchar](34) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTCIV]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTCIV](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[CIVST] [varchar](1) NULL,
	[MYNKOD] [decimal](5, 0) NULL,
	[AEGTEPNR] [decimal](11, 0) NULL,
	[AEFODDTO] [decimal](9, 0) NULL,
	[AEGTEDOK] [varchar](3) NULL,
	[HAENST] [decimal](13, 0) NULL,
	[HAENSLUT] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL,
	[MYTAJDTO] [decimal](13, 0) NULL,
	[MYNTXT] [varchar](20) NULL,
	[AEGTENVN] [varchar](34) NULL,
	[SEP_HENVIS_TS] [varchar](26) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[dtddelibra]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dtddelibra](
	[PNR] [decimal](11, 0) NOT NULL,
	[DPRAJDTO] [decimal](8, 0) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DTDIVDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTDIVDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[DISTTYP] [decimal](2, 0) NOT NULL,
	[DIVDISTKOD] [char](4) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTEVAKUERDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTEVAKUERDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[EVAKUERKOD] [decimal](1, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTFORALDREMYND]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTFORALDREMYND](
	[PNR] [decimal](11, 0) NOT NULL,
	[RELTYP] [decimal](4, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[START_MYNKOD] [decimal](5, 0) NOT NULL,
	[STARTDATE] [datetime] NOT NULL,
	[STARTDATE_UMRK] [char](1) NULL,
	[SLETDATE] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTFORSV]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTFORSV](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[FORSVDTO] [decimal](13, 0) NOT NULL,
	[GENFDTO] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTHAEN]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTHAEN](
	[PNR] [decimal](11, 0) NOT NULL,
	[TSU] [decimal](13, 0) NOT NULL,
	[HAEND] [varchar](3) NULL,
	[AFLMRK] [varchar](2) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTKADR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTKADR](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[HAENST] [decimal](13, 0) NOT NULL,
	[KADR1] [varchar](34) NULL,
	[KADR2] [varchar](34) NULL,
	[KADR3] [varchar](34) NULL,
	[KADR4] [varchar](34) NULL,
	[KADR5] [varchar](34) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTKIRKEDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTKIRKEDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[KIRKEKOD] [decimal](2, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTKOMFOR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTKOMFOR](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[KOMFTYP] [decimal](1, 0) NOT NULL,
	[KOMFKOD] [varchar](5) NULL,
	[HAENST] [decimal](13, 0) NOT NULL,
	[BEMAERK] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTMYN]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTMYN](
	[MYNKOD] [decimal](5, 0) NOT NULL,
	[MYNTYP] [decimal](3, 0) NOT NULL,
	[MYNGRP] [decimal](1, 0) NOT NULL,
	[MYNNVN] [varchar](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTNAVNE]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTNAVNE](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[MYNKOD] [decimal](5, 0) NULL,
	[STATUS] [decimal](3, 0) NULL,
	[STHAENST] [decimal](13, 0) NULL,
	[FORNMRK] [varchar](1) NULL,
	[EFTNMRK] [varchar](1) NULL,
	[NVHAENST] [decimal](13, 0) NULL,
	[NVHAENSL] [decimal](13, 0) NULL,
	[ANHAENST] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL,
	[INDRAP] [varchar](3) NULL,
	[MYTAJDTO] [decimal](13, 0) NULL,
	[SNHAENST] [decimal](13, 0) NULL,
	[FORNVN] [varchar](50) NULL,
	[EFTERNVN] [varchar](40) NULL,
	[ADRNVN] [varchar](34) NULL,
	[SOEGNVN] [varchar](34) NULL,
	[MYNTXT] [varchar](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTNOTAT]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTNOTAT](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[HAENST] [decimal](13, 0) NOT NULL,
	[SLETDTO] [decimal](13, 0) NULL,
	[NOTATNR] [decimal](3, 0) NOT NULL,
	[NOTATLI] [varchar](40) NULL,
	[KOMKOD] [decimal](5, 0) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTPERS]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTPERS](
	[PNR] [decimal](11, 0) NOT NULL,
	[PERAJDTO] [decimal](13, 0) NOT NULL,
	[FOEDDTO] [decimal](9, 0) NOT NULL,
	[KOEN] [varchar](1) NULL,
	[FODMYKOD] [decimal](5, 0) NOT NULL,
	[FMHAENST] [decimal](13, 0) NOT NULL,
	[FMAJFDTO] [decimal](13, 0) NOT NULL,
	[FKIRK] [varchar](1) NULL,
	[FKAJFDTO] [decimal](13, 0) NOT NULL,
	[FKMYNKOD] [decimal](5, 0) NOT NULL,
	[FKHAENST] [decimal](13, 0) NOT NULL,
	[UMYMYKOD] [decimal](5, 0) NULL,
	[UMAJFDTO] [decimal](13, 0) NULL,
	[UMHAENST] [decimal](13, 0) NULL,
	[PMHAENST] [decimal](13, 0) NULL,
	[PNHAENST] [decimal](13, 0) NOT NULL,
	[PGAJFDTO] [decimal](13, 0) NULL,
	[PNRGAELD] [decimal](11, 0) NULL,
	[PNHAENSL] [decimal](13, 0) NULL,
	[STILDTO] [decimal](13, 0) NULL,
	[FTAJFDTO] [decimal](13, 0) NULL,
	[KUNDENR] [decimal](5, 0) NULL,
	[MFAJFDTO] [decimal](13, 0) NOT NULL,
	[PNRMOR] [decimal](11, 0) NOT NULL,
	[MORFDTO] [decimal](9, 0) NULL,
	[MORDOK] [varchar](3) NULL,
	[PNRFAR] [decimal](11, 0) NOT NULL,
	[FARFDTO] [decimal](9, 0) NULL,
	[FARDOK] [varchar](3) NULL,
	[FSHAENST] [decimal](13, 0) NULL,
	[FSMYNKOD] [decimal](5, 0) NULL,
	[MORNVN] [varchar](34) NULL,
	[FARNVN] [varchar](34) NULL,
	[FMTXT] [varchar](20) NULL,
	[STILLING] [varchar](34) NULL,
	[UMYNSLETDATE] [smalldatetime] NULL,
	[UMYNRELTYP] [decimal](4, 0) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTPERSAJOUR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTPERSAJOUR](
	[PNR] [decimal](11, 0) NOT NULL,
	[DPRAJDTO] [decimal](9, 0) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DTPERSAJOURHIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTPERSAJOURHIST](
	[PNR] [decimal](11, 0) NOT NULL,
	[DPRAJDTO] [decimal](9, 0) NULL,
	[LEVAJDTO] [decimal](9, 0) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DTPERSBO]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTPERSBO](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNR] [varchar](4) NULL,
	[ETAGE] [varchar](2) NULL,
	[SIDEDOER] [varchar](4) NULL,
	[BNR] [varchar](4) NULL,
	[POSTNR] [decimal](5, 0) NOT NULL,
	[KOMNVN] [varchar](20) NULL,
	[VEJADNVN] [varchar](20) NULL,
	[TILFDTO] [decimal](13, 0) NOT NULL,
	[TFDTOMRK] [varchar](1) NULL,
	[FRAFDTO] [decimal](13, 0) NULL,
	[FRAFKKOD] [decimal](5, 0) NULL,
	[FRAFKDTO] [decimal](13, 0) NULL,
	[TILFKDTO] [decimal](13, 0) NULL,
	[ABHAENST] [decimal](13, 0) NULL,
	[ABSLEDTO] [decimal](13, 0) NULL,
	[LBHAENST] [decimal](13, 0) NULL,
	[SAHAENST] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL,
	[CONVN] [varchar](34) NULL,
	[BYNVN] [varchar](34) NULL,
	[LOKAL] [varchar](34) NULL,
	[SUPLADR1] [varchar](34) NULL,
	[SUPLADR2] [varchar](34) NULL,
	[SUPLADR3] [varchar](34) NULL,
	[LBSLEDTO] [decimal](13, 0) NULL,
	[LBINDRAP] [varchar](3) NULL,
	[SUPLADR4] [varchar](34) NULL,
	[SUPLADR5] [varchar](34) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTPOSTDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTPOSTDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[POSTNR] [decimal](4, 0) NOT NULL,
	[DISTTXT] [varchar](30) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTPOSTNR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTPOSTNR](
	[POSTNR] [decimal](5, 0) NOT NULL,
	[POSTTXT] [varchar](20) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTRELPNR_PNR]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTRELPNR_PNR](
	[PNR] [decimal](11, 0) NOT NULL,
	[RELPNR] [decimal](11, 0) NOT NULL,
	[RELTYP] [decimal](4, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[STARTDATE] [smalldatetime] NOT NULL,
	[SLETDATE] [smalldatetime] NULL,
	[MYNKOD] [decimal](5, 0) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DTRELPNR_TXT]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTRELPNR_TXT](
	[PNR] [decimal](11, 0) NOT NULL,
	[RELADRSAT] [varchar](34) NULL,
	[RELTYP] [decimal](4, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[RELTXT1] [varchar](34) NULL,
	[RELTXT2] [varchar](34) NULL,
	[RELTXT3] [varchar](34) NULL,
	[RELTXT4] [varchar](34) NULL,
	[RELTXT5] [varchar](34) NULL,
	[STARTDATE] [smalldatetime] NOT NULL,
	[SLETDATE] [smalldatetime] NULL,
	[MYNKOD] [decimal](5, 0) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSEPARATION]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSEPARATION](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[SEP_HENVIS_TS] [varchar](26) NOT NULL,
	[ANNKOR] [char](1) NULL,
	[START_MYNKOD] [decimal](5, 0) NOT NULL,
	[STARTDATE] [datetime] NOT NULL,
	[STARTDATE_UMRK] [char](1) NULL,
	[SLUT_MYNKOD] [decimal](5, 0) NULL,
	[SLUTDATE] [datetime] NULL,
	[SLUTDATE_UMRK] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSKOLEDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSKOLEDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[SKOLEKOD] [decimal](2, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSOCIALDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSOCIALDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[SOCIALKOD] [decimal](2, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSOGNEDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSOGNEDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[MYNKOD] [decimal](4, 0) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSPKOM]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSPKOM](
	[KOMTYPE] [varchar](3) NULL,
	[NETNODE] [varchar](21) NULL,
	[SERVERID] [varchar](9) NULL,
	[INSTANCEID] [varchar](3) NULL,
	[LOEBENR] [varchar](6) NULL,
	[TLOC] [smalldatetime] NULL,
	[ORDRE] [varchar](255) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSTAT]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSTAT](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[MYNKOD] [decimal](5, 0) NOT NULL,
	[HAENST] [decimal](13, 0) NOT NULL,
	[HAENSLUT] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSTKOM]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSTKOM](
	[KOMTYPE] [varchar](3) NULL,
	[NETNODE] [varchar](21) NULL,
	[SERVERID] [varchar](9) NULL,
	[INSTANCEID] [varchar](3) NULL,
	[LOEBENR] [varchar](6) NULL,
	[TLOC] [smalldatetime] NULL,
	[ORDRE] [varchar](255) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSVKOM]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSVKOM](
	[KOMTYPE] [varchar](3) NULL,
	[NETNODE] [varchar](21) NULL,
	[SERVERID] [varchar](9) NULL,
	[INSTANCEID] [varchar](3) NULL,
	[LOEBENR] [varchar](6) NULL,
	[TLOC] [smalldatetime] NULL,
	[ORDRE] [varchar](255) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSYBRUGER]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSYBRUGER](
	[BRUGERID] [varchar](10) NOT NULL,
	[BRUGERNVN] [varchar](30) NULL,
	[PASSWORD] [varchar](8) NULL,
	[AFDELING] [varchar](20) NULL,
	[INSTITUTION] [varchar](20) NULL,
	[ADGADMKOD] [varchar](1) NULL,
	[PWAENDDATE] [smalldatetime] NULL,
	[PWGL1] [char](8) NULL,
	[PWGL2] [char](8) NULL,
	[PWGL3] [char](8) NULL,
	[PWGL4] [char](8) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSYDPRLOG]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSYDPRLOG](
	[BRUGERID] [varchar](20) NULL,
	[LOGDTO] [varchar](8) NOT NULL,
	[LOGTID] [varchar](5) NOT NULL,
	[APPLID] [varchar](8) NULL,
	[FUNKTION] [varchar](1) NULL,
	[PNR] [decimal](10, 0) NULL,
	[SOEGKRIT] [varchar](255) NULL,
	[PCID] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSYKLSTAT]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSYKLSTAT](
	[AAR] [decimal](4, 0) NOT NULL,
	[KVARTAL] [decimal](1, 0) NOT NULL,
	[BRUGERID] [varchar](10) NOT NULL,
	[APPLID] [varchar](8) NULL,
	[APPLANT] [decimal](6, 0) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTSYTXTAPPL]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTSYTXTAPPL](
	[APPLID] [varchar](8) NOT NULL,
	[APPLTXT] [varchar](60) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTTOTAL]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTTOTAL](
	[PNR] [decimal](11, 0) NOT NULL,
	[STHAENST] [decimal](13, 0) NULL,
	[STATUS] [decimal](3, 0) NOT NULL,
	[FOEDDTO] [decimal](9, 0) NOT NULL,
	[KOEN] [varchar](1) NULL,
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[AKTKONVN] [varchar](20) NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNR] [varchar](4) NULL,
	[ETAGE] [varchar](2) NULL,
	[SIDEDOER] [varchar](4) NULL,
	[BNR] [varchar](4) NULL,
	[ABMRK] [varchar](1) NULL,
	[LBMRK] [varchar](1) NULL,
	[TFDTOMRK] [varchar](1) NULL,
	[TILFDTO] [decimal](13, 0) NOT NULL,
	[TILFKDTO] [decimal](13, 0) NULL,
	[FRAFKDTO] [decimal](13, 0) NULL,
	[FKIRK] [varchar](1) NULL,
	[FREGSTED] [varchar](20) NULL,
	[PMHAENST] [decimal](13, 0) NULL,
	[MOR] [varchar](11) NULL,
	[MORMRK] [varchar](1) NULL,
	[FAR] [varchar](11) NULL,
	[FARMRK] [varchar](1) NULL,
	[UDINDMRK] [varchar](1) NULL,
	[FORSVMRK] [varchar](1) NULL,
	[UMHAENST] [decimal](13, 0) NULL,
	[FSHAENST] [decimal](13, 0) NULL,
	[CIVST] [varchar](1) NULL,
	[CIHAENST] [decimal](13, 0) NULL,
	[AEGTE] [varchar](11) NULL,
	[AEGTEMRK] [varchar](1) NULL,
	[POSTNR] [decimal](5, 0) NOT NULL,
	[POSTTXT] [varchar](20) NULL,
	[VALGRDTO] [decimal](13, 0) NULL,
	[BARNMRK] [varchar](1) NULL,
	[SUPLMRK] [varchar](1) NULL,
	[KOMFMRK] [varchar](1) NULL,
	[NOTATMRK] [varchar](1) NULL,
	[TPNRMRK] [varchar](1) NULL,
	[FSMYNNVN] [varchar](20) NULL,
	[CIVMYN] [varchar](20) NULL,
	[FMTXT] [varchar](20) NULL,
	[STILLING] [varchar](34) NULL,
	[CONVN] [varchar](34) NULL,
	[BYNVN] [varchar](34) NULL,
	[STBORGER] [varchar](20) NULL,
	[TIDLADR] [varchar](65) NULL,
	[TIDLKNVN] [varchar](20) NULL,
	[FORNVN] [varchar](50) NULL,
	[EFTERNVN] [varchar](40) NULL,
	[ADRNVN] [varchar](34) NULL,
	[STANDADR] [varchar](34) NULL,
	[LOKAL] [varchar](34) NULL,
	[KADRMRK] [varchar](1) NULL,
	[INDLAESDTO] [datetime] NULL,
	[INDLAESPGM] [char](1) NULL,
	[HENTTYP] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTUDRIND]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTUDRIND](
	[PNR] [decimal](11, 0) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[UDRMYKOD] [decimal](5, 0) NULL,
	[UDRDTO] [decimal](13, 0) NULL,
	[UDRAJDTO] [decimal](13, 0) NULL,
	[ULADRDTO] [decimal](13, 0) NULL,
	[VALGRDTO] [decimal](13, 0) NULL,
	[INDMYKOD] [decimal](5, 0) NULL,
	[INDRDTO] [decimal](13, 0) NULL,
	[INDAJDTO] [decimal](13, 0) NULL,
	[ANNKOR] [varchar](1) NULL,
	[UADR1] [varchar](34) NULL,
	[UADR2] [varchar](34) NULL,
	[UADR3] [varchar](34) NULL,
	[UADR4] [varchar](34) NULL,
	[UADR5] [varchar](34) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTVALGDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTVALGDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[VALGKOD] [decimal](2, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DTVARMEDIST]    Script Date: 29-09-2014 10:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DTVARMEDIST](
	[KOMKOD] [decimal](5, 0) NOT NULL,
	[VEJKOD] [decimal](5, 0) NOT NULL,
	[HUSNRFRA] [char](4) NOT NULL,
	[HUSNRTIL] [char](4) NOT NULL,
	[LIGEULIGE] [char](1) NOT NULL,
	[AJFDTO] [decimal](13, 0) NOT NULL,
	[VARMEKOD] [decimal](4, 0) NOT NULL,
	[DISTTXT] [varchar](30) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
