using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class Util
	{
		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BListArray<T> list, BListXmlParams @params)
			where T : IO.IPhxXmlStreamable, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BListArrayXmlSerializer<T>(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}
	};

	public class BListXmlParams : BCollectionXmlParams
	{
		public /*readonly*/ string DataName;

		#region Flags
		[Contracts.Pure]
		public bool InternDataNames { get { return HasFlag(BCollectionXmlParamsFlags.InternDataNames); } }
		[Contracts.Pure]
		public bool UseInnerTextForData { get { return HasFlag(BCollectionXmlParamsFlags.UseInnerTextForData); } }
		[Contracts.Pure]
		public bool UseElementForData { get { return HasFlag(BCollectionXmlParamsFlags.UseElementForData); } }
		[Contracts.Pure]
		public bool RequiresDataNamePreloading { get { return HasFlag(BCollectionXmlParamsFlags.RequiresDataNamePreloading); } }
		[Contracts.Pure]
		public bool SupportsUpdating { get { return HasFlag(BCollectionXmlParamsFlags.SupportsUpdating); } }
		#endregion

		public BListXmlParams() { }
		/// <summary>Sets RootName to plural of ElementName and sets UseInnerTextForData</summary>
		/// <param name="element_name"></param>
		/// <param name="additional_flags"></param>
		public BListXmlParams(string element_name, BCollectionXmlParamsFlags additional_flags = 0) : base(element_name)
		{
			Flags = additional_flags;
			Flags |= BCollectionXmlParamsFlags.UseInnerTextForData;
		}

		public void StreamDataName(KSoft.IO.XmlElementStream s, FA mode, ref string name)
		{
			BCollectionXmlParams.StreamValue(s, mode, DataName, ref name,
				UseInnerTextForData, UseElementForData, InternDataNames,
				HasFlag(BCollectionXmlParamsFlags.ToLowerDataNames));
		}
		public void StreamIsUpdateAttr(KSoft.IO.XmlElementStream s, FA mode, ref bool is_update)
		{
			s.StreamAttributeOpt(mode, "update", ref is_update, PhxLib.Util.kNotFalsePredicate);
		}
	};

	internal abstract class BListXmlSerializerBase<T>
	{
		public abstract BListXmlParams Params { get; }
		public abstract Collections.BListBase<T> List { get; }

		#region IXmlElementStreamable Members
		protected abstract void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration);
		protected abstract void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, T data);

		protected virtual void ReadXmlDetermineListSize(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			int xml_node_count = s.Cursor.ChildNodes.Count;
			if (List.Capacity < xml_node_count)
				List.Capacity = xml_node_count;
		}
		protected virtual void ReadXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			ReadXmlDetermineListSize(s, xs);

			int x = 0;
			foreach (XmlNode n in s.Cursor.ChildNodes)
			{
				if (n.Name != Params.ElementName) continue;

				using (s.EnterCursorBookmark(n as XmlElement))
					ReadXml(s, xs, x++);
			}

			List.OptimizeStorage();
		}
		protected virtual void WriteXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			foreach (T data in List)
				using (s.EnterCursorBookmark(Params.ElementName))
					WriteXml(s, xs, data);
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			using (s.EnterCursorBookmark(mode, Params.GetOptionalRootName()))
			{
					 if (mode == FA.Read)	ReadXmlNodes(s, xs);
				else if (mode == FA.Write)	WriteXmlNodes(s, xs);
			}
		}
		#endregion
	};

	internal class BListArrayXmlSerializer<T> : BListXmlSerializerBase<T>
		where T : IO.IPhxXmlStreamable, new()
	{
		BListXmlParams mParams;
		Collections.BListArray<T> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<T> List { get { return mList; } }

		public BListArrayXmlSerializer(BListXmlParams @params, Collections.BListArray<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			T item = new T();
			item.StreamXml(s, FA.Read, xs);

			List.Add(item);
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, T data)
		{
			data.StreamXml(s, FA.Write, xs);
		}
		#endregion
	};
}