using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib
{
	partial class PhxEngine
	{
		void InitializeEngine(Engine.GameDirectories dirs)
		{
			Directories = dirs;
			Database = new HaloWars.BDatabase(this);
			TriggerDb = new Engine.TriggerDatabase();

			Database.InitializeTriggerScriptSerializer();
		}
		public static PhxEngine CreateForHaloWars(string gameRoot, string updateRoot)
		{
			var e = new PhxEngine();
			e.Build = PhxEngineBuild.Release;
			e.InitializeEngine(new Engine.GameDirectories(gameRoot, updateRoot));

			return e;
		}
		public static PhxEngine CreateForHaloWarsAlpha(string gameRoot)
		{
			var e = new PhxEngine();
			e.Build = PhxEngineBuild.Alpha;
			e.InitializeEngine(new Engine.GameDirectories(gameRoot));

			return e;
		}
	};
}