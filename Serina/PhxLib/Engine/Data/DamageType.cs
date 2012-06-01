using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public class BDamageType : Collections.BListAutoIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("DamageType", 
			XML.BCollectionXmlParamsFlags.RequiresDataNamePreloading);
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "DamageTypes.xml",
			RootName = kBListXmlParams.RootName
		};

		const string kXmlAttrAttackRating = "AttackRating";
		const string kXmlAttrBaseType = "BaseType";
		const string kXmlAttrShielded = "Shielded";
		#endregion

		bool m_attackRating;
		public bool AttackRating { get { return m_attackRating; } }

		bool m_baseType;
		public bool BaseType { get { return m_baseType; } }

		bool m_shielded;
		public bool Shielded { get { return m_shielded; } }

		#region BListAutoIdObject Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttributeOpt(mode, kXmlAttrAttackRating, ref m_attackRating, Util.kNotFalsePredicate);
			s.StreamAttributeOpt(mode, kXmlAttrBaseType, ref m_baseType, Util.kNotFalsePredicate);
			s.StreamAttributeOpt(mode, kXmlAttrShielded, ref m_shielded, Util.kNotFalsePredicate);
		}
		#endregion
	};
}