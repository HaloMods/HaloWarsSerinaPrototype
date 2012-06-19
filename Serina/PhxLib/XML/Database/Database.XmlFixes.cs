using System.Xml;

namespace PhxLib.XML
{
	partial class BDatabaseXmlSerializerBase
	{
		protected virtual void FixWeaponTypes() {}

		protected static XmlNode XPathSelectNodeByName(KSoft.IO.XmlElementStream s, XML.BListXmlParams op, 
			string data_name, string attr_name = Engine.DatabaseNamedObject.kXmlAttrName)
		{
			string xpath = string.Format(
				"/{0}/{1}[@{2}='{3}']",
				op.RootName, op.ElementName, attr_name, data_name);
			return s.Document.SelectSingleNode(xpath);
		}

		protected virtual void FixGameDataXml(KSoft.IO.XmlElementStream s) { }
		protected virtual void FixGameData() { }

		protected virtual void FixObjectsXml(KSoft.IO.XmlElementStream s) {}

		protected virtual void FixSquadsXml(KSoft.IO.XmlElementStream s) { }

		protected virtual void FixTechsXml(KSoft.IO.XmlElementStream s) {}

		protected static void FixTacticsTraceFixEvent(string tactic_name, string xpath)
		{
			Debug.Trace.XML.TraceEvent(System.Diagnostics.TraceEventType.Warning, -1,
					"Fixing Tactic '{0}' with XPath={1}", tactic_name, xpath);
		}
		protected virtual void FixTacticsXml(KSoft.IO.XmlElementStream s, string name) {}
	};
}