DECLARE @fromYear INT ,@fromMonth INT,@toYear INT,@toMonth INT, @dayOffset INT

SET @fromYear=2011; set @fromMonth=1
SET @toYear =2011; set @toMonth=8
SET @dayOffset=1

DECLARE @fromDate DATETIME, @toDate DATETIME, @fromDecimal INT,@toDecimal INT

DECLARE @Base Table(Year INT, Month INT,BrokerUpdates INT, DPRUpdates INT,DPRSubscriptionFailures INT)

SET @fromDecimal = @fromYear*10000 + @fromMonth* 100 + 1
SET @fromDate = CAST(@fromDecimal AS VARCHAR(10))
DECLARE @tmpDate DATETIME = @fromDate

WHILE DATEPART(YEAR, @tmpDate) <= @toYear AND DATEPART(MONTH, @tmpDate) <= @toMonth
BEGIN
	SET @fromDate = @tmpDate
	SET @toDate = DATEADD(MONTH,1,@tmpDate)
	
	SET @fromDate = DATEADD(DAY,@dayOffset,@fromDate)
	SET @toDate = DATEADD(DAY,@dayOffset,@toDate)
	
	SET @fromDecimal = DATEPART(YEAR,@fromDate)*10000 + DATEPART(MONTH,@fromDate)* 100 + DATEPART(DAY,@fromDate)
	SET @toDecimal = DATEPART(YEAR,@toDate)*10000 + DATEPART(MONTH,@toDate)* 100 + DATEPART(DAY,@toDate)

	INSERT INTO @Base VALUES (
		DATEPART(YEAR,@tmpDate), 
		DATEPART(MONTH,@tmpDate),
		(SELECT COUNT(*) FROM CprBroker.dbo.PersonRegistration WHERE BrokerUpdateDate >= @fromDate AND BrokerUpdateDate < @toDate),
		(SELECT COUNT(*) FROM DTPERSAJOURHIST WHERE DPRAJDTO >= @fromDecimal AND DPRAJDTO < @toDecimal),
		(SELECT COUNT(*) FROM DTABONNFEJL WHERE DPRAJDTO >= @fromDecimal AND DPRAJDTO < @toDecimal)
	)
	SET @tmpDate = DATEADD(MONTH,1,@tmpDate)
END


SELECT *, DPRUpdates + DPRSubscriptionFailures AS CprDirectEst
FROM @Base
	