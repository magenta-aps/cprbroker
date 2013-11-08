SELECT PersonRegistrations,DTTotalCount,Count(*),SUM(SubscriptionFailureCount ) FROM (
	select UUID, CprNumberDecimal,
		(SELECT COUNT(*) FROM CprBroker.dbo.PersonRegistration PR WHERE PR.UUID = PM.UUID) As PersonRegistrations,
		(SELECT COUNT(*) FROM DTTOTAL WHERE PNR =PM.CprNumberDecimal) AS DTTotalCount,
		(SELECT COUNT(*) FROM DTABONNFEJL WHERE PNR=PM.CprNumberDecimal AND DPRAJDTO between 20110601 AND 20110630) AS SubscriptionFailureCount
	from CprBroker.dbo.PersonMapping AS PM
) AS SSS

GROUP BY PersonRegistrations, DTTotalCount
ORDER BY PersonRegistrations, DTTotalCount