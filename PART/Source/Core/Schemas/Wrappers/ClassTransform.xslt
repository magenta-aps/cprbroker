<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:d="http://itst.dk/CPRDirect"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl"
>
  <xsl:output method="text" indent="yes"/>
  <xsl:param name="namespace" />
  <xsl:param name="using" />

  <xsl:template match="/d:Layer">
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CprBroker.Schemas.Wrappers;
    <xsl:if test="$using != ''">
      using <xsl:value-of select="$using"/>;
    </xsl:if>
    namespace <xsl:value-of select="$namespace"/>
    {
    <xsl:apply-templates select="//d:Object"/>
    }
  </xsl:template>

  <!-- 
___________Attribute template _____________________________
-->
<xsl:template match="d:Attribute">
<xsl:if test="@description != '' or @danishName!=''">
<xsl:text>        ///  &lt;summary&gt;&#xa;</xsl:text>
      <xsl:if test="@danishName != ''">
        <xsl:text>        /// Danish: </xsl:text>
        <xsl:value-of select="@danishName"/>
        <xsl:text>&#xa;</xsl:text>
      </xsl:if>
      <xsl:if test="@description != ''">
        <xsl:text>        /// </xsl:text>
        <xsl:value-of select="@description"/>
        <xsl:text>&#xa;</xsl:text>
      </xsl:if>
      <xsl:text>        ///  &lt;/summary&gt;&#xa;</xsl:text>
    </xsl:if>
    <xsl:text>        public </xsl:text>
    <xsl:choose>
      <xsl:when test="@dateFormat != ''">
        <xsl:text>DateTime?</xsl:text>
      </xsl:when>
      <xsl:when test="@enumType != ''">
        <xsl:value-of select="@enumType"/>
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
        <xsl:text>return this.GetDateTime(</xsl:text><xsl:value-of select="@position"/><xsl:call-template name="lengthTemplate"/>, "<xsl:value-of select="@dateFormat"/><xsl:text>"); }</xsl:text>
      </xsl:when>
      <xsl:when test="@enumType != ''">
        <xsl:text>return this.GetEnum&lt;</xsl:text>
        <xsl:value-of select="@enumType"/>
        <xsl:text>&gt;(</xsl:text>
        <xsl:value-of select="@position"/>
        <xsl:call-template name="lengthTemplate"/>
        <xsl:text>); }</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="@format = 'Alpha'">
            <xsl:choose>
              <xsl:when test="@length = 1">
                <xsl:text>return this.GetChar(</xsl:text>
                <xsl:value-of select="@position"/>
                <xsl:text>); }</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>return this.GetString(</xsl:text>
                <xsl:value-of select="@position"/>
                <xsl:call-template name="lengthTemplate"/>
                <xsl:text>); }</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>return this.GetDecimal(</xsl:text>
            <xsl:value-of select="@position"/>
            <xsl:call-template name="lengthTemplate"/>
            <xsl:text>); }</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>&#10;</xsl:text>
    <xsl:text>            set { </xsl:text>
    <xsl:choose>
      <xsl:when test="@dateFormat != ''">
        <xsl:text>this.SetDateTime(value, </xsl:text><xsl:value-of select="@position"/><xsl:call-template name="lengthTemplate"/>, "<xsl:value-of select="@dateFormat"/><xsl:text>"); }</xsl:text>
      </xsl:when>
      <xsl:when test="@enumType != ''">
        <xsl:text>this.SetEnum&lt;</xsl:text>
        <xsl:value-of select="@enumType"/>
        <xsl:text>&gt;(value, </xsl:text>
        <xsl:value-of select="@position"/>
        <xsl:call-template name="lengthTemplate"/>
        <xsl:text>); }</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="@format = 'Alpha'">
            <xsl:choose>
              <xsl:when test="@length = 1">
                <xsl:text>this.SetChar(value, </xsl:text>
                <xsl:value-of select="@position"/>
                <xsl:text>); }</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>this.SetString(value, </xsl:text>
                <xsl:value-of select="@position"/>
                <xsl:call-template name="lengthTemplate"/>
                <xsl:text>); }</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>this.SetDecimal(value, </xsl:text>
            <xsl:value-of select="@position"/>
            <xsl:call-template name="lengthTemplate"/>
            <xsl:text>); }</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
        }
<xsl:choose>
      <xsl:when test="@dateFormat != ''">
        <xsl:text>&#xa;        public Decimal </xsl:text>
        <xsl:value-of select="@name"/><xsl:text>Decimal
        {
            get { return this.GetDecimal(</xsl:text>
        <xsl:value-of select="@position"/>
        <xsl:call-template name="lengthTemplate"/>
<xsl:text>); }
            set { this.SetDecimal(value, </xsl:text>
<xsl:value-of select="@position"/>
<xsl:call-template name="lengthTemplate"/>
<xsl:text>); }
        }

</xsl:text>
</xsl:when>
</xsl:choose>
<xsl:text></xsl:text>
</xsl:template>

  <!-- 
___________Attribute def template _____________________________
-->
  <xsl:template match="d:Attribute" mode="def" xml:space="default">
    <xsl:text>                    new Tuple&lt;string, int, int&gt;("</xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>", </xsl:text>
    <xsl:value-of select="@position"/>
    <xsl:text>, </xsl:text>
    <xsl:value-of select="@length"/>
    <xsl:text>)</xsl:text>
    <xsl:if test="position() != last()">
      <xsl:text>,</xsl:text>
    </xsl:if>
    <xsl:text>&#10;</xsl:text>
  </xsl:template>


  <!-- 
  ___________________________ Length template _____________________________
  -->
  <xsl:template name="lengthTemplate">
    <xsl:choose>
      <xsl:when test="@length != ''">
        <xsl:text>, </xsl:text>
        <xsl:value-of select="@length"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>, null</xsl:text>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>


  <!-- 
_____________________________ Object template _____________________________
-->
  <xsl:template match="//d:Object">
    public partial class <xsl:value-of select="@name"/>
    <xsl:text>Type: </xsl:text>
    <xsl:choose>
      <xsl:when test="string-length(@base) > 0">
        <xsl:value-of select="@base"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>Wrapper</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>
    {
        #region Common

        public override int Length
        {
            get { return </xsl:text>
<xsl:choose>
<xsl:when test="@length">
    <xsl:value-of select="sum(@length)"/>
</xsl:when>
<xsl:otherwise>
  <xsl:text>base.Length</xsl:text>
</xsl:otherwise>
</xsl:choose>
    
<xsl:text>; }
        }
        #endregion

        #region Properties&#xa;&#xa;</xsl:text>

<xsl:apply-templates select="d:Attribute" />
<xsl:text>
        #endregion
        public override Tuple&lt;string, int, int&gt;[] DefaultPropertyDefinitions
        {
            get 
            {
                var ret = base.DefaultPropertyDefinitions.ToList();
                ret.AddRange(new Tuple&lt;string, int, int&gt;[]{
</xsl:text>
    <xsl:apply-templates select="d:Attribute" mode="def"/>
<xsl:text>                });
                return ret.ToArray();
            }
        }
</xsl:text>
    <xsl:if test="d:Object">
<xsl:text>
        #region Sub objects&#10;
</xsl:text>
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

