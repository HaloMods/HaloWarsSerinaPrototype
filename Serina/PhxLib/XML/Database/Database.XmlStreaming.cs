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

		public Dictionary<int, string/*BTacticData*/> ObjectIdToTacticsMap { get; private set; }
		public Dictionary<string, Engine.BTacticData> TacticsMap { get; private set; }

		PhxEngine.XmlFileInfo StreamTacticsGetFileInfo(FA mode)
		{
			return new PhxEngine.XmlFileInfo()
			{
				Location = Engine.ContentStorage.UpdateOrGame,
				Directory = Engine.GameDirectory.Tactics,

				RootName = Engine.BTacticData.kXmlRoot,

				Writable = mode == FA.Write,
			};
		}
		static void StreamTactic(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs, string name)
		{
			var td = new Engine.BTacticData(name);

			if (mode == FA.Read) xs.FixTacticsXml(s, name);
			td.StreamXml(s, mode, xs);

			xs.TacticsMap[name] = td;
		}

		/// <remarks>For streaming directly from gamedata.xml</remarks>
		static void StreamXmlGameData(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			xs.Database.GameData.StreamGameXml(s, mode, xs);
		}
		/// <remarks>For streaming directly from damagetypes.xml</remarks>
		static void StreamXmlDamageTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.DamageTypes, Engine.BDamageType.kBListXmlParams, true);
		}
		/// <remarks>For streaming directly from weapontypes.xml</remarks>
		static void StreamXmlWeaponTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.WeaponTypes, Engine.BWeaponType.kBListXmlParams, true);
			if (mode == FA.Read) xs.FixWeaponTypes();
		}
		/// <remarks>For streaming directly from UserClasses.xml</remarks>
		static void StreamXmlUserClasses(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.UserClasses, Engine.BUserClass.kBListXmlParams, true);
		}
		/// <remarks>For streaming directly from objecttypes.xml</remarks>
		static void StreamXmlObjectTypes(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.ObjectTypes, Engine.BDatabaseBase.kObjectTypesXmlParams, true);
		}
		/// <remarks>For streaming directly from abilities.xml</remarks>
		static void StreamXmlAbilities(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.Abilities, Engine.BAbility.kBListXmlParams, true);
		}

		static void PreloadObjects(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.SerializePreload(s, xs, xs.mObjectsSerializer, true);
		}
		/// <remarks>For streaming directly from objects.xml</remarks>
		static void StreamXmlObjects(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			if (mode == FA.Read) xs.FixObjectsXml(s);
			XML.Util.Serialize(s, mode, xs, xs.mObjectsSerializer, true);
		}

		static void PreloadSquads(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.SerializePreload(s, xs, xs.mSquadsSerializer, true);
		}
		/// <remarks>For streaming directly from squads.xml</remarks>
		static void StreamXmlSquads(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.mSquadsSerializer, true);
		}
		/// <remarks>For streaming directly from powers.xml</remarks>
		static void StreamXmlPowers(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.Powers, Engine.BProtoPower.kBListXmlParams, true);
		}

		static void PreloadTechs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.SerializePreload(s, xs, xs.mTechsSerializer, true);
		}
		/// <remarks>For streaming directly from techs.xml</remarks>
		static void StreamXmlTechs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			if (mode == FA.Read) xs.FixTechsXml(s);
			XML.Util.Serialize(s, mode, xs, xs.mTechsSerializer, true);
		}

		/// <remarks>For streaming directly from civs.xml</remarks>
		static void StreamXmlCivs(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.Civs, Engine.BCiv.kBListXmlParams, true);
		}
		/// <remarks>For streaming directly from leaders.xml</remarks>
		static void StreamXmlLeaders(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.Serialize(s, mode, xs, xs.Database.Leaders, Engine.BLeader.kBListXmlParams, true);
		}

		#region Update
		/// <remarks>For streaming directly from objects_update.xml</remarks>
		static void StreamXmlObjectsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			//if(mode == FA.Read) xs.FixObjectsXml(s);
			XML.Util.SerializeUpdate(s, xs, xs.mObjectsSerializer, true);
		}

		/// <remarks>For streaming directly from squads_update.xml</remarks>
		static void StreamXmlSquadsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			XML.Util.SerializeUpdate(s, xs, xs.mSquadsSerializer, true);
		}

		/// <remarks>For streaming directly from techs_update.xml</remarks>
		static void StreamXmlTechsUpdate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			if (mode == FA.Read) xs.FixTechsXml(s);
			XML.Util.SerializeUpdate(s, xs, xs.mTechsSerializer, true);
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