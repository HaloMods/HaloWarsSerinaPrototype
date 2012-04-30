using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	partial class BDatabaseBase
	{
		void StreamData(FA mode)
		{
			var e = Engine;
			bool r;

			r = e.TryStreamData(BGameData.kXmlFileInfo, mode, StreamXmlGameData);
			r = e.TryStreamData(BDamageType.kXmlFileInfo, mode, StreamXmlDamageTypes);
			r = e.TryStreamData(BWeaponType.kXmlFileInfo, mode, StreamXmlWeaponTypes);
			//r = e.TryStreamData(BUserClass.kXmlFileInfo, mode, StreamXmlUserClasses);
			r = e.TryStreamData(kObjectTypesXmlFileInfo, mode, StreamXmlObjectTypes);
			r = e.TryStreamData(BAbility.kXmlFileInfo, mode, StreamXmlAbilities);
			r = e.TryStreamData(BProtoObject.kXmlFileInfo, mode, StreamXmlObjects);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfo, mode, StreamXmlSquads);
			r = e.TryStreamData(BProtoPower.kXmlFileInfo, mode, StreamXmlPowers);
			r = e.TryStreamData(BProtoTech.kXmlFileInfo, mode, StreamXmlTechs);
			r = e.TryStreamData(BCiv.kXmlFileInfo, mode, StreamXmlCivs);
			r = e.TryStreamData(BLeader.kXmlFileInfo, mode, StreamXmlLeaders);
		}

		// objects, objects_update, objectTypes, squads, squads_update, tech, techs_update, leaders, civs, powers, damageTypes
		void Preload()
		{
			var e = Engine;
			bool r;
			const FA k_mode = FA.Read;

			r = e.TryStreamData(BProtoObject.kXmlFileInfo, k_mode, PreloadObjects);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfo, k_mode, PreloadSquads);

			r = e.TryStreamData(BProtoTech.kXmlFileInfo, k_mode, PreloadTechs);
		}
		public void Load()
		{
			Preload();

			StreamData(FA.Read);
		}

		void StreamDataUpdates()
		{
			var e = Engine;
			bool r;
			const FA k_mode = FA.Read;

			r = e.TryStreamData(BProtoObject.kXmlFileInfoUpdate, k_mode, StreamXmlObjectsUpdate);
			r = e.TryStreamData(BProtoSquad.kXmlFileInfoUpdate, k_mode, StreamXmlSquadsUpdate);

			r = e.TryStreamData(BProtoTech.kXmlFileInfoUpdate, k_mode, StreamXmlTechsUpdate);

			r.ToString();
		}
		public void LoadUpdates()
		{
			StreamDataUpdates();
		}
	};
}