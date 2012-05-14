using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	using PhxLib.Engine;

	partial class BDatabaseXmlSerializerBase
	{
		void StreamTactics(FA mode)
		{
			var e = Database.Engine;
			bool r;
			var xfi = StreamTacticsGetFileInfo(mode);

			var keys_copy = new System.Collections.Generic.List<string>(TacticsMap.Keys);
			foreach (var name in keys_copy)
			{
				xfi.FileName = name;
				r = e.TryStreamData(xfi, mode, this, StreamTactic, xfi.FileName, BTacticData.kFileExt);
			}
		}

		void StreamData(FA mode)
		{
			var e = Database.Engine;
			bool r;

			r = e.TryStreamData(BGameData.kXmlFileInfo, mode, this, StreamXmlGameData);
			r = e.TryStreamData(BDamageType.kXmlFileInfo, mode, this, StreamXmlDamageTypes);
			r = e.TryStreamData(BWeaponType.kXmlFileInfo, mode, this, StreamXmlWeaponTypes);
			r = e.TryStreamData(BUserClass.kXmlFileInfo, mode, this, StreamXmlUserClasses);
			r = e.TryStreamData(BDatabaseBase.kObjectTypesXmlFileInfo, mode, this, StreamXmlObjectTypes);
			r = e.TryStreamData(BAbility.kXmlFileInfo, mode, this, StreamXmlAbilities);
			r = e.TryStreamData(BProtoObject.kXmlFileInfo, mode, this, StreamXmlObjects);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfo, mode, this, StreamXmlSquads);
			r = e.TryStreamData(BProtoPower.kXmlFileInfo, mode, this, StreamXmlPowers);
			r = e.TryStreamData(BProtoTech.kXmlFileInfo, mode, this, StreamXmlTechs);
			r = e.TryStreamData(BCiv.kXmlFileInfo, mode, this, StreamXmlCivs);
			r = e.TryStreamData(BLeader.kXmlFileInfo, mode, this, StreamXmlLeaders);
		}

		// objects, objects_update, objectTypes, squads, squads_update, tech, techs_update, leaders, civs, powers, damageTypes
		void Preload()
		{
			var e = Database.Engine;
			bool r;
			const FA k_mode = FA.Read;

			r = e.TryStreamData(BProtoObject.kXmlFileInfo, k_mode, this, PreloadObjects);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfo, k_mode, this, PreloadSquads);

			r = e.TryStreamData(BProtoTech.kXmlFileInfo, k_mode, this, PreloadTechs);
		}

		void StreamDataUpdates()
		{
			var e = Database.Engine;
			bool r;
			const FA k_mode = FA.Read;

			r = e.TryStreamData(BProtoObject.kXmlFileInfoUpdate, k_mode, this, StreamXmlObjectsUpdate);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfoUpdate, k_mode, this, StreamXmlSquadsUpdate);

			r = e.TryStreamData(BProtoTech.kXmlFileInfoUpdate, k_mode, this, StreamXmlTechsUpdate);

			r.ToString();
		}


		protected virtual void LoadCore()
		{
		}
		protected virtual void LoadUpdates()
		{
			StreamDataUpdates();
		}

		void LoadImpl()
		{
			const FA k_mode = FA.Read;

			Preload();

			PostStreamXml(k_mode);

			StreamData(k_mode);
			LoadCore();
			StreamTactics(k_mode);

			PostStreamXml(k_mode);
		}
		public void Load(BDatabaseXmlSerializerLoadFlags flags = 0)
		{
			AutoIdSerializersInitialize();

			LoadImpl();
			if ((flags & BDatabaseXmlSerializerLoadFlags.LoadUpdates) != 0) LoadUpdates();

			AutoIdSerializersDispose();
		}
	};
}