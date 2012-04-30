using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib
{
	partial class PhxEngine
	{
		public static PhxEngine CreateForHaloWars(string game_root, string update_root)
		{
			var e = new PhxEngine();
			e.Directories = new Engine.GameDirectories(game_root, update_root);
			e.Database = new HaloWars.BDatabase(e);

			return e;
		}
	};
}