using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum TriggerScriptObjectType
	{
		Invalid = -1,
		Group,
		Var,
		Trigger,
		Condition,
		Effect,
		TemplateMapping,
	};

	public enum BTriggerScriptType
	{
		Invalid = -1,
		TriggerScript,
		Scenario,
		Ability,
		Power,
	};

	public interface IEditorVisualObject
	{
		string Name { get; }

		int X { get; set; }
		int Y { get; set; }
	};

	/// <summary>Explicitly ID'd script objects</summary>
	public abstract class TriggerScriptIdObject : Collections.BListAutoIdObject
	{
		#region Xml constants
		const string kXmlAttrId = "ID"; // EditorID
		#endregion

		int mID = Util.kInvalidInt32;
		public int ID { get { return mID; } }

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrId, ref mID);
		}
	};
	/// <summary>Script objects which can be "commented" out</summary>
	public abstract class TriggerScriptCodeObject : TriggerScriptIdObject
	{
		#region Xml constants
		const string kXmlAttrCommentOut = "CommentOut";
		#endregion

		bool mCommentOut;
		public bool CommentOut { get { return mCommentOut; } }

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamAttribute(mode, kXmlAttrCommentOut, ref mCommentOut);
		}
	};
	/// <summary>Script objects which map to a editor-only database</summary>
	public abstract class TriggerScriptDbObject : TriggerScriptCodeObject
	{
		#region Xml constants
		protected const string kXmlAttrType = "Type";
		const string kXmlAttrDbId = "DBID";
		const string kXmlAttrVersion = "Version";
		#endregion

//		string mTypeStr; // TODO: temporary!
		int mDbId = Util.kInvalidInt32;
		public int DbId { get { return mDbId; } }

		int mVersion = Util.kInvalidInt32;
		public int Version { get { return mVersion; } }

		protected void StreamType<TTypeEnum>(KSoft.IO.XmlElementStream s, FA mode, ref TTypeEnum type)
			where TTypeEnum : struct, IFormattable
		{
			s.StreamAttributeEnum(mode, kXmlAttrType, ref type);
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamAttribute(mode, kXmlAttrDbId, ref mDbId);
			s.StreamAttribute(mode, kXmlAttrVersion, ref mVersion);
			// Stream it last, so when we save it ourselves, the (relatively) fixed width stuff comes first
//			XML.Util.StreamInternString(s, mode, kXmlAttrType, ref mTypeStr, false);
		}
	};
	public abstract class TriggerScriptDbObjectWithArgs : TriggerScriptDbObject
	{
		public Collections.BListExplicitIndex<BTriggerArg> Args { get; private set; }

		protected TriggerScriptDbObjectWithArgs()
		{
			Args = new Collections.BListExplicitIndex<BTriggerArg>(BTriggerArg.kBListExplicitIndexParams);
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			XML.Util.Serialize(s, mode, xs, Args, BTriggerArg.kBListExplicitIndexXmlParams);
		}
	};

	public class BTriggerGroup : TriggerScriptIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "TriggerGroups",
			ElementName = "Group",
			DataName = DatabaseNamedObject.kXmlAttrNameN,
		};

		const string kXmlAttrId = "ID";
		#endregion

		//string mValue;

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			//s.StreamCursor(mode, ref mValue);
		}
	};

	public class BTriggerSystem : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public const string kXmlRootName = "TriggerSystem";

		const string kXmlAttrType = "Type";
		const string kXmlAttrNextTriggerVar = "NextTriggerVarID";
		const string kXmlAttrNextTrigger = "NextTriggerID";
		const string kXmlAttrNextCondition = "NextConditionID";
		const string kXmlAttrNextEffect = "NextEffectID";
		const string kXmlAttrExternal = "External";
		#endregion

		#region File Util
		public static string GetFileExt(BTriggerScriptType type)
		{
			switch (type)
			{
				case BTriggerScriptType.TriggerScript: return ".triggerscript";
				case BTriggerScriptType.Ability: return ".ability";
				case BTriggerScriptType.Power: return ".power";

				default: throw new Debug.UnreachableException(type.ToString());
			}
		}
		public static string GetFileExtSearchPattern(BTriggerScriptType type)
		{
			switch (type)
			{
				case BTriggerScriptType.TriggerScript: return "*.triggerscript";
				case BTriggerScriptType.Ability: return "*.ability";
				case BTriggerScriptType.Power: return "*.power";

				default: throw new Debug.UnreachableException(type.ToString());
			}
		}
		#endregion

		public BTriggerSystem Owner { get; private set; }

		string mName;
		public string Name { get { return mName; } }
		public override string ToString() { return mName; }

		BTriggerScriptType mType;
		int mNextTriggerVarID = Util.kInvalidInt32;
		int mNextTriggerID = Util.kInvalidInt32;
		int mNextConditionID = Util.kInvalidInt32;
		int mNextEffectID = Util.kInvalidInt32;
		bool mExternal;

		public Collections.BListAutoId<BTriggerGroup> Groups { get; private set; }

		public Collections.BListAutoId<BTriggerVar> Vars { get; private set; }
		public Collections.BListAutoId<BTrigger> Triggers { get; private set; }

		public BTriggerEditorData EditorData { get; private set; }

		public BTriggerSystem()
		{
			Groups = new Collections.BListAutoId<BTriggerGroup>();

			Vars = new Collections.BListAutoId<BTriggerVar>();
			Triggers = new Collections.BListAutoId<BTrigger>();
		}

		#region Database interfaces
		Dictionary<int, BTriggerGroup> m_dbiGroups;
		Dictionary<int, BTriggerVar> m_dbiVars;
		Dictionary<int, BTrigger> m_dbiTriggers;

		static void BuildDictionary<T>(out Dictionary<int, T> dic, Collections.BListAutoId<T> list)
			where T : TriggerScriptIdObject, new()
		{
			dic = new Dictionary<int, T>(list.Count);

			foreach (var item in list)
				dic.Add(item.ID, item);
		}

		public BTriggerVar GetVar(int var_id)
		{
			BTriggerVar var;
			m_dbiVars.TryGetValue(var_id, out var);
			
			return var;
		}
		#endregion

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, DatabaseNamedObject.kXmlAttrNameN, ref mName);
			s.StreamAttributeEnum(mode, kXmlAttrType, ref mType);
			s.StreamAttribute(mode, kXmlAttrNextTriggerVar, ref mNextTriggerVarID);
			s.StreamAttribute(mode, kXmlAttrNextTrigger, ref mNextTriggerID);
			s.StreamAttribute(mode, kXmlAttrNextCondition, ref mNextConditionID);
			s.StreamAttribute(mode, kXmlAttrNextEffect, ref mNextEffectID);
			s.StreamAttribute(mode, kXmlAttrExternal, ref mExternal);

			using (s.EnterOwnerBookmark(this))
			{
				XML.Util.Serialize(s, mode, xs, Groups, BTriggerGroup.kBListXmlParams);
				if (mode == FA.Read) BuildDictionary(out m_dbiGroups, Groups);

				XML.Util.Serialize(s, mode, xs, Vars, BTriggerVar.kBListXmlParams);
				if (mode == FA.Read) BuildDictionary(out m_dbiVars, Vars);
				XML.Util.Serialize(s, mode, xs, Triggers, BTrigger.kBListXmlParams);
				if (mode == FA.Read) BuildDictionary(out m_dbiTriggers, Triggers);
			}

			(xs as XML.BTriggerScriptSerializer).TriggerDb.UpdateFromGameData(this);
		}
		#endregion
	};
}