using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using IO = System.IO;

namespace Serina
{
	/// <summary>Interaction logic for App.xaml</summary>
	public partial class App : Application
	{
		static bool PathsAreGood(string root, string update, string app_xml, 
			ref string out_root, ref string out_update, ref string out_app_xml)
		{
			bool exists = IO.Directory.Exists(root) && IO.Directory.Exists(update);

			if (exists)
			{
				out_root = root;
				out_update = update;
				out_app_xml = app_xml;
			}
			return exists;
		}
		static PhxLib.PhxEngine InitializeHaloWars(string game_root, string update_root)
		{
			var hw = PhxLib.PhxEngine.CreateForHaloWars(game_root, update_root);
			hw.Load();
			return hw;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
// 			string lc = System.Globalization.CultureInfo.CurrentCulture.Name;

			const string kGameRoot = @"C:\Users\Sean\Downloads\HW\Release\";
			const string kGameRootAlpha = @"C:\Users\Sean\Downloads\HW\phx_alpha\";
			const string kUpdateRoot = @"C:\Users\Sean\Downloads\HW\phx_tu6\";
			const string kXmlPhxApp = @"C:\Users\Sean\Downloads\HW\_Serina\";

			const string kGameRoot2 = @"C:\Kornner\Phx\Release\";
			const string kUpdateRoot2 = @"C:\Kornner\Phx\TU\phx_tu6\";
			const string kXmlPhxApp2 = @"C:\Kornner\Phx\";

			string game_root = null, update_root = null, app_xml = null;
			if (!PathsAreGood(kGameRoot, kUpdateRoot, kXmlPhxApp, ref game_root, ref update_root, ref app_xml) &&
				!PathsAreGood(kGameRoot2, kUpdateRoot2, kXmlPhxApp2, ref game_root, ref update_root, ref app_xml))
			{
				MessageBox.Show("Phx paths invalid on this machine", "Error", MessageBoxButton.OK);
				return;
			}

			base.OnStartup(e);

			var hw = InitializeHaloWars(game_root, update_root);

			#region Save
			using (var s = KSoft.IO.XmlElementStream.CreateForWrite("PhxLib", hw))
			{
				s.InitializeAtRootElement();
				hw.Database.StreamXml(s, System.IO.FileAccess.Write);
				s.Document.Save(app_xml + "PhxLib.xml");
			}
			#endregion
			#region Verify reading
			if (false) using (var s = new KSoft.IO.XmlElementStream(app_xml, System.IO.FileAccess.Read, hw))
			{
				s.InitializeAtRootElement();
				hw.Database.StreamXml(s, System.IO.FileAccess.Read);
			}
			#endregion
			#region dump objects in sorted DBID order
			if (false) using (var s = KSoft.IO.XmlElementStream.CreateForWrite("ObjectDBIDs"))
			{
				var objs = new List<PhxLib.Engine.BProtoObject>(hw.Database.Objects);
				objs.Sort((x, y) => x.DbId - y.DbId);

				s.InitializeAtRootElement();
				foreach (var obj in objs)
				{
					using (s.EnterCursorBookmark("Object"))
					{
						s.WriteAttribute("dbid", obj.DbId);
						s.WriteAttribute("name", obj.Name);
						s.WriteAttributeOptOnTrue("id", obj.Id, PhxLib.Util.kNotInvalidPredicate);
					}
				}
				s.Document.Save(app_xml + "ObjectDBIDs.xml");
			}
			#endregion
		}
	};
}