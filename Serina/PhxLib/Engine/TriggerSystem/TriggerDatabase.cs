using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum TriggerScriptDbObjectType
	{
		Invalid = -1,
		ProtoCondition,
		ProtoEffect,
		Template, // is this needed?
	};

	public abstract class TriggerProtoDbObject : Collections.BListAutoIdObject
	{
		#region Xml constants
		const string kXmlAttrDbId = "DBID";
		const string kXmlAttrVersion = "Version";
		#endregion

		int mDbId = Util.kInvalidInt32;
		public int DbId { get { return mDbId; } }

		int mVersion = Util.kInvalidInt32;
		public int Version { get { return mVersion; } }

		public Collections.BListExplicitIndex<BTriggerParam> Params { get; private set; }

		protected TriggerProtoDbObject()
		{
			Params = new Collections.BListExplicitIndex<BTriggerParam>(BTriggerParam.kBListExplicitIndexParams);
		}
		protected TriggerProtoDbObject(BTriggerSystem root, TriggerScriptDbObjectWithArgs instance)
		{
			mName = instance.Name;

			mDbId = instance.DbId;
			mVersion = instance.Version;
			Params = BTriggerParam.BuildDefinition(root, instance.Args);
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrDbId, ref mDbId);
			s.StreamAttribute(mode, kXmlAttrVersion, ref mVersion);

			XML.Util.Serialize(s, mode, xs, Params, BTriggerParam.kBListExplicitIndexXmlParams);
		}

		static bool ContainsUserClassTypeVar(BTriggerSystem ts, TriggerScriptDbObjectWithArgs obj)
		{
			foreach (var arg in obj.Args)
			{
				if (arg.IsInvalid) continue;
				if (arg.GetVarType(ts) == BTriggerVarType.UserClassType) return true;
			}
			return false;
		}
		public virtual int CompareTo(BTriggerSystem ts, TriggerScriptDbObjectWithArgs obj)
		{
			if (Name != obj.Name)
				Debug.Trace.Engine.TraceInformation(
					"TriggerProtoDbObject: '{0}' - Encountered different names for {1}, '{2}' != '{3}'",
					ts, this.DbId.ToString(), this.Name, obj.Name);

			if (ContainsUserClassTypeVar(ts, obj))
			{
				Debug.Trace.Engine.TraceInformation(
					"TriggerProtoDbObject: {0} - Encountered {1}/{2} which has a UserClassType Var, skipping comparison",
					ts, DbId.ToString(), Name);
				return 0;
			}

			Contract.Assert(Version == obj.Version);
			Contract.Assert(Params.Count == obj.Args.Count);

			int diff = 0;
			for (int x = 0; x < Params.Count; x++)
			{
				int sig = Params[x].SigID;
				int obj_sig = obj.Args[x].SigID;
				sig = sig < 0 ? 0 : sig;
				obj_sig = obj_sig < 0 ? 0 : obj_sig;

				diff += sig - obj_sig;
			}
			
			return diff;
		}
	};

	public class TriggerDatabase : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public const string kXmlRootName = "TriggerDatabase";
		#endregion

		public Collections.BListAutoId<BTriggerProtoCondition> Conditions { get; private set; }
		public Collections.BListAutoId<BTriggerProtoEffect> Effects { get; private set; }
		public Dictionary<uint, TriggerProtoDbObject> LookupTable { get; private set; }
		System.Collections.BitArray mUsedIds;

		public TriggerDatabase()
		{
			Conditions = new Collections.BListAutoId<BTriggerProtoCondition>();
			Effects = new Collections.BListAutoId<BTriggerProtoEffect>();
			LookupTable = new Dictionary<uint, TriggerProtoDbObject>();
			mUsedIds = new System.Collections.BitArray(1088);
		}

		#region IPhxXmlStreamable Members
		static int SortById(TriggerProtoDbObject x, TriggerProtoDbObject y)
		{
			if(x.DbId != y.DbId)
				return x.DbId - y.DbId;

			return x.Version - y.Version;
		}
		int WriteUnknowns(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			int count = 0;
			for (int x = 1; x < mUsedIds.Length; x++)
			{
				if (!mUsedIds[x])
				{
					s.WriteElement("Unknown", x);
					count++;
				}
			}
			return count;
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			if (mode == FA.Write)
			{
				var task_sort_cond = Task.Factory.StartNew(() => Conditions.Sort(SortById));
				var task_sort_effe = Task.Factory.StartNew(() => Effects.Sort(SortById));

				var task_unknowns = Task<int>.Factory.StartNew(() =>
				{
					using (s.EnterCursorBookmark("Unknowns"))
						return WriteUnknowns(s, mode, xs);
				});
				s.WriteAttribute("UnknownCount", task_unknowns.Result);
				s.WriteAttribute("ConditionsCount", Conditions.Count);
				s.WriteAttribute("EffectsCount", Effects.Count);

				Task.WaitAll(task_sort_cond, task_sort_effe);
			}

			XML.Util.Serialize(s, mode, xs, Conditions, BTriggerProtoCondition.kBListXmlParams);
			XML.Util.Serialize(s, mode, xs, Effects, BTriggerProtoEffect.kBListXmlParams);

			if (mode == FA.Read)
			{
				foreach (var c in Conditions) LookupTableAdd(c);
				foreach (var e in Effects) LookupTableAdd(e);
			}
		}
		#endregion

		static uint GenerateHandle(TriggerProtoDbObject dbo)
		{
			return ((uint)dbo.DbId << 8) | (uint)dbo.Version;
		}
		static uint GenerateHandle(TriggerScriptDbObject dbo)
		{
			return ((uint)dbo.DbId << 8) | (uint)dbo.Version;
		}
		static void HandleGetData(uint handle, out int dbid, out int version)
		{
			version = (int)(handle & 0xFF);
			dbid = (int)(handle >> 8);
		}

		void LookupTableAdd(TriggerProtoDbObject dbo)
		{
			mUsedIds[dbo.DbId] = true;
			LookupTable.Add(GenerateHandle(dbo), dbo);
		}
		bool LookupTableContains<T>(T obj, out TriggerProtoDbObject dbo)
			where T : TriggerScriptDbObject
		{
			return LookupTable.TryGetValue(GenerateHandle(obj), out dbo);
		}

		static void TraceUpdate(BTriggerSystem ts, TriggerProtoDbObject dbo)
		{
			Debug.Trace.Engine.TraceInformation(
				"TriggerProtoDbObject: {0} - Updated {1}/{2}",
				ts, dbo.DbId.ToString(), dbo.Name);
		}
		void TryUpdate(BTriggerSystem ts, BTriggerCondition cond)
		{
			TriggerProtoDbObject dbo;
			if (!LookupTableContains(cond, out dbo))
			{
				var dbo_cond = new BTriggerProtoCondition(ts, cond);

				Conditions.DynamicAdd(dbo_cond, dbo_cond.Name);
				LookupTableAdd(dbo_cond);
			}
			else
			{
				int diff = dbo.CompareTo(ts, cond);
				if (diff < 0)
				{
					var dbo_cond = new BTriggerProtoCondition(ts, cond);
					LookupTable[GenerateHandle(cond)] = dbo_cond;
					TraceUpdate(ts, dbo_cond);
				}
			}
		}
		void TryUpdate(BTriggerSystem ts, BTriggerEffect effe)
		{
			TriggerProtoDbObject dbo;
			if (!LookupTableContains(effe, out dbo))
			{
				var dbo_effe = new BTriggerProtoEffect(ts, effe);

				Effects.DynamicAdd(dbo_effe, dbo_effe.Name);
				LookupTableAdd(dbo_effe);
			}
			else
			{
				int diff = dbo.CompareTo(ts, effe);
				if (diff < 0)
				{
					var dbo_effe = new BTriggerProtoEffect(ts, effe);
					LookupTable[GenerateHandle(effe)] = dbo_effe;
					TraceUpdate(ts, dbo_effe);
				}
			}
		}
		internal void UpdateFromGameData(BTriggerSystem ts)
		{
			lock (LookupTable)
			{
				foreach (var t in ts.Triggers)
				{
					foreach (var c in t.Conditions) TryUpdate(ts, c);
					foreach (var e in t.EffectsOnTrue) TryUpdate(ts, e);
					foreach (var e in t.EffectsOnFalse) TryUpdate(ts, e);
				}
			}
		}
		public void Save(string path, BDatabaseBase db)
		{
			using (var s = KSoft.IO.XmlElementStream.CreateForWrite(kXmlRootName))
			{
				s.InitializeAtRootElement();
				StreamXml(s, FA.Write, XML.BXmlSerializerInterface.GetNullInterface(db));

				s.Document.Save(path);
			}
		}
	};
}