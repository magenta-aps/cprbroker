SELECT *, UpdatedInJune_Hist * Count AS JuneUpdateCount_Hist, UpdatedInJune_Rows * Count AS JuneUpdateCount_Rows, SubscriptionFailsInJune * Count AS JuneFailsCount FROM (
	SELECT CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows,SubscriptionFailsInJune, COUNT(*) AS Count FROM (
	SELECT  PNR, 
	 (SELECT COUNT(*) FROM DTPERSAJOURHIST WHERE PNR =T.PNR AND DPRAJDTO>=20110601 AND DPRAJDTO<=20110630 ) AS UpdatedInJune_Hist,
	 (SELECT COUNT(*) FROM (SELECT DISTINCT PNR, DatePart(DAY,CreateTS) AS Day FROM T_DPRUpdateStaging WHERE PNR =T.PNR AND DPRTable='DTPERS' AND CreateTS>='20110601' AND CreateTS<'20110701')as r6523 ) AS UpdatedInJune_Rows,
	 (CASE(ISNULL(TFDTOMRK,' ')) WHEN ' ' THEN (CASE(select top 1 KUNDENR FROM DTPERS WHERE PNR=T.PNR) WHEN 4432 THEN 1 ELSE 0 END) ELSE 0 END) AS HasSubscription,
	 (SELECT count(*) FROM CprBroker.dbo.PersonMapping where CprNumberDecimal=T.PNR)As CalledForUuid,
	 (SELECT COUNT(*) FROM CprBroker.dbo.PersonMapping AS P INNER JOIN CprBroker.dbo.PersonRegistration AS PR ON P.UUID=PR.UUID where CprNumberDecimal=T.PNR AND BrokerUpdateDate>='20110601' AND BrokerUpdateDate<'20110701') AS BrokerUpdatedInJune,
	 (SELECT COUNT(*) FROM DTABONNFEJL WHERE PNR = T.PNR AND DTABONNFEJL.DPRAJDTO BETWEEN 20110601 AND 20110630) As SubscriptionFailsInJune
	FROM DTTOTAL AS T
	) AS sss
	GROUP BY CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows, SubscriptionFailsInJune
) AS ddd
WHERE CalledForUuid = 1
ORDER BY CalledForUuid, BrokerUpdatedInJune, HasSubscription,UpdatedInJune_Hist,UpdatedInJune_Rows,SubscriptionFailsInJune