IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xSLAnalysisSum]') AND type in (N'U'))
DROP TABLE [dbo].[xSLAnalysisSum]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[xSLAnalysisSum](
	[AnalysisType] [char](30) NOT NULL,
	[Descr] [char](100) NOT NULL,
	[LUpd_DateTime] [smalldatetime] NOT NULL,
	[Module] [char](2) NOT NULL,
	[RecordID] [int] NOT NULL,
	[Result] [char](60) NOT NULL,
	[tstamp] [timestamp] NOT NULL,
 CONSTRAINT [xSLAnalysisSum0] PRIMARY KEY CLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/***************************************/
/***** Create xSLAnalysisCpny table *****/
/***************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xSLAnalysisCpny]') AND type in (N'U'))
DROP TABLE [dbo].[xSLAnalysisCpny]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[xSLAnalysisCpny](
	[Active] [smallint] NOT NULL,
	[Addr1] [char](30) NOT NULL,
	[Addr2] [char](30) NOT NULL,
	[BaseCuryID] [char](4) NOT NULL,
	[City] [char](30) NOT NULL,
	[Country] [char](3) NOT NULL,
	[CpnyID] [char](10) NOT NULL,
	[CpnyCOA] [char](10) NOT NULL,
	[CpnySub] [char](10) NOT NULL,
	[CpnyName] [char](30) NOT NULL,
	[DatabaseName] [char](30) NOT NULL,
	[Fax] [char](30) NOT NULL,
	[IASEmailAddress] [char](255) NOT NULL,
	[IASPubKey] [char](128) NOT NULL,
	[IASPubKeySize] [smallint] NOT NULL,
	[IASRemoteAccess] [smallint] NOT NULL,
	[InterCpnyID] [char](2) NOT NULL,
	[LocalDomain] [smallint] NOT NULL,
	[Master_Fed_ID] [char](12) NOT NULL,
	[Phone] [char](30) NOT NULL,
	[State] [char](3) NOT NULL,
	[User1] [char](30) NOT NULL,
	[User2] [char](30) NOT NULL,
	[User3] [float] NOT NULL,
	[User4] [float] NOT NULL,
	[Zip] [char](10) NOT NULL,
	[CpnyColor] [int] NOT NULL,
	[tstamp] [timestamp] NOT NULL
) ON [PRIMARY]

GO

/****************************************/
/***** Create xSLMPTSubErrors table *****/
/****************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xSLMPTSubErrors]') AND type in (N'U'))
DROP TABLE [dbo].[xSLMPTSubErrors]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[xSLMPTSubErrors](
	[ID] [char](24) NOT NULL,
	[SegNumber] [char](2) NOT NULL,
	[tstamp] [timestamp] NOT NULL,
 CONSTRAINT [xSLMPTSubErrors0] PRIMARY KEY CLUSTERED 
(
	[SegNumber] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/***************************************/
/***** Create xv_PCLDUPAPCHKS view *****/
/***************************************/
/****** Object:  View [dbo].[xv_PCLDUPAPCHKS]    Script Date: 11/04/2016 17:03:38 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[xv_PCLDUPAPCHKS]'))
DROP VIEW [dbo].[xv_PCLDUPAPCHKS]
GO

/****** Object:  View [dbo].[xv_PCLDUPAPCHKS]    Script Date: 11/04/2016 17:03:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[xv_PCLDUPAPCHKS] 
AS
	SELECT	RefNbr, Acct, Sub 
	FROM	APDoc 
	WHERE	DocType IN ('CK', 'HC', 'MC', 'SC', 'ZC', 'QC') 
	GROUP BY RefNbr, Acct, Sub
	HAVING COUNT (*) > 1	

GO

/**************************************/
/***** Create xv_PCLDUPARPAY view *****/
/**************************************/
/****** Object:  View [dbo].[xv_PCLDUPARPAY]    Script Date: 11/08/2016 11:58:58 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[xv_PCLDUPARPAY]'))
DROP VIEW [dbo].[xv_PCLDUPARPAY]
GO

/****** Object:  View [dbo].[xv_PCLDUPARPAY]    Script Date: 11/08/2016 11:58:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[xv_PCLDUPARPAY] 
AS
	SELECT	RefNbr, CustID 
	FROM	ARDoc 
	WHERE	DocType = 'PA' 
	GROUP BY RefNbr, CustID
	HAVING COUNT (*) > 1
	
GO


/*************************************************/
/***** Create xp_SLAnalysisCpnyPop procedure *****/
/*************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xp_SLAnalysisCpnyPop]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[xp_SLAnalysisCpnyPop]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[xp_SLAnalysisCpnyPop]
AS

	/* Insert records into xSLAnalysisCpny from vs_Company */
	INSERT INTO [dbo].[xSLAnalysisCpny]
		([Active],[Addr1],[Addr2],[BaseCuryID],[City]
		,[Country],[CpnyID],[CpnyCOA],[CpnySub],[CpnyName]
		,[DatabaseName],[Fax],[IASEmailAddress],[IASPubKey],[IASPubKeySize]
		,[IASRemoteAccess],[InterCpnyID],[LocalDomain],[Master_Fed_ID],[Phone]
		,[State],[User1],[User2],[User3],[User4]
		,[Zip],[CpnyColor])
	SELECT Active,Addr1,Addr2,BaseCuryID,City
		,Country,CpnyID,CpnyCOA,CpnySub,CpnyName
		,DatabaseName,Fax,IASEmailAddress,IASPubKey,IASPubKeySize
		,IASRemoteAccess,InterCpnyID,LocalDomain,Master_Fed_ID,Phone
		,State,User1,User2,User3,User4
		,Zip,CpnyColor
	FROM vs_company
GO


/***********************************************************/
/***** Create xSLMPT_AcctCurBalByType stored procedure *****/
/***********************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xSLMPT_AcctCurBalByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[xSLMPT_AcctCurBalByType]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[xSLMPT_AcctCurBalByType]
	@parm1 VARCHAR(10), @parm2 VARCHAR(4), @parm3 VARCHAR(10), @parm4 VARCHAR(1)
AS
SELECT	SUM(h.BegBal + h.PtdBal00 + h.PtdBal01 + h.PtdBal02 + h.PtdBal03 + h.PtdBal04 + h.PtdBal05 + 
		h.PtdBal06 + h.PtdBal07 + h.PtdBal08 + h.PtdBal09 + h.PtdBal10 + h.PtdBal11 + h.PtdBal12) AS [CurrBal]
FROM	AcctHist h
JOIN	Account a ON a.Acct = h.Acct
JOIN	GLSetup g ON g.SetupId = 'GL'
WHERE	h.CpnyID = @parm1
	AND h.FiscYr = @parm2
	AND h.Acct <> @parm3
	AND SUBSTRING(a.AcctType, 2, 1) = @parm4
	AND h.LedgerID = g.LedgerID
GROUP BY SUBSTRING(a.AcctType, 2, 1)

GO


