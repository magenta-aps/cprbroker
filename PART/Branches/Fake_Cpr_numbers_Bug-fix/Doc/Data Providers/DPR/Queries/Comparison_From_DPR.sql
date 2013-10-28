declare @year INT; declare @month INT; declare @table varchar
SET @year=2011; SET @month=7; SET @table='DTNAVNE'


declare @fromDecimal INT; DECLARE @toDecimal INT; declare @fromDate DATETIME; declare @toDate DATETIME

SET @fromDecimal = @year*10000 + @month* 100 + 1
SET @fromDate= CAST(@fromDecimal AS VARCHAR(10))
SET @toDate = DATEADD(month,1,@fromDate)
SET @toDecimal = DATEPART(YEAR,@toDate)*10000 + DATEPART(MONTH,@toDate)* 100 + DATEPART(DAY,@toDate)

select @Year,@month,@fromDecimal,@toDecimal,@fromDate,@toDate

SELECT *, UpdatedInJune_Hist * Count AS JuneUpdateCount_Hist, UpdatedInJune_Rows * Count AS JuneUpdateCount_Rows, SubscriptionFailsInJune * Count AS JuneFailsCount FROM (
	SELECT CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows,SubscriptionFailsInJune, COUNT(*) AS Count FROM (
	SELECT  PNR, 
	 (SELECT COUNT(*) FROM DTPERSAJOURHIST WHERE PNR =T.PNR AND DPRAJDTO >= @fromDecimal AND DPRAJDTO< @toDecimal) AS UpdatedInJune_Hist,
	 (SELECT COUNT(*) FROM (SELECT DISTINCT PNR, DatePart(DAY,CreateTS) AS Day FROM T_DPRUpdateStaging WHERE PNR =T.PNR AND DPRTable='DTTOTAL' AND CreateTS>= @fromDate AND CreateTS<@toDate)as r6523 ) AS UpdatedInJune_Rows,
	 (CASE(ISNULL(TFDTOMRK,' ')) WHEN ' ' THEN (CASE(select top 1 KUNDENR FROM DTPERS WHERE PNR=T.PNR) WHEN 4432 THEN 1 ELSE 0 END) ELSE 0 END) AS HasSubscription,
	 (SELECT count(*) FROM CprBroker.dbo.PersonMapping where CprNumberDecimal=T.PNR)As CalledForUuid,
	 (SELECT COUNT(*) FROM CprBroker.dbo.PersonMapping AS P INNER JOIN CprBroker.dbo.PersonRegistration AS PR ON P.UUID=PR.UUID where CprNumberDecimal=T.PNR AND BrokerUpdateDate>=@fromDate AND BrokerUpdateDate<@toDate) AS BrokerUpdatedInJune,
	 (SELECT COUNT(*) FROM DTABONNFEJL WHERE PNR = T.PNR AND DTABONNFEJL.DPRAJDTO BETWEEN 20110601 AND 20110630) As SubscriptionFailsInJune
	FROM DTTOTAL AS T
	) AS sss
	GROUP BY CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows, SubscriptionFailsInJune
) AS ddd
WHERE CalledForUuid = 1
ORDER BY CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows,SubscriptionFailsInJune

