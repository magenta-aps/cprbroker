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
    using System.Collections.Generic;

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
    <xsl:text>    public </xsl:text>
    <xsl:choose>
      <xsl:when test="@dateFormat != ''">
        <xsl:text>DateTime?</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="@format = 'Alpha'">
            <xsl:choose>
              <xsl:when test="@length = 1">
                <xsl:text>char</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>string</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>decimal</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text> </xsl:text>
    <xsl:value-of select="@name"/>
        {
            get { <xsl:choose>
              <xsl:when test="@dateFormat != ''">
                <xsl:text>return this.GetDateTime(</xsl:text><xsl:value-of select="@position"/>, <xsl:value-of select="@length"/>, "<xsl:value-of select="@dateFormat"/><xsl:text>"); }</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:choose>
                  <xsl:when test="@format = 'Alpha'">
                    <xsl:choose>
                      <xsl:when test="@length = 1">
                        <xsl:text>return this.GetChar(</xsl:text><xsl:value-of select="@position"/><xsl:text>); }</xsl:text>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:text>return this.GetString(</xsl:text>
                        <xsl:value-of select="@position"/>
                        <xsl:text>, </xsl:text>
                        <xsl:value-of select="@length"/>
                        <xsl:text>); }</xsl:text>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>return this.GetDecimal(</xsl:text><xsl:value-of select="@position"/>, <xsl:value-of select="@length"/><xsl:text>); }</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:otherwise>
    </xsl:choose>

            set { <xsl:choose>
              <xsl:when test="@dateFormat != ''">
                <xsl:text>this.SetDateTime(value, </xsl:text><xsl:value-of select="@position"/>, <xsl:value-of select="@length"/>, "<xsl:value-of select="@dateFormat"/><xsl:text>"); }</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:choose>
                  <xsl:when test="@format = 'Alpha'">
                    <xsl:choose>
                      <xsl:when test="@length = 1">
                        <xsl:text>this.SetChar(value, </xsl:text><xsl:value-of select="@position"/><xsl:text>); }</xsl:text>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:text>this.SetString(value, </xsl:text>
                        <xsl:value-of select="@position"/>
                        <xsl:text>, </xsl:text>
                        <xsl:value-of select="@length"/>
                        <xsl:text>); }</xsl:text>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>this.SetDecimal(value, </xsl:text><xsl:value-of select="@position"/>, <xsl:value-of select="@length"/><xsl:text>); }</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:otherwise>
    </xsl:choose>
        }  </xsl:template>
  
  <!-- 
_____________________________ Object template _____________________________
-->
  <xsl:template match="//d:Object">
    public partial class <xsl:value-of select="@name"/>Type: Wrapper
    {
        #region Common
        public override int Length
        {
            get { return <xsl:value-of select="sum(@length)"/>; }
        }
        #endregion
        
        #region Properties
    <xsl:apply-templates select="d:Attribute" />
        #endregion
<xsl:if test="d:Object">
        #region Sub objects
        
</xsl:if>
<xsl:for-each select="d:Object">
  <xsl:text>        private </xsl:text>
  <xsl:if test="@maxOccurs > 1">List&lt;</xsl:if>
  <xsl:value-of select="@name"/>Type<xsl:if test="@maxOccurs > 1">&gt;</xsl:if>
  <xsl:text> _</xsl:text>
  <xsl:value-of select="@name"/>
  <xsl:text> = </xsl:text>
  <xsl:choose>
    <xsl:when test="@maxOccurs > 1">
      <xsl:text>new List&lt;</xsl:text>
      <xsl:value-of select="@name"/>
      <xsl:text>Type&gt;()</xsl:text>
    </xsl:when>
    <xsl:otherwise>
    <xsl:text>null</xsl:text>
    </xsl:otherwise>
  </xsl:choose>
  <xsl:text>;&#10;&#10;</xsl:text>

  <xsl:text>        [MinMaxOccurs(MinOccurs = </xsl:text>
  <xsl:choose>
    <xsl:when test="@minOccurs">
      <xsl:value-of select="@minOccurs"/>
    </xsl:when>
    <xsl:otherwise>1</xsl:otherwise>
  </xsl:choose>
  <xsl:text>, MaxOccurs = </xsl:text>
  <xsl:choose>
    <xsl:when test="@maxOccurs">
      <xsl:value-of select="@maxOccurs"/>
    </xsl:when>
    <xsl:otherwise>1</xsl:otherwise>
  </xsl:choose>
  <xsl:text>)]&#10;</xsl:text>
  <xsl:text>        public </xsl:text>
  <xsl:if test="@maxOccurs > 1">List&lt;</xsl:if>
  <xsl:value-of select="@name"/>Type<xsl:if test="@maxOccurs > 1">&gt;</xsl:if>
  <xsl:text> </xsl:text>
  <xsl:value-of select="@name"/>
  <xsl:text>&#10;        {&#10;</xsl:text>
  <xsl:text>            get { return _</xsl:text>
  <xsl:value-of select="@name"/>
  <xsl:text>; }&#10;</xsl:text>
  <xsl:text>            set { _</xsl:text>
  <xsl:value-of select="@name"/>
  <xsl:text> = value; }</xsl:text>  
  <xsl:text>&#10;        }&#10;&#10;</xsl:text>
</xsl:for-each>
<xsl:if test="d:Object">
        #endregion
</xsl:if>
    }

  </xsl:template>


</xsl:stylesheet>

