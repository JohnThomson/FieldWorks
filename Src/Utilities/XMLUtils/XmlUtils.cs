// Copyright (c) 2003-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)
//
// File: XmlUtils.cs
// Responsibility: Andy Black
// Last reviewed:
//
// <remarks>
// This makes available some utilities for handling XML Nodes
// </remarks>
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace SIL.Utils
{
	/// <summary>
	/// Summary description for XmlUtils.
	/// </summary>
	public class XmlUtils
	{
		/// <summary>
		/// Returns true if value of attrName is 'true' or 'yes' (case ignored)
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The optional attribute to find.</param>
		/// <returns></returns>
		public static bool GetBooleanAttributeValue(XmlNode node, string attrName)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(node, attrName));
		}

		/// <summary>
		/// Returns true if value of attrName is 'true' or 'yes' (case ignored)
		/// </summary>
		/// <param name="node">The XElement to look in.</param>
		/// <param name="attrName">The optional attribute to find.</param>
		/// <returns></returns>
		public static bool GetBooleanAttributeValue(XElement node, string attrName)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(node, attrName));
		}

		/// <summary>
		/// Returns true if sValue is 'true' or 'yes' (case ignored)
		/// </summary>
		public static bool GetBooleanAttributeValue(string sValue)
		{
			return (sValue != null
				&& (sValue.ToLowerInvariant().Equals("true")
				|| sValue.ToLowerInvariant().Equals("yes")));
		}

		/// <summary>
		/// Returns a integer obtained from the (mandatory) attribute named.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The mandatory attribute to find.</param>
		/// <returns>The value, or 0 if attr is missing.</returns>
		public static int GetMandatoryIntegerAttributeValue(XmlNode node, string attrName)
		{
			return Int32.Parse(GetManditoryAttributeValue(node, attrName), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Returns a integer obtained from the (mandatory) attribute named.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The mandatory attribute to find.</param>
		/// <returns>The value, or 0 if attr is missing.</returns>
		public static int GetMandatoryIntegerAttributeValue(XElement node, string attrName)
		{
			return Int32.Parse(GetManditoryAttributeValue(node, attrName), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Return an optional integer attribute value, or if not found, the default value.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attrName"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		public static int GetOptionalIntegerValue(XmlNode node, string attrName, int defaultVal)
		{
			string val = GetOptionalAttributeValue(node, attrName);
			if (string.IsNullOrEmpty(val))
				return defaultVal;
			return Int32.Parse(val, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Return an optional integer attribute value, or if not found, the default value.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attrName"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		public static int GetOptionalIntegerValue(XElement node, string attrName, int defaultVal)
		{
			string val = GetOptionalAttributeValue(node, attrName);
			if (string.IsNullOrEmpty(val))
				return defaultVal;
			return Int32.Parse(val, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Retrieve an array, given an attribute consisting of a comma-separated list of integers
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		public static int[] GetMandatoryIntegerListAttributeValue(XElement node, string attrName)
		{
			string input = GetManditoryAttributeValue(node, attrName);
			string[] vals = input.Split(',');
			var result = new int[vals.Length];
			for (int i = 0; i < vals.Length; i++)
				result[i] = Int32.Parse(vals[i], CultureInfo.InvariantCulture);
			return result;
		}

		/// <summary>
		/// Make a value suitable for GetMandatoryIntegerListAttributeValue to parse.
		/// </summary>
		/// <param name="vals"></param>
		/// <returns></returns>
		public static string MakeIntegerListValue(int[] vals)
		{
			var builder = new StringBuilder(vals.Length * 7); // enough unless VERY big numbers
			for (int i = 0; i < vals.Length; i++)
			{
				if (i != 0)
					builder.Append(",");
				builder.Append(vals[i].ToString(CultureInfo.InvariantCulture));
			}
			return builder.ToString();
		}

		/// <summary>
		/// Make a comma-separated list of the ToStrings of the values in the list.
		/// </summary>
		/// <param name="vals"></param>
		/// <returns></returns>
		public static string MakeListValue(List<int> vals)
		{
			var builder = new StringBuilder(vals.Count * 7); // enough unless VERY big numbers
			for (int i = 0; i < vals.Count; i++)
			{
				if (i != 0)
					builder.Append(",");
				builder.Append(vals[i].ToString(CultureInfo.InvariantCulture));
			}
			return builder.ToString();
		}

		/// <summary>
		/// Make a comma-separated list of the ToStrings of the values in the list.
		/// </summary>
		/// <param name="vals"></param>
		/// <returns></returns>
		public static string MakeListValue(List<uint> vals)
		{
			var builder = new StringBuilder(vals.Count * 7); // enough unless VERY big numbers
			for (int i = 0; i < vals.Count; i++)
			{
				if (i != 0)
					builder.Append(",");
				builder.Append(vals[i].ToString(CultureInfo.InvariantCulture));
			}
			return builder.ToString();
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <param name="defaultValue"></param>
		/// <returns>The value of the attribute, or the default value, if the attribute dismissing</returns>
		public static bool GetOptionalBooleanAttributeValue(XmlNode node, string attrName, bool defaultValue)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(node, attrName, defaultValue?"true":"false"));
		}

		/// <summary>
		/// Get an optional attribute value from an XElement.
		/// </summary>
		/// <param name="element">The XElement to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <param name="defaultValue"></param>
		/// <returns>The value of the attribute, or the default value, if the attribute dismissing</returns>
		public static bool GetOptionalBooleanAttributeValue(XElement element, string attrName, bool defaultValue)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(element, attrName, defaultValue ? "true" : "false"));
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetAttributeValue(XmlNode node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XElement to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetAttributeValue(XElement node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XmlNode node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName, null);
		}

		/// <summary>
		/// Get an optional attribute value from an XElement.
		/// </summary>
		/// <param name="node">The XElement to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XElement node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName, null);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <param name="defaultString"></param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XmlNode node, string attrName, string defaultString)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[attrName];
				if (xa != null)
					return xa.Value;
			}
			return defaultString;
		}

		/// <summary>
		/// Get an optional attribute value from an XElement.
		/// </summary>
		/// <param name="element">The XElement to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <param name="defaultString"></param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XElement element, string attrName, string defaultString)
		{
			if (element == null || !element.Attributes().Any())
			{
				return defaultString;
			}
			var attribute = element.Attribute(attrName);
			return attribute != null ? attribute.Value : defaultString;
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode, and look up its localized value in the
		/// given StringTable.
		/// </summary>
		public static string GetLocalizedAttributeValue(XmlNode node,
			string attrName, string defaultString)
		{
			string sValue = GetOptionalAttributeValue(node, attrName, defaultString);
			return StringTable.Table.LocalizeAttributeValue(sValue);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode, and look up its localized value in the
		/// given StringTable.
		/// </summary>
		public static string GetLocalizedAttributeValue(XElement element,
			string attrName, string defaultString)
		{
			var sValue = GetOptionalAttributeValue(element, attrName, defaultString);
			return StringTable.Table.LocalizeAttributeValue(sValue);
		}

		/// <summary>
		/// Return the node that has the desired 'name', either the input node or a decendent.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="name">The XmlNode name to find.</param>
		/// <returns></returns>
		public static XElement FindNode(XElement node, string name)
		{
			if (node.Name == name)
				return node;
			foreach (var childNode in node.Elements())
			{
				if (childNode.Name == name)
					return childNode;
				var n = FindNode(childNode, name);
				if (n != null)
					return n;
			}
			return null;
		}

		/// <summary>
		/// Return the element that has the desired 'name', either the input element or a decendent.
		/// </summary>
		/// <param name="element">The XElement to look in.</param>
		/// <param name="name">The XElement name to find.</param>
		/// <returns></returns>
		public static XElement FindElement(XElement element, string name)
		{
			if (element.Name == name)
				return element;
			foreach (var childElement in element.Elements())
			{
				if (childElement.Name == name)
					return childElement;
				var grandchildElement = FindElement(childElement, name);
				if (grandchildElement != null)
					return grandchildElement;
			}
			return null;
		}

		/// <summary>
		/// Get an obligatory attribute value.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The required attribute to find.</param>
		/// <returns>The value of the attribute.</returns>
		/// <exception cref="ApplicationException">
		/// Thrown when the value is not found in the node.
		/// </exception>
		public static string GetManditoryAttributeValue(XmlNode node, string attrName)
		{
			string retval = GetOptionalAttributeValue(node, attrName, null);
			if (retval == null)
			{
				throw new ApplicationException("The attribute'"
					+ attrName
					+ "' is mandatory, but was missing. "
					+ node.OuterXml);
			}
			return retval;
		}

		/// <summary>
		/// Get an obligatory attribute value.
		/// </summary>
		/// <param name="element">The XElement to look in.</param>
		/// <param name="attrName">The required attribute to find.</param>
		/// <returns>The value of the attribute.</returns>
		/// <exception cref="ApplicationException">
		/// Thrown when the value is not found in the node.
		/// </exception>
		public static string GetManditoryAttributeValue(XElement element, string attrName)
		{
			var retval = GetOptionalAttributeValue(element, attrName, null);
			if (retval == null)
			{
				throw new ApplicationException("The attribute'"
					+ attrName
					+ "' is mandatory, but was missing. "
					+ element);
			}
			return retval;
		}

		/// <summary>
		/// Append an attribute with the specified name and value to parent.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="attrName"></param>
		/// <param name="attrVal"></param>
		public static void AppendAttribute(XmlNode parent, string attrName, string attrVal)
		{
			XmlAttribute xa = parent.OwnerDocument.CreateAttribute(attrName);
			xa.Value = attrVal;
			parent.Attributes.Append(xa);
		}

		/// <summary>
		/// Append an attribute with the specified name and value to parent.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="attrName"></param>
		/// <param name="attrVal"></param>
		public static void AppendAttribute(XElement parent, string attrName, string attrVal)
		{
			var attr = parent.Attribute(attrName);
			if (attr == null)
				parent.Add(new XAttribute(attrName, attrVal));
			else if (attr.Value != attrVal)
				attr.SetValue(attrVal);
		}

		/// <summary>
		/// Change the value of the specified attribute, appending it if not already present.
		/// </summary>
		public static void SetAttribute(XmlNode parent, string attrName, string attrVal)
		{
			XmlAttribute xa = parent.Attributes[attrName];
			if (xa != null)
				xa.Value = attrVal;
			else
				AppendAttribute(parent, attrName, attrVal);
		}

		/// <summary>
		/// Change the value of the specified attribute, appending it if not already present.
		/// </summary>
		public static void SetAttribute(XElement parent, string attrName, string attrVal)
		{
			var xa = parent.Attribute(attrName);
			if (xa != null)
				xa.Value = attrVal;
			else
				AppendAttribute(parent, attrName, attrVal);
		}

		/// <summary>
		/// Append an attribute with the specified name and value to parent.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="elementName"></param>
		public static XmlElement AppendElement(XmlNode parent, string elementName)
		{
			XmlElement xe = parent.OwnerDocument.CreateElement(elementName);
			parent.AppendChild(xe);
			return xe;
		}

		/// <summary>
		/// Return true if the two nodes match. Corresponding children should match, and
		/// corresponding attributes (though not necessarily in the same order).
		/// Comments do not affect equality.
		/// </summary>
		/// <param name="node1"></param>
		/// <param name="node2"></param>
		/// <returns></returns>
		static public bool NodesMatch(XElement node1, XElement node2)
		{
			if (node1 == null && node2 == null)
				return true;
			if (node1 == null || node2 == null)
				return false;
			if (node1.Name != node2.Name)
				return false;
			if (node1.GetInnerText() != node2.GetInnerText())
				return false;
			if (!node1.Attributes().Any() && node2.Attributes().Any())
				return false;
			if (node1.Attributes().Any() && !node2.Attributes().Any())
				return false;
			if (node1.Attributes().Any())
			{
				if (node1.Attributes().Count() != node2.Attributes().Count())
					return false;
				foreach (var xa1 in node1.Attributes())
				{
					var xa2 = node2.Attribute(xa1.Name);
					if (xa2 == null || xa1.Value != xa2.Value)
						return false;
				}
			}
			if (!node1.HasElements && node2.HasElements)
				return false;
			if (node1.HasElements && !node2.HasElements)
				return false;
			if (node1.HasElements)
			{
				int ichild1 = 0; // index node1.Elements()
				int ichild2 = 0; // index node2.Elements()
				while (ichild1 < node1.Elements().Count() && ichild2 < node1.Elements().Count())
				{
					var child1 = node1.Elements().ToList()[ichild1];
					var child2 = node2.Elements().ToList()[ichild2];

					if (!NodesMatch(child1, child2))
						return false;
					ichild1++;
					ichild2++;
				}
				// If we finished both lists we got a match.
				return (ichild1 == node1.Elements().Count()) && (ichild2 == node2.Elements().Count());
			}

			// both lists are null
			return true;
		}

		/// <summary>
		/// Return the first child of the node that is not a comment (or null).
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static XmlNode GetFirstNonCommentChild(XmlNode node)
		{
			if (node == null)
				return null;
			foreach(XmlNode child in node.ChildNodes)
				if (!(child is XmlComment))
					return child;
			return null;
		}

		/// <summary>
		/// Return the first child of the node that is not a comment (or null).
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static XElement GetFirstNonCommentChild(XElement element)
		{
			return (element == null) ? null : element.Elements().FirstOrDefault();
		}

		/// <summary>
		/// Convert an encoded string (safe XML) into plain text.
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		public static string DecodeXml(string sInput)
		{
			string sOutput = sInput;

			if (!String.IsNullOrEmpty(sOutput))
			{
				sOutput = sOutput.Replace("&amp;", "&");
				sOutput = sOutput.Replace("&lt;", "<");
				sOutput = sOutput.Replace("&gt;", ">");
			}
			return sOutput;
		}

		/// <summary>
		/// Fix the string to be safe in a text region of XML.
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		public static string MakeSafeXml(string sInput)
		{
			string sOutput = sInput;

			if (!String.IsNullOrEmpty(sOutput))
			{
				sOutput = sOutput.Replace("&", "&amp;");
				sOutput = sOutput.Replace("<", "&lt;");
				sOutput = sOutput.Replace(">", "&gt;");
			}
			return sOutput;
		}

		/// <summary>
		/// Convert a possibly multiparagraph string to a form that is safe to store both in an XML file. Also deals
		/// with some characters that are not safe to put in an FDO field, which is more to the point since that's
		/// where most of this data is heading.
		/// </summary>
		/// <remarks>JohnT: escaping the XML characters is slightly bizarre, since LiftMerger is an IMPORT function and for the most part,
		/// we are not creating XML files. Most of the places we want to put this text, in FDO objects, we end up
		/// Decoding it again. Worth considering refactoring so that this method (renamed) just deals with characters
		/// we don't want in FDO objects, like tab and newline, and leaves the XML reserved characters alone. Then
		/// we could get rid of a lot of Decode statements also.
		/// Steve says one place we do need to make encoded XML is in the content of Residue fields.</remarks>
		/// <param name="sInput"></param>
		/// <returns></returns>
		[SuppressMessage("Gendarme.Rules.Portability", "NewLineLiteralRule",
			Justification="Replacing new line characters")]
		public static string ConvertMultiparagraphToSafeXml(string sInput)
		{
			string sOutput = sInput;

			if (!String.IsNullOrEmpty(sOutput))
			{
				sOutput = sOutput.Replace(Environment.NewLine, "\u2028");
				sOutput = sOutput.Replace("\n", "\u2028");
				sOutput = sOutput.Replace("\r", "\u2028");
				sOutput = MakeSafeXml(sOutput);
			}
			return sOutput;
		}

		/// <summary>
		/// Fix the string to be safe in an attribute value of XML.
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		public static string MakeSafeXmlAttribute(string sInput)
		{
			string sOutput = sInput;

			if (!String.IsNullOrEmpty(sOutput))
			{
				sOutput = sOutput.Replace("&", "&amp;");
				sOutput = sOutput.Replace("\"", "&quot;");
				sOutput = sOutput.Replace("'", "&apos;");
				sOutput = sOutput.Replace("<", "&lt;");
				sOutput = sOutput.Replace(">", "&gt;");
				for (int i = 0; i < sOutput.Length; ++i)
				{
					if (Char.IsControl(sOutput, i))
					{
						char c = sOutput[i];
						string sReplace = String.Format("&#x{0:X};", (int)c);
						sOutput = sOutput.Replace(c.ToString(CultureInfo.InvariantCulture), sReplace);
						i += (sReplace.Length - 1);		// skip over the replacement string.
					}
				}
			}
			return sOutput;
		}

		/// <summary>
		/// Convert an encoded attribute string into plain text.
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		public static string DecodeXmlAttribute(string sInput)
		{
			string sOutput = sInput;
			if (!String.IsNullOrEmpty(sOutput) && sOutput.Contains("&"))
			{
				sOutput = sOutput.Replace("&gt;", ">");
				sOutput = sOutput.Replace("&lt;", "<");
				sOutput = sOutput.Replace("&apos;", "'");
				sOutput = sOutput.Replace("&quot;", "\"");
				sOutput = sOutput.Replace("&amp;", "&");
			}
			for (int idx = sOutput.IndexOf("&#"); idx >= 0; idx = sOutput.IndexOf("&#"))
			{
				int idxEnd = sOutput.IndexOf(';', idx);
				if (idxEnd < 0)
					break;
				string sOrig = sOutput.Substring(idx, (idxEnd - idx) + 1);
				string sNum = sOutput.Substring(idx + 2, idxEnd - (idx + 2));
				string sReplace = null;
				int chNum = 0;
				if (sNum[0] == 'x' || sNum[0] == 'X')
				{
					if (Int32.TryParse(sNum.Substring(1), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out chNum))
						sReplace = Char.ConvertFromUtf32(chNum);
				}
				else
				{
					if (Int32.TryParse(sNum, out chNum))
						sReplace = Char.ConvertFromUtf32(chNum);
				}
				if (sReplace == null)
					sReplace = sNum;
				sOutput = sOutput.Replace(sOrig, sReplace);
			}
			return sOutput;
		}

		/// <summary>
		/// build an xpath to the given node in its document.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetXPathInDocument(XmlNode node)
		{
			if (node == null || node.NodeType != XmlNodeType.Element)
				return "";
			//XmlNode parent = node.ParentNode;
			// start with the name of the node, and tentatively guess it to be the root element.
			string xpath = String.Format("/{0}", node.LocalName);
			// append the index of the node amongst any preceding siblings.
			int index = GetIndexAmongSiblings(node);
			if (index != -1)
			{
				index = index + 1; // add one for an xpath index.
				xpath += String.Format("[{0}]", index);
			}
			return String.Concat(GetXPathInDocument(node.ParentNode), xpath);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		/// <returns>Zero-based Index of Node in ParentNode.ChildNodes. -1, if node has no parent.</returns>
		[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
			Justification = "In .NET 4.5 XmlNodeList implements IDisposable, but not in 4.0.")]
		public static int GetIndexAmongSiblings(XmlNode node)
		{
			XmlNode parent = node.ParentNode;
			if (parent != null)
			{
				return node.SelectNodes("./preceding-sibling::" + node.LocalName).Count;
			}
			return -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Find the index of the node in nodes that 'matches' the target node.
		/// Return -1 if not found.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static int FindIndexOfMatchingNode(IEnumerable<XElement> nodes, XElement target)
		{
			int index = 0;
			foreach (var node in nodes)
			{
				if (NodesMatch(node, target))
					return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// return the deep clone of the given node, in a clone of its document context.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static XmlNode CloneNodeWithDocument(XmlNode node)
		{
			if (node == null)
				return null;
			// get the xpath of the node in its document
			if (node.NodeType != XmlNodeType.Document)
			{
				string xpath = GetXPathInDocument(node);
				XmlNode clonedOwner = node.OwnerDocument.CloneNode(true);
				return clonedOwner.SelectSingleNode(xpath);
			}

			return node.CloneNode(true);
		}

		#region Serialize/Deserialize

		/// <summary>
		/// Try to serialize the given object into an xml string
		/// </summary>
		/// <param name="objToSerialize"></param>
		/// <returns>empty string if couldn't serialize object</returns>
		public static string SerializeObjectToXmlString(object objToSerialize)
		{
			string settingsXml = "";
			using (MemoryStream stream = new MemoryStream())
			{
				using (XmlTextWriter textWriter = new XmlTextWriter(stream, Encoding.UTF8))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
					xmlSerializer.Serialize(textWriter, objToSerialize);
					textWriter.Flush();
					stream.Seek(0, SeekOrigin.Begin);
					XmlDocument doc = new XmlDocument();
					doc.Load(stream);
					settingsXml = doc.OuterXml;
				}
			}
			return settingsXml;
		}

		/// <summary>
		/// Deserialize the given xml string into an object of targetType class
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="targetType"></param>
		/// <returns>null if we didn't deserialize the object</returns>
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		static public object DeserializeXmlString(string xml, Type targetType)
		{
			// TODO-Linux: System.Boolean System.Type::op_{Ine,E}quality(System.Type,System.Type)
			// is marked with [MonoTODO] and might not work as expected in 4.0.
			if (String.IsNullOrEmpty(xml) || targetType == null)
				return null;
			using (MemoryStream stream = new MemoryStream())
			{
				using (XmlTextWriter textWriter = new XmlTextWriter(stream, Encoding.UTF8))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xml);
					// get the type from the xml itself.
					// if we can find an existing class/type, we can try to deserialize to it.
					if (targetType != null)
					{
						doc.WriteContentTo(textWriter);
						textWriter.Flush();
						stream.Seek(0, SeekOrigin.Begin);
						XmlSerializer xmlSerializer = new XmlSerializer(targetType);
						try
						{
							object settings = xmlSerializer.Deserialize(stream);
							return settings;
						}
						catch
						{
							// something went wrong trying to deserialize the xml
							// perhaps the structure of the stored data no longer matches the class
						}
					}
				}
			}
			return null;
		}

		#endregion Serialize/Deserialize


		/// <summary>
		/// Utility function to find a methodInfo for the named method.
		/// It is a static method of the class specified in the EditRowClass of the EditRowAssembly.
		/// </summary>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public static MethodInfo GetStaticMethod(XmlNode node, string sAssemblyAttr, string sClassAttr,
			string sMethodName, out Type typeFound)
		{
			string sAssemblyName = GetAttributeValue(node, sAssemblyAttr);
			string sClassName = GetAttributeValue(node, sClassAttr);
			MethodInfo mi = GetStaticMethod(sAssemblyName, sClassName, sMethodName,
				"node " + node.OuterXml, out typeFound);
			return mi;
		}

		/// <summary>
		/// Utility function to find a methodInfo for the named method.
		/// It is a static method of the class specified in the EditRowClass of the EditRowAssembly.
		/// </summary>
		public static MethodInfo GetStaticMethod(XElement node, string sAssemblyAttr, string sClassAttr,
			string sMethodName, out Type typeFound)
		{
			string sAssemblyName = GetAttributeValue(node, sAssemblyAttr);
			string sClassName = GetAttributeValue(node, sClassAttr);
			MethodInfo mi = GetStaticMethod(sAssemblyName, sClassName, sMethodName,
				"node " + node.GetOuterXml(), out typeFound);
			return mi;
		}

		/// <summary>
		/// Utility function to find a methodInfo for the named method.
		/// It is a static method of the class specified in the EditRowClass of the EditRowAssembly.
		/// </summary>
		[SuppressMessage("Gendarme.Rules.Portability", "MonoCompatibilityReviewRule",
			Justification="See TODO-Linux comment")]
		public static MethodInfo GetStaticMethod(string sAssemblyName, string sClassName,
			string sMethodName, string sContext, out Type typeFound)
		{
			typeFound = null;
			Assembly assemblyFound;
			try
			{
				string baseDir = Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().CodeBase).
					Substring(MiscUtils.IsUnix ? 5 : 6);
				assemblyFound = Assembly.LoadFrom(
					Path.Combine(baseDir, sAssemblyName));
			}
			catch (Exception error)
			{
				string sMainMsg = "DLL at " + sAssemblyName;
				string sMsg = MakeGetStaticMethodErrorMessage(sMainMsg, sContext);
				throw new RuntimeConfigurationException(sMsg, error);
			}
			Debug.Assert(assemblyFound != null);
			try
			{
				typeFound = assemblyFound.GetType(sClassName);
			}
			catch (Exception error)
			{
				string sMainMsg = "class called " + sClassName;
				string sMsg = MakeGetStaticMethodErrorMessage(sMainMsg, sContext);
				throw new RuntimeConfigurationException(sMsg, error);
			}
			// TODO-Linux: System.Boolean System.Type::op_Inequality(System.Type,System.Type)
			// is marked with [MonoTODO] and might not work as expected in 4.0.
			Debug.Assert(typeFound != null);
			MethodInfo mi;
			try
			{
				mi = typeFound.GetMethod(sMethodName);
			}
			catch (Exception error)
			{
				string sMainMsg = "method called " + sMethodName + " of class " + sClassName +
					" in assembly " + sAssemblyName;
				string sMsg = MakeGetStaticMethodErrorMessage(sMainMsg, sContext);
				throw new RuntimeConfigurationException(sMsg, error);
			}
			return mi;
		}
		static protected string MakeGetStaticMethodErrorMessage(string sMainMsg, string sContext)
		{
			string sResult = "GetStaticMethod() could not find the " + sMainMsg +
				" while processing " + sContext;
			return sResult;
		}

		/// <summary>
		/// Allow the visitor to 'visit' each attribute in the input XmlNode.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="visitor"></param>
		/// <returns>true if any Visit call returns true</returns>
		public static bool VisitAttributes(XmlNode input, IAttributeVisitor visitor)
		{
			bool fSuccessfulVisit = false;
			if (input.Attributes != null) // can be, e.g, if Input is a XmlTextNode
			{
				foreach (XmlAttribute xa in input.Attributes)
				{
					if (visitor.Visit(new XAttribute(xa.Name, xa.Value)))
						fSuccessfulVisit = true;
				}
			}
			if (input.ChildNodes != null) // not sure whether this can happen.
			{
				foreach (XmlNode child in input.ChildNodes)
				{
					if (VisitAttributes(child, visitor))
						fSuccessfulVisit = true;
				}
			}
			return fSuccessfulVisit;
		}

		/// <summary>
		/// Allow the visitor to 'visit' each attribute in the input XmlNode.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="visitor"></param>
		/// <returns>true if any Visit call returns true</returns>
		public static bool VisitAttributes(XElement input, IAttributeVisitor visitor)
		{
			bool fSuccessfulVisit = false;
			if (input.HasAttributes) // can be, e.g, if Input is a XmlTextNode
			{
				foreach (XAttribute xa in input.Attributes())
				{
					if (visitor.Visit(xa))
						fSuccessfulVisit = true;
				}
			}
			if (input.Elements().Any()) // not sure whether this can happen.
			{
				foreach (var child in input.Elements())
				{
					if (VisitAttributes(child, visitor))
						fSuccessfulVisit = true;
				}
			}
			return fSuccessfulVisit;
		}

		public static XslCompiledTransform CreateTransform(string xslName, string assemblyName)
		{
			var transform = new XslCompiledTransform();
#if !__MonoCS__
			// Assumes the XSL has been precompiled.  xslName is the name of the precompiled class
			Type type = Type.GetType(xslName + "," + assemblyName);
			Debug.Assert(type != null);
			transform.Load(type);
#else
			string libPath = Path.GetDirectoryName(FileUtils.StripFilePrefix(Assembly.GetExecutingAssembly().CodeBase));
			Assembly transformAssembly = Assembly.LoadFrom(Path.Combine(libPath, assemblyName + ".dll"));
			using (Stream stream = transformAssembly.GetManifestResourceStream(xslName + ".xsl"))
			{
				Debug.Assert(stream != null);
				using (XmlReader reader = XmlReader.Create(stream))
					transform.Load(reader, new XsltSettings(true, false), new XmlResourceResolver(transformAssembly));
			}
#endif
			return transform;
		}

#if __MonoCS__
		private class XmlResourceResolver : XmlUrlResolver
		{
			private readonly Assembly m_assembly;

			public XmlResourceResolver(Assembly assembly)
			{
				m_assembly = assembly;
			}

			public override Uri ResolveUri(Uri baseUri, string relativeUri)
			{
				if (baseUri == null)
					return new Uri(string.Format("res://{0}", relativeUri));
				return base.ResolveUri(baseUri, relativeUri);
			}

			[SuppressMessage("Gendarme.Rules.Correctness", "EnsureLocalDisposalRule",
				Justification = "Method returns a reference")]
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				switch (absoluteUri.Scheme)
				{
				case "res":
					return m_assembly.GetManifestResourceStream(absoluteUri.OriginalString.Substring(6));

				default:
					// Handle file:// and http://
					// requests from the XmlUrlResolver base class
					return base.GetEntity(absoluteUri, role, ofObjectToReturn);
				}
			}
		}
#endif
	}

	/// <summary>
	/// Interface for operations we can apply to attributes.
	/// </summary>
	public interface IAttributeVisitor
	{
		bool Visit(XAttribute xa);
	}

	public class ReplaceSubstringInAttr : IAttributeVisitor
	{
		string m_pattern;
		string m_replacement;
		public ReplaceSubstringInAttr(string pattern, string replacement)
		{
			m_pattern = pattern;
			m_replacement = replacement;
		}
		public virtual bool Visit(XAttribute xa)
		{
			string old = xa.Value;
			int index = old.IndexOf(m_pattern);
			if (index < 0)
				return false;
			xa.Value = old.Replace(m_pattern, m_replacement);
			return false;
		}
	}
}
