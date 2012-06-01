using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using PhxUtil = PhxLib.Util;

namespace PhxLib.XML
{
	public class BTriggerScriptSerializer : BXmlSerializerInterface
	{
		static string GetFileExt(Engine.BTriggerScriptType type)
		{
			switch (type)
			{
				case Engine.BTriggerScriptType.TriggerScript: return ".triggerscript";
				case Engine.BTriggerScriptType.Ability: return ".ability";
				case Engine.BTriggerScriptType.Power: return ".power";

				default: throw new Debug.UnreachableException(type.ToString());
			}
		}
		static PhxEngine.XmlFileInfo GetFileInfo(FA mode, Engine.BTriggerScriptType type, string filename = null)
		{
			string root_name = Engine.BTriggerSystem.kXmlRootName;
			Engine.GameDirectory dir;
			var location = Engine.ContentStorage.Game;

			switch (type)
			{
				case Engine.BTriggerScriptType.TriggerScript:
					dir = Engine.GameDirectory.TriggerScripts;
					location = Engine.ContentStorage.UpdateOrGame; // TUs have only included updated TS files only
					break;
				case Engine.BTriggerScriptType.Scenario:
					dir = Engine.GameDirectory.Scenario;
					break;
				case Engine.BTriggerScriptType.Ability:
					dir = Engine.GameDirectory.AbilityScripts;
					break;
				case Engine.BTriggerScriptType.Power:
					dir = Engine.GameDirectory.PowerScripts;
					break;

				default: throw new Debug.UnreachableException(type.ToString());
			}

			return new PhxEngine.XmlFileInfo()
			{
				Location = location,
				Directory = dir,

				RootName = root_name,
				FileName = filename,

				Writable = mode == FA.Write,
			};
		}

		Engine.BDatabaseBase mDatabase;
		internal override Engine.BDatabaseBase Database { get { return mDatabase; } }

		public Engine.TriggerDatabase TriggerDb { get; private set; }

		public Engine.BScenario Scenario { get; private set; }

		public BTriggerScriptSerializer(PhxEngine phx, Engine.BScenario scnr = null)
		{
			Contract.Requires(phx != null);

			mDatabase = phx.Database;
			TriggerDb = phx.TriggerDb;
			Scenario = scnr;
		}

		#region IDisposable Members
		public override void Dispose()
		{
		}
		#endregion

		public class StreamTriggerScriptContext
		{
			public PhxEngine.XmlFileInfo FileInfo { get; set; }

			public Engine.BTriggerSystem Script { get; set; }

			public Engine.BTriggerSystem[] Scripts { get; set; }
		};
		public StreamTriggerScriptContext StreamTriggerScriptGetContext(FA mode, Engine.BTriggerScriptType type, string name)
		{
			return new StreamTriggerScriptContext
			{
				FileInfo = GetFileInfo(mode, type, name),
			};
		}
		public void StreamTriggerScript(KSoft.IO.XmlElementStream s, FA mode, StreamTriggerScriptContext ctxt)
		{
			var ts = ctxt.Script = new Engine.BTriggerSystem();

			ts.StreamXml(s, mode, this);
		}
		public void LoadScenarioScripts(KSoft.IO.XmlElementStream s, FA mode, StreamTriggerScriptContext ctxt)
		{
			//var ts = ctxt.Script = new Engine.BTriggerSystem();

			foreach (System.Xml.XmlElement e in s.Cursor)
			{
				if (e.Name != Engine.BTriggerSystem.kXmlRootName) continue;

				using (s.EnterCursorBookmark(e))
					new Engine.BTriggerSystem().StreamXml(s, mode, this);
			}
			//ts.StreamXml(s, mode, this);
		}
	};
}