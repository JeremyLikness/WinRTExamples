<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="catalog">
    <xsl:for-each select="book">
      <xsl:value-of select="title"/> - <xsl:value-of select="price"/>
      by <xsl:value-of select="author"/>
            <xsl:value-of select="description"/>
              (published on <xsl:value-of select="publish_date"/>)            
        </xsl:for-each>      
  </xsl:template>
</xsl:stylesheet>