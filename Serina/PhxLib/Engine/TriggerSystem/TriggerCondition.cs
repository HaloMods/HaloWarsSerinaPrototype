using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BTriggerConditionType
	{
		#region 0

		#region 00
		CanGetUnits = 2,

		CanGetSquads = 4,
		TechStatus = 5,

		TriggerActiveTime = 7,
		#endregion
		#region 10
		CompareInteger = 14,
		CanPayCost = 15,
		#endregion
		#region 20
		CanUsePower = 27,
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 100

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		CanRetrieveExternals = 192,
		#endregion

		#endregion

		#region 200

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		CompareCiv = 240,
		#endregion
		#region 50
		#endregion
		#region 60
		GameTimeReached = 268,
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 300

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		CanGetOneSquad = 357,
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 400

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		CompareFloat = 453,
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		CanGetOneProtoSquad = 486,
		#endregion
		#region 90
		#endregion

		#endregion

		#region 500

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		CanGetOneInteger = 552,
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 600

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		UISquadListOK = 640,
		UISquadListCancel = 641,
		UISquadListUILockError = 642,
		UISquadListWaiting = 643,
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 700

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		CheckPop = 763,
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 800

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		#endregion

		#endregion

		#region 900

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		AICanGetDifficultySetting = 920,
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion
		#region 70
		#endregion
		#region 80
		#endregion
		#region 90
		CanGetCoopPlayer = 991,
		#endregion

		#endregion

		#region 1000

		#region 00
		#endregion
		#region 10
		#endregion
		#region 20
		#endregion
		#region 30
		#endregion
		#region 40
		#endregion
		#region 50
		#endregion
		#region 60
		#endregion

		#endregion
	};

	public class BTriggerProtoCondition : TriggerProtoDbObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Condition")
		{
			DataName = DatabaseNamedObject.kXmlAttrNameN,
			Flags = 0,
		};

		const string kXmlAttrAsync = "Async";
		const string kXmlAttrAsyncParameterKey = "AsyncParameterKey"; // really a sbyte
		#endregion

		bool mAsync;
		public bool Async { get { return mAsync; } }

		int mAsyncParameterKey;
		public int AsyncParameterKey { get { return mAsyncParameterKey; } }

		public BTriggerProtoCondition() { }
		public BTriggerProtoCondition(BTriggerSystem root, BTriggerCondition instance) : base(root, instance)
		{
			mAsync = instance.Async;
			mAsyncParameterKey = instance.AsyncParameterKey;
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			if(s.StreamAttributeOpt(mode, kXmlAttrAsync, ref mAsync, Util.kNotFalsePredicate))
				s.StreamAttribute(mode, kXmlAttrAsyncParameterKey, ref mAsyncParameterKey);
		}
	};

	public class BTriggerCondition : TriggerScriptDbObjectWithArgs
	{
		#region Xml constants
		public const string kXmlRootName = "TriggerConditions";

		public static readonly XML.BListXmlParams kBListXmlParams_And = new XML.BListXmlParams
		{
			RootName = "And",
			ElementName = "Condition",
			DataName = kXmlAttrType,
		};
		public static readonly XML.BListXmlParams kBListXmlParams_Or = new XML.BListXmlParams
		{
			RootName = "Or",
			ElementName = "Condition",
			DataName = kXmlAttrType,
		};

		const string kXmlAttrInvert = "Invert";
		const string kXmlAttrAsync = "Async"; // engine treats this as optional, but not the key
		const string kXmlAttrAsyncParameterKey = "AsyncParameterKey"; // really a sbyte
		#endregion

		bool mInvert;

		bool mAsync;
		public bool Async { get { return mAsync; } }

		int mAsyncParameterKey; // References a Parameter (via SigID). Runtime then takes that parameter's BTriggerVarID
		public int AsyncParameterKey { get { return mAsyncParameterKey; } }

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamAttribute(mode, kXmlAttrInvert, ref mInvert);
			s.StreamAttribute(mode, kXmlAttrAsync, ref mAsync);
			s.StreamAttribute(mode, kXmlAttrAsyncParameterKey, ref mAsyncParameterKey);
		}
	};
}