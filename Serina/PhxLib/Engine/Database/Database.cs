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

		static int TryGetId(Collections.BTypeNames dbi, string name)
		{
			return dbi.TryGetMemberId(name);
		}
		static string TryGetName(Collections.BTypeNames dbi, int id)
		{
			if (id >= 0 && id < dbi.Count) return dbi[id];

			return null;
		}
		static int TryGetId<T>(Collections.BListAutoId<T> dbi, string name)
			where T : class, Collections.IListAutoIdObject, new()
		{
			return dbi.TryGetMemberId(name);
		}
		internal static string TryGetName<T>(Collections.BListAutoId<T> dbi, int id)
			where T : class, Collections.IListAutoIdObject, new()
		{
			if (id >= 0 && id < dbi.Count) return dbi[id].Data;

			return null;
		}
		internal static int TryGetId<T>(Dictionary<string, T> dbi, string name, Collections.BListAutoId<T> _unused)
			where T : class, Collections.IListAutoIdObject, new()
		{
			int id = Util.kInvalidInt32;

			T obj;
			if (dbi.TryGetValue(name, out obj)) id = obj.AutoID;

			return id;
		}

		static int TryGetIdWithUndefined(Collections.BTypeNames dbi, string name)
		{
			return dbi.UndefinedInterface.GetMemberIdOrUndefined(name);
		}
		static string TryGetNameWithUndefined(Collections.BTypeNames dbi, int id)
		{
			return dbi.UndefinedInterface.GetMemberNameOrUndefined(id);
		}
		static int TryGetIdWithUndefined<T>(Collections.BListAutoId<T> dbi, string name)
			where T : class, Collections.IListAutoIdObject, new()
		{
			return dbi.UndefinedInterface.GetMemberIdOrUndefined(name);
		}
		internal static string TryGetNameWithUndefined<T>(Collections.BListAutoId<T> dbi, int id)
			where T : class, Collections.IListAutoIdObject, new()
		{
			return dbi.UndefinedInterface.GetMemberNameOrUndefined(id);
		}
		internal static int TryGetIdWithUndefined<T>(Dictionary<string, T> dbi, string name, Collections.BListAutoId<T> list)
			where T : class, Collections.IListAutoIdObject, new()
		{
			int id = TryGetId(dbi, name, null);

			if (id == Util.kInvalidInt32)
				id = list.UndefinedInterface.GetMemberIdOrUndefined(name);

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
			int id = TryGetIdWithUndefined(m_dbiObjects, name, Objects);

			if (id == Util.kInvalidInt32 && (id = TryGetId(ObjectTypes, name)) != Util.kInvalidInt32)
				ObjectIdIsObjectTypeBitSet(ref id);

			return id;
		}
		string TryGetNameUnit(int id)
		{
			if (ObjectIdIsObjectTypeBitGet(ref id))
				return TryGetNameWithUndefined(ObjectTypes, id);

			return TryGetNameWithUndefined(Objects, id);
		}

		public int GetId(DatabaseTypeKind kind, string name)
		{
			Contract.Requires<ArgumentOutOfRangeException>(kind != DatabaseTypeKind.None);

			switch (kind)
			{
				case DatabaseTypeKind.Cost:	return TryGetIdWithUndefined(GameData.Resources, name);
				case DatabaseTypeKind.Pop:	return TryGetIdWithUndefined(GameData.Populations, name);
				case DatabaseTypeKind.Rate:	return TryGetIdWithUndefined(GameData.Rates, name);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public int GetId(DatabaseObjectKind kind, string name)
		{
			Contract.Requires<ArgumentOutOfRangeException>(kind != DatabaseObjectKind.None);

			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetIdWithUndefined(m_dbiAbilities, name, Abilities);
				case DatabaseObjectKind.Civ:		return TryGetIdWithUndefined(m_dbiCivs, name, Civs);
				case DatabaseObjectKind.DamageType:	return TryGetIdWithUndefined(m_dbiDamageTypes, name, DamageTypes);
				case DatabaseObjectKind.Leader:		return TryGetIdWithUndefined(m_dbiLeaders, name, Leaders);
				case DatabaseObjectKind.Object:		return TryGetIdWithUndefined(m_dbiObjects, name, Objects);
				case DatabaseObjectKind.ObjectType:	return TryGetIdWithUndefined(ObjectTypes, name);
				case DatabaseObjectKind.Power:		return TryGetIdWithUndefined(m_dbiPowers, name, Powers);
				case DatabaseObjectKind.Squad:		return TryGetIdWithUndefined(m_dbiSquads, name, Squads);
				case DatabaseObjectKind.Tech:		return TryGetIdWithUndefined(m_dbiTechs, name, Techs);
				// TODO: Should just use the Objects DBI AFAICT
				case DatabaseObjectKind.Unit:		return TryGetIdUnit(name);
				case DatabaseObjectKind.UserClass:	return TryGetIdWithUndefined(m_dbiUserClasses, name, UserClasses);
				case DatabaseObjectKind.WeaponType:	return TryGetIdWithUndefined(m_dbiWeaponTypes, name, WeaponTypes);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public string GetName(DatabaseTypeKind kind, int id)
		{
			Contract.Requires<ArgumentOutOfRangeException>(kind != DatabaseTypeKind.None);

			switch (kind)
			{
				case DatabaseTypeKind.Cost:	return TryGetNameWithUndefined(GameData.Resources, id);
				case DatabaseTypeKind.Pop:	return TryGetNameWithUndefined(GameData.Populations, id);
				case DatabaseTypeKind.Rate:	return TryGetNameWithUndefined(GameData.Rates, id);

				default: throw new Debug.UnreachableException(kind.ToString());
			}
		}
		public string GetName(DatabaseObjectKind kind, int id)
		{
			Contract.Requires<ArgumentOutOfRangeException>(kind != DatabaseObjectKind.None);

			switch (kind)
			{
				case DatabaseObjectKind.Ability:	return TryGetNameWithUndefined(Abilities, id);
				case DatabaseObjectKind.Civ:		return TryGetNameWithUndefined(Civs, id);
				case DatabaseObjectKind.DamageType:	return TryGetNameWithUndefined(DamageTypes, id);
				case DatabaseObjectKind.Leader:		return TryGetNameWithUndefined(Leaders, id);
				case DatabaseObjectKind.Object:		return TryGetNameWithUndefined(Objects, id);
				case DatabaseObjectKind.ObjectType:	return TryGetNameWithUndefined(ObjectTypes, id);
				case DatabaseObjectKind.Power:		return TryGetNameWithUndefined(Powers, id);
				case DatabaseObjectKind.Squad:		return TryGetNameWithUndefined(Squads, id);
				case DatabaseObjectKind.Tech:		return TryGetNameWithUndefined(Techs, id);
				// TODO: Should just use the Objects DBI AFAICT
				case DatabaseObjectKind.Unit:		return TryGetNameUnit(id);
				case DatabaseObjectKind.UserClass:	return TryGetNameWithUndefined(UserClasses, id);
				case DatabaseObjectKind.WeaponType:	return TryGetNameWithUndefined(WeaponTypes, id);

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