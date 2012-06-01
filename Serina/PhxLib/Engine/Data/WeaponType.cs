using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public class BDamageModifier : IO.IPhxXmlStreamable, IEqualityComparer<BDamageModifier>
	{
		#region Xml constants
		public static readonly Collections.BTypeValuesParams<BDamageModifier> kBListParams = new
			Collections.BTypeValuesParams<BDamageModifier>(db => db.DamageTypes);
		public static readonly XML.BTypeValuesXmlParams<BDamageModifier> kBListXmlParams = new
			XML.BTypeValuesXmlParams<BDamageModifier>("DamageModifier", "type");

		const string kXmlAttrRating = "rating";
		//float reflectDamageFactor
		//bool bowlable, rammable
		#endregion

		float mRating;
		public float Rating { get { return mRating; } }
		float mValue;
		public float Value { get { return mValue; } }

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrRating, ref mRating);
			s.StreamCursor(mode, ref mValue);
		}
		#endregion

		#region IEqualityComparer<BDamageModifier> Members
		public bool Equals(BDamageModifier x, BDamageModifier y)
		{
			return x.Rating == y.Rating && x.Value == y.Value;
		}

		public int GetHashCode(BDamageModifier obj)
		{
			return obj.Rating.GetHashCode() ^ obj.Value.GetHashCode();
		}
		#endregion
	};
	public class BWeaponType : Collections.BListAutoIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("WeaponType")
		{
			DataName = "Name",
			Flags = XML.BCollectionXmlParamsFlags.UseElementForData
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "WeaponTypes.xml",
			RootName = kBListXmlParams.RootName
		};

		//string DeathAnimation
		#endregion

		public Collections.BTypeValues<BDamageModifier> DamageModifiers { get; private set; }

		public BWeaponType()
		{
			DamageModifiers = new Collections.BTypeValues<BDamageModifier>(BDamageModifier.kBListParams);
		}

		#region BListObjectBase Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			XML.Util.Serialize(s, mode, xs, DamageModifiers, BDamageModifier.kBListXmlParams);
		}
		#endregion
	};
}