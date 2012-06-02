using System.Collections.Generic;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class BDatabaseXmlSerializerBase
	{
		protected virtual void PreStreamXml(FA mode) { }
		protected virtual void PostStreamXml(FA mode)
		{
			if (mode == FA.Read)
			{
				Database.BuildObjectTacticsMap(ObjectIdToTacticsMap, TacticsMap);
			}
		}

		Dictionary<int, string> ObjectIdToTacticsMap;
		Dictionary<string, Engine.BTacticData> TacticsMap;

		PhxEngine.XmlFileInfo StreamTacticsGetFileInfo(FA mode, string filename = null)
		{
			return new PhxEngine.XmlFileInfo()
			{
				Location = Engine.ContentStorage.UpdateOrGame,
				Directory = Engine.GameDirectory.Tactics,

				RootName = Engine.BTacticData.kXmlRoot,
				FileName = filename,

				Writable = mode == FA.Write,
			};
		}
		void StreamTactic(KSoft.IO.XmlElementStream s, FA mode, string name)
		{
			var td = new Engine.BTacticData(name);

			if (mode == FA.Read) FixTacticsXml(s, name);
			td.StreamXml(s, mode, this);

			TacticsMap[name] = td;
		}

		/// <remarks>For streaming directly from gamedata.xml</remarks>
		void StreamXmlGameData(KSoft.IO.XmlElementStream s, FA mode)
		{
			if(mode == FA.Read) FixGameDataXml(s);
			Database.GameData.StreamGameXml(s, mode, this);
		}
		void PreloadDamageTypes(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.SerializePreload(s, this, mDamageTypesSerializer, true);
		}
		/// <remarks>For streaming directly from damagetypes.xml</remarks>
		void StreamXmlDamageTypes(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, mDamageTypesSerializer, true);
		}
		/// <remarks>For streaming directly from weapontypes.xml</remarks>
		void StreamXmlWeaponTypes(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.WeaponTypes, Engine.BWeaponType.kBListXmlParams, true);
			if (mode == FA.Read) FixWeaponTypes();
		}
		/// <remarks>For streaming directly from UserClasses.xml</remarks>
		void StreamXmlUserClasses(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.UserClasses, Engine.BUserClass.kBListXmlParams, true);
		}
		/// <remarks>For streaming directly from objecttypes.xml</remarks>
		void StreamXmlObjectTypes(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.ObjectTypes, Engine.BDatabaseBase.kObjectTypesXmlParams, true);
		}
		/// <remarks>For streaming directly from abilities.xml</remarks>
		void StreamXmlAbilities(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.Abilities, Engine.BAbility.kBListXmlParams, true);
		}

		void PreloadObjects(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.SerializePreload(s, this, mObjectsSerializer, true);
		}
		/// <remarks>For streaming directly from objects.xml</remarks>
		void StreamXmlObjects(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) FixObjectsXml(s);
			XML.Util.Serialize(s, mode, this, mObjectsSerializer, true);
		}

		void PreloadSquads(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.SerializePreload(s, this, mSquadsSerializer, true);
		}
		/// <remarks>For streaming directly from squads.xml</remarks>
		void StreamXmlSquads(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) FixSquadsXml(s);
			XML.Util.Serialize(s, mode, this, mSquadsSerializer, true);
		}

		void PreloadPowers(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.SerializePreload(s, this, mPowersSerializer, true);
		}
		/// <remarks>For streaming directly from powers.xml</remarks>
		void StreamXmlPowers(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, mPowersSerializer, true);
		}

		void PreloadTechs(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.SerializePreload(s, this, mTechsSerializer, true);
		}
		/// <remarks>For streaming directly from techs.xml</remarks>
		void StreamXmlTechs(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) FixTechsXml(s);
			XML.Util.Serialize(s, mode, this, mTechsSerializer, true);
		}

		/// <remarks>For streaming directly from civs.xml</remarks>
		void StreamXmlCivs(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.Civs, Engine.BCiv.kBListXmlParams, true);
		}
		/// <remarks>For streaming directly from leaders.xml</remarks>
		void StreamXmlLeaders(KSoft.IO.XmlElementStream s, FA mode)
		{
			XML.Util.Serialize(s, mode, this, Database.Leaders, Engine.BLeader.kBListXmlParams, true);
		}

		#region Update
		/// <remarks>For streaming directly from objects_update.xml</remarks>
		void StreamXmlObjectsUpdate(KSoft.IO.XmlElementStream s, FA mode)
		{
			//if(mode == FA.Read) FixObjectsXml(s);
			XML.Util.SerializeUpdate(s, this, mObjectsSerializer, true);
		}

		/// <remarks>For streaming directly from squads_update.xml</remarks>
		void StreamXmlSquadsUpdate(KSoft.IO.XmlElementStream s, FA mode)
		{
			//if (mode == FA.Read) FixSquadsXml(s);
			XML.Util.SerializeUpdate(s, this, mSquadsSerializer, true);
		}

		/// <remarks>For streaming directly from techs_update.xml</remarks>
		void StreamXmlTechsUpdate(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) FixTechsXml(s);
			XML.Util.SerializeUpdate(s, this, mTechsSerializer, true);
		}
		#endregion

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode)
		{
			var db = Database;

			PreStreamXml(mode);

			db.GameData.StreamXml(s, mode, this);
			XML.Util.Serialize(s, mode, this, db.DamageTypes, Engine.BDamageType.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.WeaponTypes, Engine.BWeaponType.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.UserClasses, Engine.BUserClass.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.ObjectTypes, Engine.BDatabaseBase.kObjectTypesXmlParams);
			XML.Util.Serialize(s, mode, this, db.Abilities, Engine.BAbility.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Objects, Engine.BProtoObject.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Squads, Engine.BProtoSquad.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Powers, Engine.BProtoPower.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Techs, Engine.BProtoTech.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Civs, Engine.BCiv.kBListXmlParams);
			XML.Util.Serialize(s, mode, this, db.Leaders, Engine.BLeader.kBListXmlParams);

			PostStreamXml(mode);
		}
	};
}