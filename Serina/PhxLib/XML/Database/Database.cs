using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using PhxUtil = PhxLib.Util;

namespace PhxLib.XML
{
	[Flags]
	public enum BDatabaseXmlSerializerLoadFlags
	{
		LoadUpdates = 1<<0,
		UseSynchronousLoading = 1<<1,
	};
	public abstract partial class BDatabaseXmlSerializerBase : IDisposable, KSoft.IO.IXmlElementStreamable
	{
		internal abstract Engine.BDatabaseBase Database { get; }

		XML.IBListAutoIdXmlSerializer mDamageTypesSerializer, mObjectsSerializer, 
			mSquadsSerializer, 
			mPowersSerializer, 
			mTechsSerializer
			;

		protected BDatabaseXmlSerializerBase()
		{
			ObjectIdToTacticsMap = new Dictionary<int, string>();
			TacticsMap = new Dictionary<string, Engine.BTacticData>();
		}

		#region IDisposable Members
		public void Dispose()
		{
			AutoIdSerializersDispose();
		}
		#endregion

		protected virtual void AutoIdSerializersInitialize()
		{
			if (mDamageTypesSerializer == null)
				mDamageTypesSerializer = Util.CreateXmlSerializer(Database.DamageTypes, Engine.BDamageType.kBListXmlParams);

			if (mObjectsSerializer == null)
				mObjectsSerializer = Util.CreateXmlSerializer(Database.Objects, Engine.BProtoObject.kBListXmlParams);
			if (mSquadsSerializer == null)
				mSquadsSerializer = Util.CreateXmlSerializer(Database.Squads, Engine.BProtoSquad.kBListXmlParams);
			if (mPowersSerializer == null)
				mPowersSerializer = Util.CreateXmlSerializer(Database.Powers, Engine.BProtoPower.kBListXmlParams);
			if (mTechsSerializer == null)
				mTechsSerializer = Util.CreateXmlSerializer(Database.Techs, Engine.BProtoTech.kBListXmlParams);
		}
		protected virtual void AutoIdSerializersDispose()
		{
			PhxLib.Util.DisposeAndNull(ref mDamageTypesSerializer);

			PhxLib.Util.DisposeAndNull(ref mObjectsSerializer);
			PhxLib.Util.DisposeAndNull(ref mSquadsSerializer);
			PhxLib.Util.DisposeAndNull(ref mPowersSerializer);
			PhxLib.Util.DisposeAndNull(ref mTechsSerializer);
		}

		public void StreamXmlForStringID(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref int value, XmlNodeType type = Util.kSourceElement)
		{
			if (type == XmlNodeType.Element)		s.StreamElementOpt(mode, name, KSoft.NumeralBase.Decimal, ref value, PhxUtil.kNotInvalidPredicate);
			else if (type == XmlNodeType.Attribute)	s.StreamAttributeOpt(mode, name, KSoft.NumeralBase.Decimal, ref value, PhxUtil.kNotInvalidPredicate);
			else if (type == XmlNodeType.Text)		s.StreamCursor(mode, KSoft.NumeralBase.Decimal, ref value);

			if (mode == FA.Read)
			{
				if (value != PhxUtil.kInvalidInt32)
					Database.AddStringIDReference(value);
			}
		}

		public bool StreamXmlTactic(KSoft.IO.XmlElementStream s, FA mode, string xml_name, Engine.BProtoObject obj, 
			ref bool was_streamed, XmlNodeType xml_source = Util.kSourceElement)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(xml_source));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(xml_source) == (xml_name != null));

			string id_name = null;
			bool to_lower = false;

			if (mode == FA.Read)
			{
				was_streamed = Util.StreamStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);

				if (was_streamed)
				{
					id_name = System.IO.Path.GetFileNameWithoutExtension(id_name);

					ObjectIdToTacticsMap[obj.AutoID] = id_name;
					TacticsMap[id_name] = null;
				}
			}
			else if (mode == FA.Write && was_streamed)
			{
				id_name = obj.Name + Engine.BTacticData.kFileExt;
				Util.StreamStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
			}

			return was_streamed;
		}

		public bool StreamXmlForTypeName(KSoft.IO.XmlElementStream s, FA mode, string xml_name, ref int dbid,
			Engine.DatabaseTypeKind kind,
			bool is_optional = true, XmlNodeType xml_source = Util.kSourceElement)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(xml_source));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(xml_source) == (xml_name != null));

			string id_name = null;
			bool was_streamed = true;
			bool to_lower = false;

			if (mode == FA.Read)
			{
				if (is_optional)
					was_streamed = Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);

				if (was_streamed)
				{
					dbid = Database.GetId(kind, id_name);
					Contract.Assert(dbid != PhxUtil.kInvalidInt32);
				}
				else
					dbid = PhxUtil.kInvalidInt32;
			}
			else if (mode == FA.Write && dbid != PhxUtil.kInvalidInt32)
			{
				id_name = Database.GetName(kind, dbid);
				Contract.Assert(!string.IsNullOrEmpty(id_name));

				if (is_optional)
					Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);
			}

			return was_streamed;
		}
		public bool StreamXmlForDBID(KSoft.IO.XmlElementStream s, FA mode, string xml_name, ref int dbid, 
			Engine.DatabaseObjectKind kind,
			bool is_optional = true, XmlNodeType xml_source = Util.kSourceElement)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(xml_source));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(xml_source) == (xml_name != null));

			string id_name = null;
			bool was_streamed = true;
			bool to_lower = kind == Engine.DatabaseObjectKind.Object || kind == Engine.DatabaseObjectKind.Unit || 
				kind == Engine.DatabaseObjectKind.Squad || kind == Engine.DatabaseObjectKind.Tech;

			if (mode == FA.Read)
			{
				if (is_optional)
					was_streamed = Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);

				if (was_streamed)
				{
					dbid = Database.GetId(kind, id_name);
					Contract.Assert(dbid != PhxUtil.kInvalidInt32);
				}
				else
					dbid = PhxUtil.kInvalidInt32;
			}
			else if (mode == FA.Write && dbid != PhxUtil.kInvalidInt32)
			{
				id_name = Database.GetName(kind, dbid);
				Contract.Assert(!string.IsNullOrEmpty(id_name));

				if (is_optional)
					Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);
			}

			return was_streamed;
		}

		internal static void StreamDamageType(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.DamageType, false, Util.kSourceCursor);
		}
		internal static void StreamSquadID(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.Squad, false, Util.kSourceCursor);
		}
		internal static void StreamUnitID(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.Unit, false, Util.kSourceCursor);
		}
	};
}