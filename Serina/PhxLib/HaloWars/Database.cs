using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PhxLib.HaloWars
{
	public class BDatabase : Engine.BDatabaseBase
	{
		static readonly Collections.CodeEnum<BCodeObjectType> kGameObjectTypes = new Collections.CodeEnum<BCodeObjectType>();
		static readonly Collections.CodeEnum<BCodeProtoObject> kGameProtoObjectTypes = new Collections.CodeEnum<BCodeProtoObject>();
		static readonly Collections.CodeEnum<BScenarioWorld> kGameScenarioWorlds = new Collections.CodeEnum<BScenarioWorld>();

		public override Collections.IProtoEnum GameObjectTypes { get { return kGameObjectTypes; } }
		public override Collections.IProtoEnum GameProtoObjectTypes { get { return kGameProtoObjectTypes; } }
		public override Collections.IProtoEnum GameScenarioWorlds { get { return kGameScenarioWorlds; } }

		public BDatabase(PhxEngine engine) : base(engine, kGameObjectTypes)
		{
		}
	};
}