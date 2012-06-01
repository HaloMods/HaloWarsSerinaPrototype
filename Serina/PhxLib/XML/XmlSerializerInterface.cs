using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using PhxUtil = PhxLib.Util;

namespace PhxLib.XML
{
	public abstract class BXmlSerializerInterface : System.IDisposable
	{
		#region NullInterface
		class NullInterface : BXmlSerializerInterface
		{
			Engine.BDatabaseBase mDatabase;
			internal override Engine.BDatabaseBase Database { get { return mDatabase; } }

			public NullInterface(Engine.BDatabaseBase db) { mDatabase = db; }

			public override void Dispose() {}
		};
		public static BXmlSerializerInterface GetNullInterface(Engine.BDatabaseBase db)
		{
			Contract.Requires(db != null);

			return new NullInterface(db);
		}
		#endregion

		internal abstract Engine.BDatabaseBase Database { get; }

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

		protected static bool ToLowerName(Engine.DatabaseObjectKind kind)
		{
			switch (kind)
			{
				case Engine.DatabaseObjectKind.Object:
				case Engine.DatabaseObjectKind.Unit: return Engine.BProtoObject.kBListXmlParams.ToLowerDataNames;

				case Engine.DatabaseObjectKind.Squad: return Engine.BProtoSquad.kBListXmlParams.ToLowerDataNames;

				case Engine.DatabaseObjectKind.Tech: return Engine.BProtoTech.kBListXmlParams.ToLowerDataNames;

				default: return false;
			}
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
			bool to_lower = ToLowerName(kind);

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

		internal static void StreamDamageType(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.DamageType, false, Util.kSourceCursor);
		}
		internal static void StreamSquadID(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.Squad, false, Util.kSourceCursor);
		}
		internal static void StreamUnitID(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs,
			XML.BListOfIDsXmlParams<object> @params, object ctxt, ref int id)
		{
			xs.StreamXmlForDBID(s, mode, null, ref id, Engine.DatabaseObjectKind.Unit, false, Util.kSourceCursor);
		}

		#region IDisposable Members
		public abstract void Dispose();
		#endregion
	};
}