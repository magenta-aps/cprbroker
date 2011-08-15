declare @PNR DECIMAL(10)
SET @PNR = 0703010000

select TFDTOMRK,KUNDENR,* from DTTOTAL INNER JOIN DTPERS ON DTTOTAL.PNR =DTPERS.PNR where DTTOTAL.PNR=@PNR

select * from DTABONNFEJL where PNR = @PNR order by DPRAJDTO

select * from (
select PNR, Min(CreateTS) AS CreateTS
FROM T_DPRUpdateStaging
group by PNR, DATEPART(YEAR,CreateTS) , DATEPART(Month,CreateTS) , DATEPART(DAY,CreateTS) , 
DATEPART(HOUR,CreateTS) , DATEPART(MINUTE,CreateTS), DATEPART(SECOND,CreateTS)
) as upd where PNR = @PNR order by CreateTS

select BrokerUpdateDate,* from CprBroker.dbo.PersonMapping AS PM left outer join 
CprBroker.dbo.Person P ON P.UUID = PM.UUID left outer join 
CprBroker.dbo.PersonRegistration PR ON P.UUID=PR.UUID
where CprNumber=@PNR

