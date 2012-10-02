<?xml version="1.0" encoding="UTF-8"?>
<!-- This xslt generates the inventory of default parts, one for each property in the model -->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
			xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

	<xsl:variable name="FieldsToShowInAnalysisWritingSystem">Name Description Abbreviation Comment Gloss Definition Bibliography</xsl:variable>

	<!-- The top level is a PartInventory node -->
	<xsl:template match="/">
		<xsl:comment>********* DO NOT MODIFY! This file is generated by PartGenerate.xslt *********</xsl:comment>
		<PartInventory>
			<xsl:apply-templates/>
		</PartInventory>
	</xsl:template>

	<!-- the top-level input node is a CellarModule containing class elements -->
	<xsl:template match="CellarModule">
		<xsl:apply-templates/>
	</xsl:template>

	<!-- Generate a bin of parts for every class  -->
	<xsl:template match="class">
		<xsl:element name="bin">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/basic">
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Detail-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">Detail</xsl:attribute>
			<xsl:element name="slice">
				<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
				<xsl:attribute name="label"><xsl:value-of select="@id"/></xsl:attribute>

				<xsl:apply-templates mode="editor" select="."/>
			</xsl:element>
		</xsl:element>
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Jt-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">JtView</xsl:attribute>

			<xsl:apply-templates mode="JtView" select="."/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/basic[@sig='MultiUnicode' or @sig='MultiString']" mode="editor">
		<xsl:attribute name="editor">string</xsl:attribute>

		<!-- hack until the UML tells us the default encoding for each string attribute -->
		<xsl:choose>
			<xsl:when test="contains($FieldsToShowInAnalysisWritingSystem, @id)">
				<xsl:attribute name="ws">analysis</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="ws">vernacular</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="props/basic[@sig='MultiUnicode' or @sig='MultiString']" mode="JtView">
		<xsl:element name="string">
			<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
			<!-- hack until the UML tells us the default encoding for each string attribute -->
			<xsl:choose>
				<xsl:when test="contains($FieldsToShowInAnalysisWritingSystem, @id)">
					<xsl:attribute name="ws">analysis</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="ws">vernacular</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/basic[@sig='Image']" mode="editor">
			<xsl:attribute name="editor">DBImage</xsl:attribute>
	</xsl:template>

	<xsl:template match="props/basic[@sig='Unicode']" mode="editor">
			<xsl:attribute name="editor">string</xsl:attribute>
	</xsl:template>

	<xsl:template match="props/basic[@sig='Unicode']" mode="JtView">
		<xsl:element name="string">
			<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/basic[@sig='Boolean']" mode="editor">
			<xsl:attribute name="editor">Checkbox</xsl:attribute>
	</xsl:template>

	<xsl:template match="props/basic[@sig='Time']" mode="JtView">
		<xsl:element name="datetime">
			<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/basic" mode="editor" priority="-1000">
			<xsl:attribute name="editor"><xsl:value-of select="@sig"/></xsl:attribute>
	</xsl:template>

	<!-- Review: would it be better to suppress creating these parts at all? -->
	<xsl:template match="props/basic" mode="JtView" priority="-1000">
		<xsl:element name="lit"> <!-- insert property name here? -->
			Unhandled property type
		</xsl:element>
	</xsl:template>

	<!-- generate either 'seq field="whatever"' or 'obj field="whatever' as appropriate for card -->
	<xsl:template match="props/owning" mode="seqOrObj">
		<xsl:choose>
			<xsl:when test="@card='atomic'">
				<xsl:element name="obj">
					<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="seq">
					<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
				</xsl:element>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- owning vectors, seq or coll -->
	<xsl:template match="props/owning">
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Detail-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">Detail</xsl:attribute>
			<xsl:apply-templates mode="seqOrObj" select="."/>
		</xsl:element>
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Jt-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">JtView</xsl:attribute>
			<xsl:apply-templates mode="seqOrObj" select="."/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="props/rel">
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Detail-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">Detail</xsl:attribute>
			<xsl:element name="slice">
				<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
				<xsl:attribute name="label"><xsl:value-of select="@id"/></xsl:attribute>
				<xsl:choose>
					<xsl:when test="@card='atomic'">
						<xsl:attribute name="editor">defaultAtomicReference</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="editor">defaultVectorReference</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:element>
		<xsl:element name="part">
			<xsl:attribute name="id"><xsl:value-of select="../../@id"/>-Jt-<xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="type">JtView</xsl:attribute>
			<xsl:choose>
				<xsl:when test="@card='atomic'">
					<xsl:element name="obj">
						<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
						<xsl:attribute name="layout">shortname</xsl:attribute>
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:element name="seq">
						<xsl:attribute name="field"><xsl:value-of select="@id"/></xsl:attribute>
						<xsl:attribute name="layout">shortname</xsl:attribute>
					</xsl:element>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:element>
	</xsl:template>

</xsl:stylesheet>
