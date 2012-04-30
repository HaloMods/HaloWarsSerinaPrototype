using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Serina
{
	/// <summary>Interaction logic for App.xaml</summary>
	public partial class App : Application
	{
		static void PhxProtoTechsFixes()
		{
			const String inputUrl = "file:///c:/Kornner/Phx/Release/data/techs.xml";
			const String outputFile = "c:\\Kornner\\Phx\\Techs.Fixed.xml";
			const String xsltUri = "file:///c:/Kornner/Phx/PhxProtoTechsFixes.xsl";

			try
			{
				var transformer = new System.Xml.Xsl.XslCompiledTransform();
				transformer.Load(xsltUri, new System.Xml.Xsl.XsltSettings(true, true), null);


				Console.WriteLine();
				Console.WriteLine("XSLT starting.");
				transformer.Transform(inputUrl, outputFile);
				Console.WriteLine();
				Console.WriteLine("XSLT finished.");
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
				// Console.Error.WriteLine(e.StackTrace);
			}
		}
		static PhxLib.PhxEngine InitializeHaloWars(string game_root, string update_root)
		{
			var hw = PhxLib.PhxEngine.CreateForHaloWars(game_root, update_root);
			hw.Load();
			hw.LoadUpdates();
			return hw;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
// 			string lc = System.Globalization.CultureInfo.CurrentCulture.Name;
// 			const string kXmlGameData = @"C:\Users\Sean\Downloads\HW\_Serina\gamedata.xml";
// 			const string kXmlGameCivs = @"C:\Users\Sean\Downloads\HW\_Serina\civs.xml";
// 			const string kXmlGameLeaders = @"C:\Users\Sean\Downloads\HW\_Serina\leaders.xml";
// 			const string kXmlGameObjects = @"C:\Users\Sean\Downloads\HW\_Serina\objects.xml";
// 			const string kXmlGameSquads = @"C:\Users\Sean\Downloads\HW\_Serina\squads.xml";
// 			const string kXmlGameTech = @"C:\Users\Sean\Downloads\HW\_Serina\techs.xml";
// 			const string kXmlGameStringsEn = @"C:\Users\Sean\Downloads\HW\_Serina\locale-en_us.stringtable.xml";
// 			const string kXmlApp = @"C:\Users\Sean\Downloads\HW\_Serina\Serina.xml";
			const string kGameRoot = @"C:\Kornner\Phx\Release\";
			const string kUpdateRoot = @"C:\Kornner\Phx\TU\phx_tu6\";

			base.OnStartup(e);

			const string kXmlPhxApp = @"C:\Kornner\Phx\PhxLib.xml";

			var hw = InitializeHaloWars(kGameRoot, kUpdateRoot);

			#region Save
			using (var s = KSoft.IO.XmlElementStream.CreateForWrite("PhxLib", hw))
			{
				s.InitializeAtRootElement();
				hw.Database.StreamXml(s, System.IO.FileAccess.Write);
				s.Document.Save(kXmlPhxApp);
			}
			#endregion
			#region Verify reading
			if(false) using (var s = new KSoft.IO.XmlElementStream(kXmlPhxApp, System.IO.FileAccess.Read, hw))
			{
				s.InitializeAtRootElement();
				hw.Database.StreamXml(s, System.IO.FileAccess.Read);
			}
			#endregion
		}
	};
}