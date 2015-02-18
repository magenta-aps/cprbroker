-- 21.05.2010 - Traen A/S - JEM
-- This query returns person relation information from DPR database.
-- The query conforms to the DPR interface documented at http://assets1.csc.com/dpr/downloads/DPR_DB_FELT_BESK006.pdf
SELECT 
		RIGHT('0' + CAST(DTTOTAL.PNR AS VARCHAR(10)), 10) AS fromID,		
		REPLACE(MOR, '-', '') AS toID,
        'RST_MOM' AS relationSubTypeCode,      
        CASE KOEN WHEN 'K' THEN 'RST_DAUGHTER' ELSE 'RST_SON' END AS backwardRelationSubTypeCode
FROM    
        DTTOTAL 
WHERE 
      DTTOTAL.MOR <> '000000-0000' AND LEN(DTTOTAL.MOR) = 11
UNION
SELECT 
		RIGHT('0' + CAST(DTTOTAL.PNR AS VARCHAR(10)), 10) AS fromID,		
		REPLACE(FAR, '-', '') AS toID,
        'RST_DAD' AS relationSubTypeCode,      
        CASE KOEN WHEN 'K' THEN 'RST_DAUGHTER' ELSE 'RST_SON' END AS backwardRelationSubTypeCode
FROM    
        DTTOTAL
WHERE 
       DTTOTAL.FAR <> '000000-0000' AND LEN(DTTOTAL.FAR) = 11
UNION
SELECT  
		RIGHT('0' + CAST(DTTOTAL.PNR AS VARCHAR(10)), 10) AS fromID,		
		REPLACE(AEGTE, '-', '') AS toID,
        'RST_SPOUSE' AS relationSubTypeCode,      
        'RST_SPOUSE' AS backwardRelationSubTypeCode
FROM    
        DTTOTAL 
WHERE 
       DTTOTAL.AEGTE <> '000000-0000' AND LEN(DTTOTAL.AEGTE) = 11
ORDER BY 
        1
