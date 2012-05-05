﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public abstract partial class BDatabaseBase : KSoft.IO.IXmlElementStreamable
	{
		public const string kInvalidString = "BORK BORK BORK";

		#region Xml constants
		static readonly Collections.BListParams kObjectTypesParams = new Collections.BListParams("ObjectType");
		public static readonly PhxEngine.XmlFileInfo kObjectTypesXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "ObjectTypes.xml",
			RootName = kObjectTypesParams.RootName
		};
		#endregion

		public PhxEngine Engine { get; private set; }

		public abstract Collections.IProtoEnum GameObjectTypes { get; }
		public abstract Collections.IProtoEnum GameProtoObjectTypes { get; }
		public abstract Collections.IProtoEnum GameScenarioWorlds { get; }

		/// <summary>StringID values indexed by their official locID</summary>
		/// <remarks>Only populated with strings from a StringTable which PhxLib-savvy objects reference, to cut down on memory usage</remarks>
		public Dictionary<int, string> StringTable { get; private set; }

		public BGameData GameData { get; private set; }
		public Collections.BListAutoId<BDamageType> DamageTypes { get; private set; }
		public Collections.BListAutoId<BWeaponType> WeaponTypes { get; private set; }
		public Collections.BListAutoId<BUserClass> UserClasses { get; private set; }
		public Collections.BTypeNamesWithCode ObjectTypes { get; private set; }
		public Collections.BListAutoId<BAbility> Abilities { get; private set; }
		public Collections.BListAutoId<BProtoObject> Objects { get; private set; }
		public Collections.BListAutoId<BProtoSquad> Squads { get; private set; }
		public Collections.BListAutoId<BProtoPower> Powers { get; private set; }
		public Collections.BListAutoId<BProtoTech> Techs { get; private set; }
		public Collections.BListAutoId<BCiv> Civs { get; private set; }
		public Collections.BListAutoId<BLeader> Leaders { get; private set; }

		public Dictionary<int, string/*BTacticData*/> ObjectIdToTacticsMap { get; private set; }
		public Dictionary<string, BTacticData> TacticsMap { get; private set; }

		protected BDatabaseBase(PhxEngine engine, Collections.IProtoEnum game_object_types)
		{
			Engine = engine;

			StringTable = new Dictionary<int, string>();

			GameData = new BGameData();
			DamageTypes = new Collections.BListAutoId<BDamageType>(BDamageType.kBListParams);
			WeaponTypes = new Collections.BListAutoId<BWeaponType>(BWeaponType.kBListParams);
			UserClasses = new Collections.BListAutoId<BUserClass>(BUserClass.kBListParams);
			ObjectTypes = new Collections.BTypeNamesWithCode(kObjectTypesParams, game_object_types);
			Abilities = new Collections.BListAutoId<BAbility>(BAbility.kBListParams);
			Objects = new Collections.BListAutoId<BProtoObject>(BProtoObject.kBListParams);
			Squads = new Collections.BListAutoId<BProtoSquad>(BProtoSquad.kBListParams);
			Powers = new Collections.BListAutoId<BProtoPower>(BProtoPower.kBListParams);
			Techs = new Collections.BListAutoId<BProtoTech>(BProtoTech.kBListParams);
			Civs = new Collections.BListAutoId<BCiv>(BCiv.kBListParams);
			Leaders = new Collections.BListAutoId<BLeader>(BLeader.kBListParams);

			ObjectIdToTacticsMap = new Dictionary<int, string>();
			TacticsMap = new Dictionary<string, BTacticData>();

			InitializeDatabaseInterfaces();
		}

		#region StringTable Util
		void AddStringIDReference(int index)
		{
			StringTable[index] = kInvalidString;
		}
		void SetStringIDValue(int index, string value)
		{
			StringTable[index] = value;
		}

		public void StreamXmlForStringID(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref int value, XmlNodeType type = Util.kSourceElement)
		{
			if (type == XmlNodeType.Element)		s.StreamElementOpt(mode, name, KSoft.NumeralBase.Decimal, ref value, Util.kNotInvalidPredicate);
			else if (type == XmlNodeType.Attribute)	s.StreamAttributeOpt(mode, name, KSoft.NumeralBase.Decimal, ref value, Util.kNotInvalidPredicate);
			else if (type == XmlNodeType.Text)		s.StreamCursor(mode, KSoft.NumeralBase.Decimal, ref value);

			if (mode == FA.Read)
			{
				if (value != Util.kInvalidInt32)
					AddStringIDReference(value);
			}
		}
		#endregion

		#region Database interfaces
		Dictionary<string, BDamageType> m_dbiDamageTypes;
		Dictionary<string, BWeaponType> m_dbiWeaponTypes;
		Dictionary<string, BUserClass> m_dbiUserClasses;
		Dictionary<string, BAbility> m_dbiAbilities;
		Dictionary<string, BProtoObject> m_dbiObjects;
		Dictionary<string, BProtoSquad> m_dbiSquads;
		Dictionary<string, BProtoPower> m_dbiPowers;
		Dictionary<string, BProtoTech> m_dbiTechs;
		Dictionary<string, BCiv> m_dbiCivs;
		Dictionary<string, BLeader> m_dbiLeaders;

		void InitializeDatabaseInterfaces()
		{
			DamageTypes.SetupDatabaseInterface(out m_dbiDamageTypes);
			WeaponTypes.SetupDatabaseInterface(out m_dbiWeaponTypes);
			UserClasses.SetupDatabaseInterface(out m_dbiUserClasses);
			Abilities.SetupDatabaseInterface(out m_dbiAbilities);
			Objects.SetupDatabaseInterface(out m_dbiObjects);
			Squads.SetupDatabaseInterface(out m_dbiSquads);
			Techs.SetupDatabaseInterface(out m_dbiTechs);
			Powers.SetupDatabaseInterface(out m_dbiPowers);
			Civs.SetupDatabaseInterface(out m_dbiCivs);
			Leaders.SetupDatabaseInterface(out m_dbiLeaders);
		}

		bool DataStoreIsReady(DatabaseObjectKind kind)
		{
			System.Collections.ICollection coll = null;

			switch (kind)
			{
				case DatabaseObjectKind.Ability:	coll = m_dbiAbilities; break;
				case DatabaseObjectKind.Civ:		coll = m_dbiCivs; break;
				case DatabaseObjectKind.Cost:		coll = GameData.Resources; break;
				case DatabaseObjectKind.DamageType:	coll = m_dbiDamageTypes; break;
				case DatabaseObjectKind.Leader:		coll = m_dbiLeaders; break;
				case DatabaseObjectKind.Object:		coll = m_dbiObjects; break;
				case DatabaseObjectKind.ObjectType:	coll = ObjectTypes; break;
				case DatabaseObjectKind.Pop:		coll = GameData.Populations; break;
				case DatabaseObjectKind.Power:		coll = m_dbiPowers; break;
				case DatabaseObjectKind.Squad:		coll = m_dbiSquads; break;
				case DatabaseObjectKind.Tech:		coll = m_dbiTechs; break;
				// TODO: Should just use the Objects DBI AFAICT
				case DatabaseObjectKind.Unit:		coll = m_dbiObjects; break;
				case DatabaseObjectKind.UserClass:	coll = m_dbiUserClasses; break;
				case DatabaseObjectKind.WeaponType:	coll = m_dbiWeaponTypes; break;

				default: throw new Debug.UnreachableException(kind.ToString());
			}

			return coll.Count != 0;
		}

		static int TryGetId(Collections.BTypeNames dbi, string name)
		{
			return dbi.FindIndex(s => Util.StrEqualsIgnoreCase(s, name));
		}
		static string TryGetName(Collections.BTypeNames dbi, int id)
		{
			if (id >= 0 && id < dbi.Count) return dbi[id];

			return null;
		}
		static int TryGetId<T>(Collections.BListAutoId<T> dbi, string name)
			where T : class, Collections.IListAutoIdObject, new()
		{
			return dbi.FindIndex(r => Util.StrEqualsIgnoreCase(r.Data, name));
		}
		internal static string TryGetName<T>(Collections.BListAutoId<T> dbi, int id)
			where T : class, Collections.IListAutoIdObject, new()
		{
			if (id >= 0 && id < dbi.Count) return dbi[id].Data;

			return null;
		}
		internal static int TryGetId<T>(Dictionary<string, T> dbi, string name)
			where T : Collections.IListAutoIdObject
		{
			int id = Util.kInvalidInt32;

			T obj;
			if (dbi.TryGetValue(name, out obj)) id = obj.AutoID;

			return id;
		}

		const int kObjectIdIsObjectTypeBitMask = 1<<30;
		static void ObjectIdIsObjectTypeBitSet(ref int id)
		{
			id |= kObjectIdIsObjectTypeBitMask;
		}
		static bool ObjectIdIsObjectTypeBitGet(ref int id)
		{
			if ((id & kObjectIdIsObjectTypeBitMask) != 0)
			{
				id &= ~kObjectIdIsObjectTypeBitMask;
				return true;
			}
			return false;
		}

		int TryGetIdUnit(string name)
		{
			int id = TryGetId(m_dbiObjects, name);

			if (id == Util.kInvalidInt32 && (id = TryGetId(ObjectTypes, name)) != Util.kInvalidInt32)
				ObjectIdIsObjectTypeBitSet(ref id);

			return id;
		}
		string TryGetNameUnit(int id)
		{
			if (ObjectIdIsObjectTypeBitGet(ref id))
				return TryGetName(ObjectTypes, id);
			
			return TryGetName(Objects, id);
		}

		public int GetId(DatabaseObjectKind kind, string name)
		{
			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetId(m_dbiAbilities, name);
				case DatabaseObjectKind.Civ:		return TryGetId(m_dbiCivs, name);
				case DatabaseObjectKind.Cost:		return TryGetId(GameData.Resources, name);
				case DatabaseObjectKind.DamageType:	return TryGetId(m_dbiDamageTypes, name);
				case DatabaseObjectKind.Leader:		return TryGetId(m_dbiLeaders, name);
				case DatabaseObjectKind.Object:		return TryGetId(m_dbiObjects, name);
				case DatabaseObjectKind.ObjectType:	return TryGetId(ObjectTypes, name);
				case DatabaseObjectKind.Pop:		return TryGetId(GameData.Populations, name);
				case DatabaseObjectKind.Power:		return TryGetId(m_dbiPowers, name);
				case DatabaseObjectKind.Squad:		return TryGetId(m_dbiSquads, name);
				case DatabaseObjectKind.Tech:		return TryGetId(m_dbiTechs, name);
				// TODO: Should just use the Objects DBI AFAICT
				case DatabaseObjectKind.Unit:		return TryGetIdUnit(name);
				case DatabaseObjectKind.UserClass:	return TryGetId(m_dbiUserClasses, name);
				case DatabaseObjectKind.WeaponType:	return TryGetId(m_dbiWeaponTypes, name);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public string GetName(DatabaseObjectKind kind, int id)
		{
			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetName(Abilities, id);
				case DatabaseObjectKind.Civ:		return TryGetName(Civs, id);
				case DatabaseObjectKind.Cost:		return TryGetName(GameData.Resources, id);
				case DatabaseObjectKind.DamageType:	return TryGetName(DamageTypes, id);
				case DatabaseObjectKind.Leader:		return TryGetName(Leaders, id);
				case DatabaseObjectKind.Object:		return TryGetName(Objects, id);
				case DatabaseObjectKind.ObjectType:	return TryGetName(ObjectTypes, id);
				case DatabaseObjectKind.Pop:		return TryGetName(GameData.Populations, id);
				case DatabaseObjectKind.Power:		return TryGetName(Powers, id);
				case DatabaseObjectKind.Squad:		return TryGetName(Squads, id);
				case DatabaseObjectKind.Tech:		return TryGetName(Techs, id);
				// TODO: Should just use the Objects DBI AFAICT
				case DatabaseObjectKind.Unit:		return TryGetNameUnit(id);
				case DatabaseObjectKind.UserClass:	return TryGetName(UserClasses, id);
				case DatabaseObjectKind.WeaponType:	return TryGetName(WeaponTypes, id);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		#endregion

		#region IXmlElementStreamable Members
		public bool StreamXmlTactic(KSoft.IO.XmlElementStream s, FA mode, string xml_name, BProtoObject obj, 
			ref bool was_streamed, XmlNodeType xml_source = XmlNodeType.Element)
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
				id_name = obj.Name + BTacticData.kFileExt;
				Util.StreamStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
			}

			return was_streamed;
		}

		public bool StreamXmlForDBID(KSoft.IO.XmlElementStream s, FA mode, string xml_name, ref int dbid, 
			DatabaseObjectKind kind,
			bool is_optional = true, XmlNodeType xml_source = XmlNodeType.Element)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(xml_source));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(xml_source) == (xml_name != null));
			Contract.Assert(DataStoreIsReady(kind));

			string id_name = null;
			bool was_streamed = true;
			bool to_lower = kind == DatabaseObjectKind.Object || kind == DatabaseObjectKind.Unit || 
				kind == DatabaseObjectKind.Squad || kind == DatabaseObjectKind.Tech;

			if (mode == FA.Read)
			{
				if (is_optional)
					was_streamed = Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);

				if (was_streamed)
				{
					dbid = GetId(kind, id_name);
					Contract.Assert(dbid != Util.kInvalidInt32);
				}
				else
					dbid = Util.kInvalidInt32;
			}
			else if (mode == FA.Write && dbid != Util.kInvalidInt32)
			{
				id_name = GetName(kind, dbid);
				Contract.Assert(!string.IsNullOrEmpty(id_name));

				if (is_optional)
					Util.StreamInternStringOpt(s, mode, xml_name, ref id_name, to_lower, xml_source);
				else
					Util.StreamInternString(s, mode, xml_name, ref id_name, to_lower, xml_source);
			}

			return was_streamed;
		}

		internal static void StreamDamageType(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db,
			Collections.BListOfIDsParams<object> @params, object ctxt, ref int id)
		{
			db.StreamXmlForDBID(s, mode, null, ref id, DatabaseObjectKind.DamageType, false, Util.kSourceCursor);
		}
		internal static void StreamUnitID(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db,
			Collections.BListOfIDsParams<object> @params, object ctxt, ref int id)
		{
			db.StreamXmlForDBID(s, mode, null, ref id, DatabaseObjectKind.Unit, false, Util.kSourceCursor);
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode)
		{
			PreStreamXml(mode);

			GameData.StreamXml(s, mode, this);
			DamageTypes.StreamXml(s, mode, this);
			WeaponTypes.StreamXml(s, mode, this);
			UserClasses.StreamXml(s, mode, this);
			ObjectTypes.StreamXml(s, mode, this);
			Abilities.StreamXml(s, mode, this);
			Objects.StreamXml(s, mode, this);
			Squads.StreamXml(s, mode, this);
			Powers.StreamXml(s, mode, this);
			Techs.StreamXml(s, mode, this);
			Civs.StreamXml(s, mode, this);
			Leaders.StreamXml(s, mode, this);

			PostStreamXml(mode);
		}
		#endregion
	};
}