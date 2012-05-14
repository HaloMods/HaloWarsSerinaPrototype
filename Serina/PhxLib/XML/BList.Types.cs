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
		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BTypeNames list, BListXmlParams @params, bool force_no_root_element_streaming = false)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(true);
			var xs = new BTypeNamesXmlSerializer(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(false);
		}

		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BTypeValues<T> list, BTypeValuesXmlParams<T> @params)
			where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BTypeValuesXmlSerializer<T>(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}

		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BTypeValuesInt32 list, BTypeValuesXmlParams<int> @params)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BTypeValuesInt32XmlSerializer(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}
		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BTypeValuesSingle list, BTypeValuesXmlParams<float> @params,
			string attr_name = null)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			BTypeValuesXmlSerializerBase<float> xs;
			if (attr_name == null)	xs = new BTypeValuesSingleXmlSerializer(@params, list);
			else					xs = new BTypeValuesSingleAttrHackXmlSerializer(@params, list, attr_name);
			{
				xs.StreamXml(s, mode, db);
			}
		}
		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BTypeValuesString list, BTypeValuesXmlParams<string> @params)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BTypeValuesStringXmlSerializer(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}
	};

	internal class BTypeNamesXmlSerializer : BListXmlSerializerBase<string>
	{
		BListXmlParams mParams;
		Collections.BTypeNames mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<string> List { get { return mList; } }

		public BTypeNamesXmlSerializer(BListXmlParams @params, Collections.BTypeNames list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			string name = null;
			mParams.StreamDataName(s, FA.Read, ref name);

			mList.Add(name);
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, string name)
		{
			mParams.StreamDataName(s, FA.Write, ref name);
		}
		#endregion
	};

	public class BTypeValuesXmlParams<T> : BListExplicitIndexXmlParams<T>
	{
		/// <summary>Sets ElementName and DataName (which defaults to XML attribute usage)</summary>
		/// <param name="element_name"></param>
		/// <param name="type_name">Name of the xml node which represents the type (enum) value</param>
		public BTypeValuesXmlParams(string element_name, string type_name, BCollectionXmlParamsFlags flags = 0)
		{
			ElementName = element_name;
			DataName = type_name;
			Flags = flags;
		}
	};

	internal abstract class BTypeValuesXmlSerializerBase<T> : BListExplicitIndexXmlSerializerBase<T>
	{
		Collections.BTypeValuesBase<T> mList;

		public override Collections.BListExplicitIndexBase<T> ListExplicitIndex { get { return mList; } }

		public BTypeValuesXmlSerializerBase(BTypeValuesXmlParams<T> @params, Collections.BTypeValuesBase<T> list) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mList = list;
		}

		#region IXmlElementStreamable Members
		protected override int ReadExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			string name = null;
			Params.StreamDataName(s, FA.Read, ref name);

			int index = mList.TypeValuesParams.kGetProtoEnumFromDB(xs.Database).GetMemberId(name);

			return index;
		}
		protected override void WriteExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int index)
		{
			string name = mList.TypeValuesParams.kGetProtoEnumFromDB(xs.Database).GetMemberName(index);

			Params.StreamDataName(s, FA.Write, ref name);
		}

		/// <summary>Not Implemented</summary>
		/// <exception cref="NotImplementedException" />
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration) { throw new NotImplementedException(); }
		/// <summary>Not Implemented</summary>
		/// <exception cref="NotImplementedException" />
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, T data) { throw new NotImplementedException(); }
		#endregion
	};
	internal class BTypeValuesXmlSerializer<T> : BTypeValuesXmlSerializerBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		public BTypeValuesXmlSerializer(BTypeValuesXmlParams<T> @params, Collections.BTypeValues<T> list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			ListExplicitIndex.InitializeItem(index);
			T data = new T();
			data.StreamXml(s, FA.Read, xs);
			ListExplicitIndex[index] = data;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, T data)
		{
			data.StreamXml(s, FA.Write, xs);
		}
		#endregion
	};

	internal class BTypeValuesInt32XmlSerializer : BTypeValuesXmlSerializerBase<int>
	{
		public BTypeValuesInt32XmlSerializer(BTypeValuesXmlParams<int> @params, Collections.BTypeValuesInt32 list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			ListExplicitIndex.InitializeItem(index);
			int value = 0;
			s.ReadCursor(KSoft.NumeralBase.Decimal, ref value);
			ListExplicitIndex[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int data)
		{
			s.WriteCursor(KSoft.NumeralBase.Decimal, data);
		}
		#endregion
	};
	internal class BTypeValuesSingleXmlSerializer : BTypeValuesXmlSerializerBase<float>
	{
		public BTypeValuesSingleXmlSerializer(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			ListExplicitIndex.InitializeItem(index);
			float value = 0;
			s.ReadCursor(ref value);
			ListExplicitIndex[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, float data)
		{
			s.WriteCursor(data);
		}
		#endregion
	};
	internal class BTypeValuesStringXmlSerializer : BTypeValuesXmlSerializerBase<string>
	{
		public BTypeValuesStringXmlSerializer(BTypeValuesXmlParams<string> @params, Collections.BTypeValuesString list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			ListExplicitIndex.InitializeItem(index);
			string value = null;
			s.ReadCursor(ref value);
			ListExplicitIndex[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, string data)
		{
			s.WriteCursor(data);
		}
		#endregion
	};

	/// <summary>
	/// Lame hack for type value maps which store their type name in the InnerText and the value in a fucking attribute
	/// </summary>
	internal class BTypeValuesSingleAttrHackXmlSerializer : BTypeValuesXmlSerializerBase<float>
	{
		readonly string kAttrName;

		public BTypeValuesSingleAttrHackXmlSerializer(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list, string attr_name)
			: base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
			Contract.Requires<ArgumentNullException>(attr_name != null);

			kAttrName = attr_name;
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			ListExplicitIndex.InitializeItem(index);
			float value = 0;
			s.ReadAttribute(kAttrName, ref value);
			ListExplicitIndex[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, float data)
		{
			s.WriteAttribute(kAttrName, data);
		}
		#endregion
	};
}