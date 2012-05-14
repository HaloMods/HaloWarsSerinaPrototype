using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.HaloWars
{
	using PhxLib.Engine;

	partial class BDatabase
	{
		protected override XML.BDatabaseXmlSerializerBase NewXmlSerializer()
		{
			return new XmlSerializer(this);
		}

		partial class XmlSerializer : XML.BDatabaseXmlSerializerBase
		{
			BDatabase mDatabase;

			internal override BDatabaseBase Database { get { return mDatabase; } }

			public XmlSerializer(BDatabase db)
			{
				mDatabase = db;
			}

			protected override void PostStreamXml(FA mode)
			{
				if (mode == FA.Read)
				{
					mDatabase.SetupDBIDs();
				}
			}
		};
	};
}