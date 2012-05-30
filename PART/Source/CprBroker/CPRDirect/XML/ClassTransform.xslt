<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:d="http://itst.dk/CPRDirect"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl"
>
    <xsl:output method="text" indent="yes"/>
    <xsl:template match="/d:Layer">
using System;

namespace CprBroker.Providers.CPRDirect
{
      <xsl:apply-templates select="//d:Object"/>
}
    </xsl:template>

  <!-- 
___________Attribute template _____________________________
-->
  <xsl:template match="d:Attribute">
      <xsl:if test="@description != '' or @danishName!=''">
        ///  &lt;summary&gt;<xsl:if test="@danishName != ''">
          <xsl:text>&#xa;</xsl:text>        /// Danish: <xsl:value-of select="@danishName"/>
        </xsl:if>
        <xsl:if test="@description != ''">
          <xsl:text>&#xa;</xsl:text>        /// <xsl:value-of select="@description"/>
        </xsl:if>
        ///  &lt;/summary&gt;
      </xsl:if>
        <xsl:text>  public </xsl:text><xsl:choose>
          <xsl:when test="@format = 'Alpha'">string</xsl:when>
          <xsl:otherwise>decimal</xsl:otherwise>
        </xsl:choose>
        <xsl:text> </xsl:text>
        <xsl:value-of select="@name"/>
        {
            get { <xsl:choose>
              <xsl:when test="@format = 'Alpha'">
                <xsl:text>return this[</xsl:text>
                <xsl:value-of select="@position"/>
                <xsl:text>,</xsl:text>
                <xsl:value-of select="@length"/>
                <xsl:text>]; }</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>return decimal.Parse(this[</xsl:text><xsl:value-of select="@position"/>,<xsl:value-of select="@length"/><xsl:text>]); }</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
        }
  </xsl:template>

  <xsl:template match="//d:Object">
    class <xsl:value-of select="@name"/>Type: Wrapper
    {
        public override int Length
        {
            get { return <xsl:value-of select="sum(d:Attribute/@length)"/>; }
        }

        #region Sub objects
        <xsl:apply-templates select="d:Attribute" />
        #endregion

        #region Sub objects
        <xsl:for-each select="d:Object">
        public <xsl:value-of select="@name"/>Type <xsl:value-of select="@name"/>;
        </xsl:for-each>
        #endregion
    }

  </xsl:template>
  
  
</xsl:stylesheet>

