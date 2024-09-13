/*************************************/
/***** Create xSLMPTStatus table *****/
/*************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[xSLMPTStatus]') AND type in (N'U'))
DROP TABLE [dbo].[xSLMPTStatus]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[xSLMPTStatus](
	[COA_Errors] [int] NOT NULL,
	[COAEventLogName] [char] (512) NOT NULL,
	[COA_LValidated] [smalldatetime] NOT NULL,
	[COA_ValCmpltd] [int] NOT NULL,
	[COA_Warnings] [int] NOT NULL,
	[CpnyID] [char](10) NOT NULL,
	[Crtd_DateTime] [smalldatetime] NOT NULL,
	[Crtd_User] [char](10) NOT NULL,
	[Cust_Errors] [int] NOT NULL,
	[CustEventLogName] [char] (512) NOT NULL,
	[Cust_LValidated] [smalldatetime] NOT NULL,
	[Cust_ValCmpltd] [int] NOT NULL,
	[Cust_Warnings] [int] NOT NULL,
	[CustAddrOpt] [int] NOT NULL,
	[ExpFilePath] [char](100) NOT NULL,
	[FiscYr_Beg] [char](4) NOT NULL,
	[FiscYr_End] [char](4) NOT NULL,
	[HistoryDate] [smalldatetime] NOT NULL,
	[Inv_Errors] [int] NOT NULL,
	[InvEventLogName] [char] (512) NOT NULL,
	[Inv_LValidated] [smalldatetime] NOT NULL,
	[Inv_ValCmpltd] [int] NOT NULL,
	[Inv_Warnings] [int] NOT NULL,
	[LUpd_DateTime] [smalldatetime] NOT NULL,
	[LUpd_User] [char](10) NOT NULL,
	[Method] [char](1) NOT NULL,
	[PO_Errors] [int] NOT NULL,
	[POEventLogName] [char] (512) NOT NULL,
	[PO_LValidated] [smalldatetime] NOT NULL,
	[PO_ValCmpltd] [int] NOT NULL,
	[PO_Warnings] [int] NOT NULL,
	[POFreightChrgItem] [char](30) NOT NULL,
	[Proj_Errors] [int] NOT NULL,
	[ProjEventLogName] [char] (512) NOT NULL,
	[Proj_LValidated] [smalldatetime] NOT NULL,
	[Proj_StatusActive] [int] NOT NULL,
	[Proj_StatusInactive] [int] NOT NULL,
	[Proj_StatusPlan] [int] NOT NULL,
	[Proj_ValCmpltd] [int] NOT NULL,
	[Proj_Warnings] [int] NOT NULL,
	[S4Future01] [char](30) NOT NULL,
	[S4Future02] [char](30) NOT NULL,
	[S4Future03] [float] NOT NULL,
	[S4Future04] [float] NOT NULL,
	[S4Future05] [float] NOT NULL,
	[S4Future06] [float] NOT NULL,
	[S4Future07] [smalldatetime] NOT NULL,
	[S4Future08] [smalldatetime] NOT NULL,
	[S4Future09] [int] NOT NULL,
	[S4Future10] [int] NOT NULL,
	[S4Future11] [char](10) NOT NULL,
	[S4Future12] [char](10) NOT NULL,
	[SO_Errors] [int] NOT NULL,
	[SOEventLogName] [char] (512) NOT NULL,
	[SO_LValidated] [smalldatetime] NOT NULL,
	[SO_ValCmpltd] [int] NOT NULL,
	[SO_Warnings] [int] NOT NULL,
	[Task_StatusActive] [int] NOT NULL,
	[Task_StatusInactive] [int] NOT NULL,
	[Task_StatusPlan] [int] NOT NULL,
	[Vend_Errors] [int] NOT NULL,
	[VendEventLogName] [char] (512) NOT NULL,
	[Vend_LValidated] [smalldatetime] NOT NULL,
	[Vend_ValCmpltd] [int] NOT NULL,
	[Vend_Warnings] [int] NOT NULL,
	[VendAddrOpt] [int] NOT NULL,
	[tstamp] [timestamp] NOT NULL,
 CONSTRAINT [xSLMPTStatus0] PRIMARY KEY CLUSTERED 
(
	[CpnyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
