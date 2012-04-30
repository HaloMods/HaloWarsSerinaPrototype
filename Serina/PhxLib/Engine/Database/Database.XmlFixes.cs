using System.Xml;

namespace PhxLib.Engine
{
	partial class BDatabaseBase
	{
		static XmlNode XPathSelectNodeByName(KSoft.IO.XmlElementStream s, Collections.BListParams op, 
			string data_name, string attr_name = DatabaseNamedObject.kXmlAttrName)
		{
			string xpath = string.Format(
				"/{0}/{1}[@{2}='{3}']",
				op.RootName, op.ElementName, attr_name, data_name);
			return s.Document.SelectSingleNode(xpath);
		}

		// Fix float values which are in an invalid format for .NET's parsing
		static void FixObjectsXmlBullshitInvalidSingles(KSoft.IO.XmlElementStream s)
		{
			var node = XPathSelectNodeByName(s, BProtoObject.kBListParams, "unsc_veh_cobra_01");
			if (node == null) return;

			var element = node[BProtoObject.kXmlElementAttackGradeDPS] as XmlElement;

			string txt = element.InnerText;
			int idx = txt.IndexOf('.');
			if (idx != -1 && (idx = txt.IndexOf('.', idx)) != -1)
				element.InnerText = txt.Remove(idx, txt.Length - idx);
		}
		static void FixObjectsXmlBullshit(KSoft.IO.XmlElementStream s)
		{
			FixObjectsXmlBullshitInvalidSingles(s);
		}

		// Rename the SubType attribute to be all lowercase (subType is only uppercase for 'TurretYawRate'...)
		static void FixTechsXmlBullshitEffectsDataSubType(XmlDocument doc, XmlNode n)
		{
			const string kAttrNameOld = "subType";
			const string kAttrName = "subtype";

			string xpath = "Effects/Effect[@" + kAttrNameOld + "]";

			var elements = n.SelectNodes(xpath);

			foreach (XmlElement e in elements)
			{
				var attr_old = e.Attributes[kAttrNameOld];
				var attr = doc.CreateAttribute(kAttrName);
				attr.Value = attr_old.Value;
				e.Attributes.InsertBefore(attr, attr_old);
				e.RemoveAttribute(kAttrNameOld);
			}
		}
		// Remove non-existent ProtoTechs that are referenced by effects
		static void FixTechsXmlBullshitEffectsInvalid(KSoft.IO.XmlElementStream s, Collections.BListParams op)
		{
			string xpath = string.Format(
				"/{0}/{1}/Effects/Effect/Target",
				op.RootName, op.ElementName);

			var elements = s.Document.SelectNodes(xpath);

			foreach(XmlElement e in elements)
			{
				if (e.InnerText != "unsc_turret_upgrade3") continue;

				var p = e.ParentNode;
				p.ParentNode.RemoveChild(p);
			}
		}
		static void FixTechsXmlBullshit(KSoft.IO.XmlElementStream s)
		{
			var node = XPathSelectNodeByName(s, BProtoTech.kBListParams, "unsc_scorpion_upgrade3");
			if (node != null) FixTechsXmlBullshitEffectsDataSubType(s.Document, node);

			node = XPathSelectNodeByName(s, BProtoTech.kBListParams, "unsc_grizzly_upgrade0");
			if (node != null) FixTechsXmlBullshitEffectsDataSubType(s.Document, node);

			FixTechsXmlBullshitEffectsInvalid(s, BProtoTech.kBListParams);
		}
	};
}