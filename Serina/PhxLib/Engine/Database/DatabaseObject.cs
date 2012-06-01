using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	/// <summary></summary>
	/// <remarks>Most or all of these are defined in GameData, so we may want to rename to GameDataType?</remarks>
	public enum DatabaseTypeKind
	{
		None,

		Cost,
		Pop,
		Rate,
	};
	public enum DatabaseObjectKind
	{
		None,

		Ability,
		Civ,
		DamageType,
		Leader,
		Object,
		ObjectType,
		Power,
		Squad,
		Tech,
		/// <summary>Object or ObjectType</summary>
		Unit,
		UserClass,
		WeaponType,
	};

	public interface IDatabaseIdObject : IO.IPhxXmlStreamable
	{
		int DbId { get; }

		string Name { get; }
	};

	public abstract class DatabaseNamedObject : Collections.BListAutoIdObject
	{
		#region Xml constants
		internal const string kXmlAttrName = "name";
		internal const string kXmlAttrNameN = "Name";
		const string kXmlElementDisplayName = "DisplayNameID";
		const string kXmlElementRolloverTextID = "RolloverTextID";
		const string kXmlElementPrereqTextID = "PrereqTextID";
		#endregion

		int mDisplayNameID;
		public int DisplayNameID { get { return mDisplayNameID; } }

		protected DatabaseNamedObject()
		{
			mDisplayNameID = Util.kInvalidInt32;
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			xs.StreamXmlForStringID(s, mode, kXmlElementDisplayName, ref mDisplayNameID);
		}
		#endregion
	};

	public abstract class DatabasePurchasableObject : DatabaseNamedObject
	{
		#region Xml constants
		const string kXmlElementBuildPoints = "BuildPoints";
		const string kXmlElementResearchPoints = "ResearchPoints";
		#endregion

		XML.BTypeValuesXmlParams<float> mResourceCostXmlParams;

		public Collections.BTypeValuesSingle ResourceCost { get; private set; }

		float mBuildTime;
		public float BuildTime { get { return mBuildTime; } }
		float mResearchTime;
		public float ResearchTime { get { return mResearchTime; } }
		/// <summary>Time, in seconds, it takes to build or research this object</summary>
		public float PurchaseTime { get { return mBuildTime != Util.kGetInvalidSingle() ? mBuildTime : mResearchTime; } }

		protected DatabasePurchasableObject(Collections.BTypeValuesParams<float> rsrc_cost_params, XML.BTypeValuesXmlParams<float> rsrc_cost_xml_params)
		{
			mResourceCostXmlParams = rsrc_cost_xml_params;

			ResourceCost = new Collections.BTypeValuesSingle(rsrc_cost_params);

			mBuildTime = mResearchTime = Util.kGetInvalidSingle();
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			XML.Util.Serialize(s, mode, xs, ResourceCost, mResourceCostXmlParams);
			s.StreamElementOpt(mode, kXmlElementBuildPoints, ref mBuildTime, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementResearchPoints, ref mResearchTime, Util.kNotInvalidPredicateSingle);
		}
		#endregion
	};

	public abstract class DatabaseIdObject : DatabasePurchasableObject, IDatabaseIdObject
	{
		#region Xml constants
		const string kXmlAttrDbId = "dbid";
		#endregion

		protected int mDbId;
		public int DbId { get { return mDbId; } }

		protected DatabaseIdObject(Collections.BTypeValuesParams<float> rsrc_cost_params, XML.BTypeValuesXmlParams<float> rsrc_cost_xml_params)
			: base(rsrc_cost_params, rsrc_cost_xml_params)
		{
			mDbId = Util.kInvalidInt32;
		}

		#region IXmlElementStreamable Members
		protected virtual void StreamXmlDbId(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrDbId, KSoft.NumeralBase.Decimal, ref mDbId);
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			StreamXmlDbId(s, mode, xs);
		}
		#endregion
	};
}