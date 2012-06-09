using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BTriggerParamType
	{
		Invalid = -1,
		Input,
		Output,
	};

	public class BTriggerParam : IO.IPhxXmlStreamable, IComparable<BTriggerParam>, IEqualityComparer<BTriggerParam>
	{
		static readonly BTriggerParam kInvalid = new BTriggerParam();
		public bool IsInvalid { get { return object.ReferenceEquals(this, kInvalid); } }

		#region Xml constants
		public static readonly Collections.BListExplicitIndexParams<BTriggerParam> kBListExplicitIndexParams = new
			Collections.BListExplicitIndexParams<BTriggerParam>(10)
			{
				kTypeGetInvalid = () => kInvalid
			};
		public static readonly XML.BListExplicitIndexXmlParams<BTriggerParam> kBListExplicitIndexXmlParams = new
			XML.BListExplicitIndexXmlParams<BTriggerParam>(/*null*/"Param", kXmlAttrSigId);

		const string kXmlAttrType = "Type";
		const string kXmlAttrSigId = "SigID";
		const string kXmlAttrOptional = "Optional";
		#endregion

		#region Properties
		BTriggerParamType mType = BTriggerParamType.Invalid;
		public BTriggerParamType Type { get { return mType; } }

		string mName;
		public string Name { get { return mName; } }

		int mSigID = Util.kInvalidInt32;
		public int SigID { get { return mSigID; } }

		BTriggerVarType mVarType = BTriggerVarType.None;
		public BTriggerVarType VarType { get { return mVarType; } }

		bool mOptional;
		public bool Optional { get { return mOptional; } }
		#endregion

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			//if (mode == FA.Read) s.ReadCursorName(ref mType);
			s.StreamAttributeEnum(mode, kXmlAttrType, ref mType);
			s.StreamAttribute(mode, kXmlAttrSigId, ref mSigID);
			s.StreamAttribute(mode, DatabaseNamedObject.kXmlAttrNameN, ref mName);
			s.StreamAttributeOpt(mode, kXmlAttrOptional, ref mOptional, Util.kNotFalsePredicate);
			s.StreamCursorEnum(mode, ref mVarType);
		}
		#endregion

		#region IComparable<BTriggerParam> Members
		public int CompareTo(BTriggerParam other)
		{
			return this.mSigID - other.mSigID;
		}
		#endregion

		#region IEqualityComparer<BTriggerParam> Members
		public bool Equals(BTriggerParam x, BTriggerParam y)
		{
			return x.mSigID == y.mSigID;
		}

		public int GetHashCode(BTriggerParam obj)
		{
			return mSigID;
		}
		#endregion

		public static Collections.BListExplicitIndex<BTriggerParam> BuildDefinition(
			BTriggerSystem root, Collections.BListExplicitIndex<BTriggerArg> args)
		{
			var p = new Collections.BListExplicitIndex<BTriggerParam>(kBListExplicitIndexParams);
			p.ResizeCount(args.Count);

			foreach (var arg in args)
			{
				if (arg.IsInvalid) continue;

				var param = new BTriggerParam();
				param.mType = arg.Type;
				param.mName = arg.Name;
				param.mSigID = arg.SigID;
				param.mOptional = arg.Optional;
				param.mVarType = arg.GetVarType(root);

				p[param.mSigID-1] = param;
			}

			p.OptimizeStorage();
			return p;
		}
	};

	public class BTriggerArg : IO.IPhxXmlStreamable, IComparable<BTriggerArg>, IEqualityComparer<BTriggerArg>
	{
		static readonly BTriggerArg kInvalid = new BTriggerArg();
		public bool IsInvalid { get { return object.ReferenceEquals(this, kInvalid); } }

		#region Xml constants
		public static readonly Collections.BListExplicitIndexParams<BTriggerArg> kBListExplicitIndexParams = new
			Collections.BListExplicitIndexParams<BTriggerArg>(10)
			{
				kTypeGetInvalid = () => kInvalid
			};
		public static readonly XML.BListExplicitIndexXmlParams<BTriggerArg> kBListExplicitIndexXmlParams = new
			XML.BListExplicitIndexXmlParams<BTriggerArg>(null, kXmlAttrSigId);

		const string kXmlAttrSigId = "SigID";
		const string kXmlAttrOptional = "Optional";
		#endregion

		BTriggerParamType mType = BTriggerParamType.Invalid; // TODO: temporary!
		public BTriggerParamType Type { get { return mType; } }

		string mName; // TODO: temporary!
		public string Name { get { return mName; } }

		int mSigID = Util.kInvalidInt32;
		public int SigID { get { return mSigID; } }

		bool mOptional; // TODO: temporary!
		public bool Optional { get { return mOptional; } }

		int mVarID = Util.kInvalidInt32;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			if (mode == FA.Read) s.ReadCursorName(ref mType);
			s.StreamAttribute(mode, kXmlAttrSigId, ref mSigID);
			s.StreamAttribute(mode, DatabaseNamedObject.kXmlAttrNameN, ref mName);
			s.StreamAttribute(mode, kXmlAttrOptional, ref mOptional);
			s.StreamCursor(mode, ref mVarID);
		}
		#endregion

		#region IComparable<BTriggerArg> Members
		public int CompareTo(BTriggerArg other)
		{
			return this.mSigID - other.mSigID;
		}
		#endregion

		#region IEqualityComparer<BTriggerArg> Members
		public bool Equals(BTriggerArg x, BTriggerArg y)
		{
			return x.mSigID == y.mSigID;
		}

		public int GetHashCode(BTriggerArg obj)
		{
			return mSigID;
		}
		#endregion

		public BTriggerVarType GetVarType(BTriggerSystem root)
		{
			return root.GetVar(mVarID).Type;
		}
	};

	public class BTrigger : TriggerScriptIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Trigger")
		{
			DataName = DatabaseNamedObject.kXmlAttrNameN,
		};

		const string kXmlAttrActive = "Active";
		const string kXmlAttrEvaluateFrequency = "EvaluateFrequency";
		const string kXmlAttrEvalLimit = "EvalLimit";
		const string kXmlAttrConditionalTrigger = "ConditionalTrigger";

		const string kXmlAttrCommentOut = "CommentOut";
		const string kXmlAttrX = "X";
		const string kXmlAttrY = "Y";
		const string kXmlAttrGroupID = "GroupID";
		const string kXmlAttrTemplateID = "TemplateID";
		#endregion

		bool mActive;

		int mEvaluateFrequency;

		int mEvalLimit;

		bool mConditionalTrigger;

		public Collections.BListAutoId<BTriggerCondition> Conditions { get; private set; }
		/// <summary>True if <see cref="Conditions"/> are OR, false if they're AND</summary>
		public bool OrConditions { get; set; }
		public Collections.BListAutoId<BTriggerEffect> EffectsOnTrue { get; private set; }
		public Collections.BListAutoId<BTriggerEffect> EffectsOnFalse { get; private set; }

		public BTrigger()
		{
			Conditions = new Collections.BListAutoId<BTriggerCondition>();
			EffectsOnTrue = new Collections.BListAutoId<BTriggerEffect>();
			EffectsOnFalse = new Collections.BListAutoId<BTriggerEffect>();
		}

		void StreamConditions(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			var k_AND_params = BTriggerCondition.kBListXmlParams_And;

			if (mode == FA.Read)
			{
				if (OrConditions = !s.ElementsExists(k_AND_params.RootName))
					XML.Util.Serialize(s, mode, xs, Conditions, BTriggerCondition.kBListXmlParams_Or);
				else
					XML.Util.Serialize(s, mode, xs, Conditions, k_AND_params);
			}
			else if (mode == FA.Write)
			{
				// Even if there are no conditions, the runtime expects there to be an empty And tag :|
				// Well, technically we could use an empty Or tag as well, but it wouldn't be consistent
				// with the engine. The runtime will assume the the TS is bad if neither tag is present
				if (Conditions.Count == 0)
					s.WriteElement(k_AND_params.RootName);
				else
					XML.Util.Serialize(s, mode, xs, Conditions, 
						OrConditions ? BTriggerCondition.kBListXmlParams_Or : k_AND_params);
			}
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamAttribute(mode, kXmlAttrActive, ref mActive);
			s.StreamAttribute(mode, kXmlAttrEvaluateFrequency, ref mEvaluateFrequency);
			s.StreamAttribute(mode, kXmlAttrEvalLimit, ref mEvalLimit);
			s.StreamAttribute(mode, kXmlAttrConditionalTrigger, ref mConditionalTrigger);

			// These tags must exist no matter what :|
			using (s.EnterCursorBookmark(mode, BTriggerCondition.kXmlRootName))
				StreamConditions(s, mode, xs);

			using (s.EnterCursorBookmark(mode, BTriggerEffect.kXmlRootName_OnTrue))
				XML.Util.Serialize(s, mode, xs, EffectsOnTrue, BTriggerEffect.kBListXmlParams);

			using (s.EnterCursorBookmark(mode, BTriggerEffect.kXmlRootName_OnFalse))
				XML.Util.Serialize(s, mode, xs, EffectsOnFalse, BTriggerEffect.kBListXmlParams);
		}
	};

	public class BTriggerTemplateMapping
	{
		#region Xml constants
		const string kBindNameTriggerActive = "Activate";
		const string kBindNameTriggerEffectsOnTrue = "Effect.True";
		const string kBindNameTriggerEffectsOnFalse = "Effect.False";

		const string kXmlElementInputMapping = "InputMapping";
		const string kXmlElementOutputMapping = "OutputMapping";
		const string kXmlElementTriggerInput = "TriggerInput";
		const string kXmlElementTriggerOutput = "TriggerOutput";

		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "TriggerMappings",
			ElementName = "TriggerTemplateMapping",
			DataName = DatabaseNamedObject.kXmlAttrNameN,
		};

		// ID, Image, X, Y, SizeX, SizeY, GroupID, CommentOut, Obsolete, DoNotUse
		#endregion
	};

	public class BTrigerUINoteNodes
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "NoteNodes",
			ElementName = "NoteNodeXml",
		};

		// X, Y, Width, Height, GroupID

		const string kXmlElementTitle = "Title";
		const string kXmlElementDescription = "Description";
		#endregion
	};
	public class BTriggerUIGroups
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "Groups",
			ElementName = "GroupUI",
		};

		// X, Y, iX, iY, oX, oY, Width, Height, GroupID, InternalGroupID

		const string kXmlElementTitle = "Title";
		#endregion
	};
	public class BTriggerUIData
	{
		#region Xml constants
		public const string kXmlRootName = "UIData";
		#endregion
	};

	public class BTriggerEditorData
	{
		#region Xml constants
		public const string kXmlRootName = "EditorData";
		#endregion
	};
}