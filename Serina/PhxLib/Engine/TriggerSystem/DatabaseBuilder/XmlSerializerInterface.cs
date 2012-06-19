using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using PhxUtil = PhxLib.Util;

namespace PhxLib.Engine.TriggerSystem.DatabaseBuilder
{
	internal class DbBuilderSerializerInterface : XML.BXmlSerializerInterface
	{
		Engine.BDatabaseBase mDatabase;
		internal override Engine.BDatabaseBase Database { get { return mDatabase; } }

		public Engine.TriggerDatabase TriggerDb { get; private set; }

		public DbBuilderSerializerInterface(PhxEngine phx)
		{
			Contract.Requires(phx != null);

			mDatabase = phx.Database;
			TriggerDb = phx.TriggerDb;
		}

		#region IDisposable Members
		public override void Dispose() {}
		#endregion

		static void WaitUntilComplete(System.Threading.Tasks.ParallelLoopResult result)
		{
			while (!result.IsCompleted) System.Threading.Thread.Sleep(500);
		}

		void ParseTriggerScript(KSoft.IO.XmlElementStream s, FA mode)
		{
			var ts = new BTriggerSystem();

			ts.StreamXml(s, mode, this);
		}
		void ParseTriggerScriptSansSkrimishAI(KSoft.IO.XmlElementStream s, FA mode)
		{
			// This HW script has all the debug info stripped :o
			if (s.StreamName.EndsWith("skirmishai.triggerscript"))
				return;

			ParseTriggerScript(s, mode);
		}
		void ParseScenarioScripts(KSoft.IO.XmlElementStream s, FA mode)
		{
			foreach (System.Xml.XmlElement e in s.Cursor)
			{
				if (e.Name != BTriggerSystem.kXmlRootName) continue;

				using (s.EnterCursorBookmark(e))
					new BTriggerSystem().StreamXml(s, mode, this);
			}
		}

		void ParseTriggerScripts(PhxEngine e)
		{
			System.Threading.Tasks.ParallelLoopResult result;

			e.ReadDataFilesAsync(ContentStorage.Game, GameDirectory.TriggerScripts,
				BTriggerSystem.GetFileExtSearchPattern(BTriggerScriptType.TriggerScript),
				ParseTriggerScriptSansSkrimishAI, out result);

			WaitUntilComplete(result);

			e.ReadDataFilesAsync(ContentStorage.Update, GameDirectory.TriggerScripts,
				BTriggerSystem.GetFileExtSearchPattern(BTriggerScriptType.TriggerScript),
				ParseTriggerScript, out result);

			WaitUntilComplete(result);
		}
		void ParseAbilities(PhxEngine e)
		{
			System.Threading.Tasks.ParallelLoopResult result;

			e.ReadDataFilesAsync(ContentStorage.Game, GameDirectory.AbilityScripts, 
				BTriggerSystem.GetFileExtSearchPattern(BTriggerScriptType.Ability),
				ParseTriggerScript, out result);

			WaitUntilComplete(result);
		}
		void ParsePowers(PhxEngine e)
		{
			System.Threading.Tasks.ParallelLoopResult result;

			e.ReadDataFilesAsync(ContentStorage.Game, GameDirectory.PowerScripts,
				BTriggerSystem.GetFileExtSearchPattern(BTriggerScriptType.Power),
				ParseTriggerScript, out result);

			WaitUntilComplete(result);
		}
		void ParseScenarios(PhxEngine e)
		{
			System.Threading.Tasks.ParallelLoopResult result;

			e.ReadDataFilesAsync(ContentStorage.Game, GameDirectory.Scenario,
				"*.scn",
				ParseScenarioScripts, out result);

			WaitUntilComplete(result);
		}

		public void ParseScriptFiles()
		{
			var e = Database.Engine;

			ParseTriggerScripts(e);
			ParseAbilities(e);
			ParsePowers(e);
			ParseScenarios(e);
		}
	};
}