<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl"
>
  <xsl:output method="text" indent="yes"/>

  <!-- ________________________ File template _________________________ -->
  <xsl:template match="/xsd:schema">
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    namespace CprBroker.Schemas.Part
    {
    <xsl:apply-templates select="xsd:element" />

    }
  </xsl:template>

  <!-- ________________________ Root elements that start the generation _________________________ -->
  <xsl:template match="xsd:element">
    <xsl:variable name="schemaTypeName" select="substring-after(@type,':')" />

    <xsl:if test="string-length($schemaTypeName) = 0">
      <xsl:variable name="TypeName" select="$schemaTypeName"/>
      <xsl:call-template name="classTemplate">
        <xsl:with-param name="className" select="@type"/>
      </xsl:call-template>
    </xsl:if>

    <xsl:if test="string-length($schemaTypeName) > 0">
      <xsl:call-template name="classTemplate">
        <xsl:with-param name="className" select="$schemaTypeName"/>
      </xsl:call-template>
    </xsl:if>

  </xsl:template>

  <!-- ________________________ Class template, calssed from root elements _________________________ -->
  <xsl:template name="classTemplate">
    <xsl:param name="className"/>
    <xsl:for-each select="//xsd:complexType[@name=$className]">
      <xsl:for-each select="xsd:complexContent">
        public class <xsl:value-of select="$className"/>
        {
        <xsl:for-each select="xsd:extension">
          <xsl:for-each select="xsd:sequence">
            <xsl:for-each select="xsd:element">
              <xsl:choose>
                <xsl:when test="@ref">
                  <xsl:call-template name="memberElementByRef">
                    <xsl:with-param name="ref" select="@ref"></xsl:with-param>
                  </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:call-template name="memberElementByNameAndType">
                    <xsl:with-param name="name" select="@name"></xsl:with-param>
                    <xsl:with-param name="type" select="@type"></xsl:with-param>
                  </xsl:call-template>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
            }
          </xsl:for-each>
        </xsl:for-each>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>

  <!-- ________________________ Type members, specified as name and type _________________________ -->
  <xsl:template name="memberElementByNameAndType">
    <xsl:param name="name"/>
    <xsl:param name="type"/>
    public <xsl:value-of select="$type"/> <xsl:text> </xsl:text> <xsl:value-of select="$name"/>;
  </xsl:template>

  <!-- ________________________ Type member, specified as reference to element _________________________ -->
  <xsl:template name="memberElementByRef">
    <xsl:param name="ref"/>
    <xsl:variable name="schemaTypeName" select="substring-after($ref,':')" />
    <xsl:if test="string-length($schemaTypeName) = 0">
      <xsl:for-each select="xsd:element[@name=$ref]">
        <xsl:call-template name="memberElementByNameAndType">
          <xsl:with-param name="name" select="@name"/>
          <xsl:with-param name="type" select="@type"/>
        </xsl:call-template>
      </xsl:for-each>
    </xsl:if>

    <xsl:if test="string-length($schemaTypeName) > 0">
      <xsl:for-each select="xsd:element[@name=$schemaTypeName]">
        <xsl:call-template name="memberElementByNameAndType">
          <xsl:with-param name="name" select="@name"/>
          <xsl:with-param name="type" select="@type"/>
        </xsl:call-template>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
