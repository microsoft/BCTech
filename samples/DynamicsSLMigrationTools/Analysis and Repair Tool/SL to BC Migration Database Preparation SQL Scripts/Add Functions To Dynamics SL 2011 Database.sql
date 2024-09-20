----------------------------------START FUNCTION NameFlip-----------------------------------
if exists(select * from sys.objects where [object_id] = object_id(N'dbo.NameFlip') and [type] = N'FN')
    DROP FUNCTION NameFlip
GO

CREATE FUNCTION NameFlip
(
	@Full Varchar(50)
)
RETURNS Varchar(50)
AS
BEGIN
DECLARE @Fixed Varchar(50)
SELECT @Fixed = CASE WHEN Ltrim(Rtrim(@Full)) LIKE '%~%' THEN substring(Ltrim(Rtrim(@Full)) , charindex('~' , Ltrim(Rtrim(@Full))) + 1 , LEN(Ltrim(Rtrim(@Full))) - charindex('~' , Ltrim(Rtrim(@Full)))) + ' ' + substring(Ltrim(Rtrim(@Full)) , 1 , charindex('~' , Ltrim(Rtrim(@Full))) - 1) ELSE Ltrim(Rtrim(@Full)) END
	RETURN (@Fixed)
END
GO
----------------------------------FINISH FUNCTION NameFlip----------------------------------

GRANT EXECUTE on NameFlip to MSDSL
GO
GRANT EXECUTE ON NameFlip TO E8F575915A2E4897A517779C0DD7CE
GO


----------------------------------BEGIN FUNCTION GetCalendarEndDateOfGLPeriod------------------------------------------------

