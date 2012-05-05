using System.Xml;

namespace PhxLib.Engine
{
	partial class BDatabaseBase
	{
		protected virtual void FixWeaponTypes() {}

		protected static XmlNode XPathSelectNodeByName(KSoft.IO.XmlElementStream s, Collections.BListParams op, 
			string data_name, string attr_name = DatabaseNamedObject.kXmlAttrName)
		{
			string xpath = string.Format(
				"/{0}/{1}[@{2}='{3}']",
				op.RootName, op.ElementName, attr_name, data_name);
			return s.Document.SelectSingleNode(xpath);
		}

		protected virtual void FixObjectsXml(KSoft.IO.XmlElementStream s) {}

		protected virtual void FixTechsXml(KSoft.IO.XmlElementStream s) {}

		protected static void FixTacticsTraceFixEvent(string tactic_name, string xpath)
		{
			Debug.Trace.Engine.TraceEvent(System.Diagnostics.TraceEventType.Warning, -1,
					"Fixing Tactic '{0}' with XPath={1}", tactic_name, xpath);
		}
		protected virtual void FixTacticsXml(KSoft.IO.XmlElementStream s, string name) {}
	};
}