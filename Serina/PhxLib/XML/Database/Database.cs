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
	public abstract partial class BDatabaseXmlSerializerBase : BXmlSerializerInterface,
		KSoft.IO.IXmlElementStreamable
	{
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
		public override void Dispose()
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

		public bool StreamXmlTactic(KSoft.IO.XmlElementStream s, FA mode, string xmlName, Engine.BProtoObject obj, 
			ref bool wasStreamed, XmlNodeType xmlSource = Util.kSourceElement)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(xmlSource));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(xmlSource) == (xmlName != null));

			string id_name = null;
			bool to_lower = false;

			if (mode == FA.Read)
			{
				wasStreamed = Util.StreamStringOpt(s, mode, xmlName, ref id_name, to_lower, xmlSource);

				if (wasStreamed)
				{
					id_name = System.IO.Path.GetFileNameWithoutExtension(id_name);

					ObjectIdToTacticsMap[obj.AutoID] = id_name;
					TacticsMap[id_name] = null;
				}
			}
			else if (mode == FA.Write && wasStreamed)
			{
				id_name = obj.Name + Engine.BTacticData.kFileExt;
				Util.StreamStringOpt(s, mode, xmlName, ref id_name, to_lower, xmlSource);
			}

			return wasStreamed;
		}
	};
}