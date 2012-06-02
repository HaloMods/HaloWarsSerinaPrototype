using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public abstract partial class BDatabaseBase : IDisposable
	{
		public const string kInvalidString = "BORK BORK BORK";

		#region Xml constants
		internal static readonly XML.BListXmlParams kObjectTypesXmlParams = new XML.BListXmlParams("ObjectType");
		internal static readonly PhxEngine.XmlFileInfo kObjectTypesXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "ObjectTypes.xml",
			RootName = kObjectTypesXmlParams.RootName
		};
		#endregion

		public PhxEngine Engine { get; private set; }

		protected abstract XML.BDatabaseXmlSerializerBase NewXmlSerializer();

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

		public Dictionary<int, BTacticData> ObjectTacticsMap { get; private set; }

		protected BDatabaseBase(PhxEngine engine, Collections.IProtoEnum game_object_types)
		{
			Engine = engine;

			StringTable = new Dictionary<int, string>();

			GameData = new BGameData();
			DamageTypes = new Collections.BListAutoId<BDamageType>();
			WeaponTypes = new Collections.BListAutoId<BWeaponType>();
			UserClasses = new Collections.BListAutoId<BUserClass>();
			ObjectTypes = new Collections.BTypeNamesWithCode(game_object_types);
			Abilities = new Collections.BListAutoId<BAbility>();
			Objects = new Collections.BListAutoId<BProtoObject>();
			Squads = new Collections.BListAutoId<BProtoSquad>();
			Powers = new Collections.BListAutoId<BProtoPower>();
			Techs = new Collections.BListAutoId<BProtoTech>();
			Civs = new Collections.BListAutoId<BCiv>();
			Leaders = new Collections.BListAutoId<BLeader>();

			InitializeDatabaseInterfaces();
		}

		XML.BTriggerScriptSerializer mTriggerSerializer;
		internal void InitializeTriggerScriptSerializer()
		{
			mTriggerSerializer = new XML.BTriggerScriptSerializer(Engine);
		}
		public BTriggerSystem LoadScript(string script_name, BTriggerScriptType type = BTriggerScriptType.TriggerScript)
		{
			var ctxt = mTriggerSerializer.StreamTriggerScriptGetContext(FA.Read, type, script_name);
			var task = System.Threading.Tasks.Task<bool>.Factory.StartNew((state) => {
				var _ctxt = state as XML.BTriggerScriptSerializer.StreamTriggerScriptContext;
				return Engine.TryStreamData(_ctxt.FileInfo, FA.Read, mTriggerSerializer.StreamTriggerScript, _ctxt);
			}, ctxt);

			return task.Result ? ctxt.Script : null;
		}
		public bool LoadScenarioScripts(string scnr_path)
		{
			var ctxt = mTriggerSerializer.StreamTriggerScriptGetContext(FA.Read, BTriggerScriptType.Scenario, scnr_path);
			var task = System.Threading.Tasks.Task<bool>.Factory.StartNew((state) => {
				var _ctxt = state as XML.BTriggerScriptSerializer.StreamTriggerScriptContext;
				return Engine.TryStreamData(_ctxt.FileInfo, FA.Read, mTriggerSerializer.LoadScenarioScripts, _ctxt);
			}, ctxt);

			return task.Result;
		}

		#region IDisposable Members
		public virtual void Dispose()
		{
			Util.DisposeAndNull(ref mTriggerSerializer);
		}
		#endregion

		internal void BuildObjectTacticsMap(Dictionary<int, string> id_to_tactic_map, Dictionary<string, BTacticData> tactic_name_to_tactic)
		{
			ObjectTacticsMap = new Dictionary<int, BTacticData>(id_to_tactic_map.Count);

			foreach (var kv in id_to_tactic_map)
				ObjectTacticsMap.Add(kv.Key, tactic_name_to_tactic[kv.Value]);
		}

		#region StringTable Util
		internal void AddStringIDReference(int index)
		{
			StringTable[index] = kInvalidString;
		}
		void SetStringIDValue(int index, string value)
		{
			StringTable[index] = value;
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

		bool DataStoreIsReady(DatabaseTypeKind kind)
		{
			System.Collections.ICollection coll = null;

			switch (kind)
			{
				case DatabaseTypeKind.Cost:		coll = GameData.Resources; break;
				case DatabaseTypeKind.Pop:		coll = GameData.Populations; break;
				case DatabaseTypeKind.Rate:		coll = GameData.Rates; break;

				default: throw new Debug.UnreachableException(kind.ToString());
			}

			return coll.Count != 0;
		}
		bool DataStoreIsReady(DatabaseObjectKind kind)
		{
			System.Collections.ICollection coll = null;

			switch (kind)
			{
				case DatabaseObjectKind.Ability:	coll = m_dbiAbilities; break;
				case DatabaseObjectKind.Civ:		coll = m_dbiCivs; break;
				case DatabaseObjectKind.DamageType:	coll = m_dbiDamageTypes; break;
				case DatabaseObjectKind.Leader:		coll = m_dbiLeaders; break;
				case DatabaseObjectKind.Object:		coll = m_dbiObjects; break;
				case DatabaseObjectKind.ObjectType:	coll = ObjectTypes; break;
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

		public int GetId(DatabaseTypeKind kind, string name)
		{
			switch (kind)
			{
				case DatabaseTypeKind.Cost:	return TryGetId(GameData.Resources, name);
				case DatabaseTypeKind.Pop:	return TryGetId(GameData.Populations, name);
				case DatabaseTypeKind.Rate:	return TryGetId(GameData.Rates, name);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public int GetId(DatabaseObjectKind kind, string name)
		{
			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetId(m_dbiAbilities, name);
				case DatabaseObjectKind.Civ:		return TryGetId(m_dbiCivs, name);
				case DatabaseObjectKind.DamageType:	return TryGetId(m_dbiDamageTypes, name);
				case DatabaseObjectKind.Leader:		return TryGetId(m_dbiLeaders, name);
				case DatabaseObjectKind.Object:		return TryGetId(m_dbiObjects, name);
				case DatabaseObjectKind.ObjectType:	return TryGetId(ObjectTypes, name);
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
		public string GetName(DatabaseTypeKind kind, int id)
		{
			switch (kind)
			{
				case DatabaseTypeKind.Cost:	return TryGetName(GameData.Resources, id);
				case DatabaseTypeKind.Pop:	return TryGetName(GameData.Populations, id);
				case DatabaseTypeKind.Rate:	return TryGetName(GameData.Rates, id);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public string GetName(DatabaseObjectKind kind, int id)
		{
			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetName(Abilities, id);
				case DatabaseObjectKind.Civ:		return TryGetName(Civs, id);
				case DatabaseObjectKind.DamageType:	return TryGetName(DamageTypes, id);
				case DatabaseObjectKind.Leader:		return TryGetName(Leaders, id);
				case DatabaseObjectKind.Object:		return TryGetName(Objects, id);
				case DatabaseObjectKind.ObjectType:	return TryGetName(ObjectTypes, id);
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

		public void Load()
		{
			using (var xs = NewXmlSerializer())
			{
				var flags = XML.BDatabaseXmlSerializerLoadFlags.LoadUpdates | XML.BDatabaseXmlSerializerLoadFlags.UseSynchronousLoading;
				xs.Load(flags);
			}
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode)
		{
			using (var xs = NewXmlSerializer())
			{
				xs.StreamXml(s, mode);
			}
		}
	};
}