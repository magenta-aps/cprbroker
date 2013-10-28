<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:d="http://itst.dk/CPRDirect"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl"
>
  <xsl:output method="text" indent="yes"/>
  <xsl:template match="/d:Layer">
    
    <xsl:apply-templates select="//d:Object/d:Object[string-length(@recordTypeCode) > 0]"/>
    
  </xsl:template>

  <!-- 
___________Attribute template _____________________________
-->
  <xsl:template match="d:Attribute">
    <xsl:text>SUBSTRING(Contents, </xsl:text>
    <xsl:value-of select="@position"/>
    <xsl:text>, </xsl:text>
    <xsl:value-of select="@length"/>
    <xsl:text>) AS </xsl:text> 
    <xsl:value-of select="@name"/>
    <xsl:text>, </xsl:text>
    </xsl:template>
  
  <!-- 
_____________________________ Object template _____________________________
-->
  <xsl:template match="//d:Object/d:Object[string-length(@recordTypeCode) > 0]">
    IF EXISTS (SELECT * FROM sys.views where name = '<xsl:value-of select="@name"/>View')
      DROP VIEW <xsl:value-of select="@name"/>View;
    GO
    
    CREATE VIEW <xsl:value-of select="@name"/>View
    AS SELECT
    e.ExtractId, e.ExtractDate, PNR, RelationPNR, RelationPNR2, <xsl:apply-templates select="d:Attribute[@name != 'PNR' and @name != 'RelationPNR' and @name != 'RelationPNR2' ]" /> Contents
    FROM ExtractItem ei
    INNER JOIN Extract e
    ON e.ExtractId = ei.ExtractId
    WHERE DataTypeCode= '<xsl:value-of select="@recordTypeCode"/>';
    
    GO
  </xsl:template>


</xsl:stylesheet>

