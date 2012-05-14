using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.HaloWars
{
	using PhxLib.Engine;

	public partial class BDatabase : Engine.BDatabaseBase
	{
		static readonly Collections.CodeEnum<BCodeObjectType> kGameObjectTypes = new Collections.CodeEnum<BCodeObjectType>();
		static readonly Collections.CodeEnum<BCodeProtoObject> kGameProtoObjectTypes = new Collections.CodeEnum<BCodeProtoObject>();
		static readonly Collections.CodeEnum<BScenarioWorld> kGameScenarioWorlds = new Collections.CodeEnum<BScenarioWorld>();

		public override Collections.IProtoEnum GameObjectTypes { get { return kGameObjectTypes; } }
		public override Collections.IProtoEnum GameProtoObjectTypes { get { return kGameProtoObjectTypes; } }
		public override Collections.IProtoEnum GameScenarioWorlds { get { return kGameScenarioWorlds; } }

		public int RepairPowerID { get; private set; }
		public int RallyPointPowerID { get; private set; }
		public int HookRepairPowerID { get; private set; }
		public int UnscOdstDropPowerID { get; private set; }

		public BDatabase(PhxEngine engine) : base(engine, kGameObjectTypes)
		{
			RepairPowerID = RallyPointPowerID = HookRepairPowerID = UnscOdstDropPowerID = 
				Util.kInvalidInt32;
		}

		void SetupDBIDs()
		{
			RepairPowerID = base.GetId(DatabaseObjectKind.Power, "_Repair");
			RallyPointPowerID = base.GetId(DatabaseObjectKind.Power, "_RallyPoint");
			HookRepairPowerID = base.GetId(DatabaseObjectKind.Power, "HookRepair");
			UnscOdstDropPowerID = base.GetId(DatabaseObjectKind.Power, "UnscOdstDrop");
		}
	};
}