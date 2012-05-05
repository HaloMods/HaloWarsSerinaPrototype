using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	partial class BDatabaseBase
	{
		protected virtual void PreStreamXml(FA mode) { }
		protected virtual void PostStreamXml(FA mode) { }

		PhxEngine.XmlFileInfo StreamTacticsGetFileInfo(FA mode)
		{
			return new PhxEngine.XmlFileInfo()
			{
				Location = ContentStorage.UpdateOrGame,
				Directory = GameDirectory.Tactics,

				RootName = BTacticData.kXmlRoot,

				Writable = mode == FA.Write,
			};
		}
		static void StreamTactic(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db, string name)
		{
			var td = new BTacticData();

			if (mode == FA.Read) db.FixTacticsXml(s, name);
			td.StreamXml(s, mode, db);

			db.TacticsMap[name] = td;
		}

		/// <remarks>For streaming directly from gamedata.xml</remarks>
		static void StreamXmlGameData(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.GameData.StreamGameXml(s, mode, db);
		}
		/// <remarks>For streaming directly from damagetypes.xml</remarks>
		static void StreamXmlDamageTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.DamageTypes.Params.SetForceNoRootElementStreaming(true);
			db.DamageTypes.StreamXml(s, mode, db);
			db.DamageTypes.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from weapontypes.xml</remarks>
		static void StreamXmlWeaponTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.WeaponTypes.Params.SetForceNoRootElementStreaming(true);
			db.WeaponTypes.StreamXml(s, mode, db);
			db.WeaponTypes.Params.SetForceNoRootElementStreaming(false);
			if (mode == FA.Read) db.FixWeaponTypes();
		}
		/// <remarks>For streaming directly from UserClasses.xml</remarks>
		static void StreamXmlUserClasses(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.UserClasses.Params.SetForceNoRootElementStreaming(true);
			db.UserClasses.StreamXml(s, mode, db);
			db.UserClasses.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from objecttypes.xml</remarks>
		static void StreamXmlObjectTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.ObjectTypes.Params.SetForceNoRootElementStreaming(true);
			db.ObjectTypes.StreamXml(s, mode, db);
			db.ObjectTypes.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from abilities.xml</remarks>
		static void StreamXmlAbilities(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Abilities.Params.SetForceNoRootElementStreaming(true);
			db.Abilities.StreamXml(s, mode, db);
			db.Abilities.Params.SetForceNoRootElementStreaming(false);
		}

		static void PreloadObjects(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Objects.Params.SetForceNoRootElementStreaming(true);
			db.Objects.StreamXmlPreload(s, db);
			db.Objects.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from objects.xml</remarks>
		static void StreamXmlObjects(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			if (mode == FA.Read) db.FixObjectsXml(s);
			db.Objects.Params.SetForceNoRootElementStreaming(true);
			db.Objects.StreamXml(s, mode, db);
			db.Objects.Params.SetForceNoRootElementStreaming(false);
		}

		static void PreloadSquads(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Squads.Params.SetForceNoRootElementStreaming(true);
			db.Squads.StreamXmlPreload(s, db);
			db.Squads.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from squads.xml</remarks>
		static void StreamXmlSquads(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Squads.Params.SetForceNoRootElementStreaming(true);
			db.Squads.StreamXml(s, mode, db);
			db.Squads.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from powers.xml</remarks>
		static void StreamXmlPowers(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Powers.Params.SetForceNoRootElementStreaming(true);
			db.Powers.StreamXml(s, mode, db);
			db.Powers.Params.SetForceNoRootElementStreaming(false);
		}

		static void PreloadTechs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Techs.Params.SetForceNoRootElementStreaming(true);
			db.Techs.StreamXmlPreload(s, db);
			db.Techs.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from techs.xml</remarks>
		static void StreamXmlTechs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			if (mode == FA.Read) db.FixTechsXml(s);
			db.Techs.Params.SetForceNoRootElementStreaming(true);
			db.Techs.StreamXml(s, mode, db);
			db.Techs.Params.SetForceNoRootElementStreaming(false);
		}

		/// <remarks>For streaming directly from civs.xml</remarks>
		static void StreamXmlCivs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Civs.Params.SetForceNoRootElementStreaming(true);
			db.Civs.StreamXml(s, mode, db);
			db.Civs.Params.SetForceNoRootElementStreaming(false);
		}
		/// <remarks>For streaming directly from leaders.xml</remarks>
		static void StreamXmlLeaders(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Leaders.Params.SetForceNoRootElementStreaming(true);
			db.Leaders.StreamXml(s, mode, db);
			db.Leaders.Params.SetForceNoRootElementStreaming(false);
		}

		#region Update
		/// <remarks>For streaming directly from objects.xml</remarks>
		static void StreamXmlObjectsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			//if(mode == FA.Read) db.FixObjectsXml(s);
			db.Objects.Params.SetForceNoRootElementStreaming(true);
			db.Objects.StreamXmlUpdate(s, db);
			db.Objects.Params.SetForceNoRootElementStreaming(false);
		}

		/// <remarks>For streaming directly from squads.xml</remarks>
		static void StreamXmlSquadsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.Squads.Params.SetForceNoRootElementStreaming(true);
			db.Squads.StreamXmlUpdate(s, db);
			db.Squads.Params.SetForceNoRootElementStreaming(false);
		}

		/// <remarks>For streaming directly from techs.xml</remarks>
		static void StreamXmlTechsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			if (mode == FA.Read) db.FixTechsXml(s);
			db.Techs.Params.SetForceNoRootElementStreaming(true);
			db.Techs.StreamXmlUpdate(s, db);
			db.Techs.Params.SetForceNoRootElementStreaming(false);
		}
		#endregion
	};
}