If EXISTS (Select * From sys.objects Where [object_id] = OBJECT_ID(N'dbo.GetCalendarEndDateOfGLPeriod') and [type] in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    Drop Function dbo.GetCalendarEndDateOfGLPeriod
Go

Create Function GetCalendarEndDateOfGLPeriod (@period char(6))
Returns smalldatetime
As
Begin
  Declare @periodList table (period smallint, md char(4), y char(4))
  Declare @i smallint, @nbrPer smallint, @begFiscalYr smallint, @year smallint, @febPeriod smallint
  Declare @leapYear bit

  -- Create record for each possible period in a memory table
  Select @i = 1, @nbrper = nbrper, @begFiscalYr = BegFiscalYr
    From GLSetup
  While @i <= @nbrPer
  Begin
    Insert Into @periodList
      Select @i period,
             Case @i When  1 Then FiscalPerEnd00  When  2 Then FiscalPerEnd01  When  3 Then FiscalPerEnd02
                     When  4 Then FiscalPerEnd03  When  5 Then FiscalPerEnd04  When  6 Then FiscalPerEnd05
                     When  7 Then FiscalPerEnd06  When  8 Then FiscalPerEnd07  When  9 Then FiscalPerEnd08
                     When 10 Then FiscalPerEnd09  When 11 Then FiscalPerEnd10  When 12 Then FiscalPerEnd11
                     Else FiscalPerEnd12 End md,
             SPACE(0) y
        From GLSetup
    Select @i = @i + 1
  End

  -- Set the calendar year (y) that belongs to the month/day (md)
  -- using the fiscal period with month/day (md) that is the latest month in the calendar year
  Set @year = CONVERT(smallint, LEFT(@period, 4))
  Update @periodList Set y = CASE WHEN period <= (Select period From @periodList Where md = (Select MAX(md) From @periodList))
                                    THEN CONVERT(char(4), CASE WHEN @begFiscalYr = 1 THEN @year ELSE @year - 1 END)
                                  ELSE CONVERT(char(4), CASE WHEN @begFiscalYr = 1 THEN @year + 1 ELSE @year END) END

  -- Deal with the potential leap year days
  -- If month/day (md) is 0228 and it's leap year, add a day
  Set @febPeriod = NULL
  Select @febPeriod = period
    From @periodList
    Where md = '0228' AND CASE WHEN y % 4 = 0 AND (y % 100 <> 0 OR y % 400 = 0) THEN 'True' ELSE 'False' END = 'True'
  If NOT @febPeriod IS NULL
    Update @periodList Set md = '0229' Where period = @febPeriod

  -- If month/day (md) is 0229 and it's NOT a leap year, subtract a day
  Set @febPeriod = NULL
  Select @febPeriod = period
    From @periodList
    Where md = '0229' AND  CASE WHEN y % 4 = 0 AND (y % 100 <> 0 OR y % 400 = 0) THEN 'True' ELSE 'False' END = 'False'
  If NOT @febPeriod IS NULL
    Update @periodList Set md = '0228' Where period = @febPeriod

  Return (Select CONVERT(smalldatetime, CAST(y As char(4)) + md) periodEndDate
            From @periodList
            Where period = CAST(RIGHT(@period, 2) As smallint))
End

Go
---------------------------------FINISH FUNCTION GetCalendarEndDateOfGLPeriod------------------------------------------------

GRANT EXECUTE on  GetCalendarEndDateOfGLPeriod to MSDSL
GO
GRANT EXECUTE ON  GetCalendarEndDateOfGLPeriod TO E8F575915A2E4897A517779C0DD7CE
GO


----------------------------------BEGIN FUNCTION GetCalendarBegDateOfGLPeriod------------------------------------------------

IF EXISTS (SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[GetCalendarBegDateOfGLPeriod]') AND [type] in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[GetCalendarBegDateOfGLPeriod]
GO

CREATE FUNCTION [dbo].[GetCalendarBegDateOfGLPeriod] (@Parm1 char(6))
RETURNS smalldatetime
AS
BEGIN
	DECLARE @CurYear INTEGER, @PrevYear INTEGER
	DECLARE @CurMonthStr CHAR(2), @PrevMonthStr CHAR(2)
	DECLARE @PrevMonthVal INTEGER
	DECLARE @FiscPerStr CHAR(6)
	DECLARE @FPDateEnd DATETIME
	DECLARE @NumPeriods INTEGER

	SELECT @CurYear = SUBSTRING(@Parm1, 1, 4)
	SELECT @PrevYear = @CurYear - 1
	SELECT @CurMonthStr = SUBSTRING(@Parm1, 5, 2)

	IF @CurMonthStr = '01'
	BEGIN
		SELECT @NumPeriods = NbrPer FROM dbo.GLSetup
		IF LEN(@NumPeriods) = 1 SELECT @PrevMonthStr = '0' + STR(@NumPeriods, 1) ELSE SELECT @PrevMonthStr = STR(@NumPeriods, 2)
		SELECT @FiscPerStr = STR(@PrevYear, 4) + @PrevMonthStr
	END
	ELSE
	BEGIN
		SELECT @PrevMonthVal = @CurMonthStr - 1
		IF LEN(@PrevMonthVal) = 1 SELECT @PrevMonthStr = '0' + STR(@PrevMonthVal, 1) ELSE SELECT @PrevMonthStr = STR(@PrevMonthVal, 2)
		SELECT @FiscPerStr = STR(@CurYear, 4) + @PrevMonthStr
	END

	SELECT @FPDateEnd = (SELECT dbo.GetCalendarEndDateOfGLPeriod(@FiscPerStr))
	SELECT @FPDateEnd = DATEADD(day, 1, @FPDateEnd)

	RETURN @FPDateEnd

END
GO

----------------------------------FINISH FUNCTION GetCalendarBegDateOfGLPeriod------------------------------------------------
GRANT EXECUTE on  GetCalendarBegDateOfGLPeriod to MSDSL
GO
GRANT EXECUTE ON  GetCalendarBegDateOfGLPeriod TO E8F575915A2E4897A517779C0DD7CE
GO

