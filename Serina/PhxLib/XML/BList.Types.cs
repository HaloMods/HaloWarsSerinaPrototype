using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class BDatabaseXmlSerializerBase
	{
#if !NO_TLS_STREAMING
		internal static System.Threading.ThreadLocal<BTypeNamesXmlSerializer> sBTypeNamesXmlSerializer =
			new System.Threading.ThreadLocal<BTypeNamesXmlSerializer>(BTypeNamesXmlSerializer.kNewFactory);

		internal class _BTypeValues<T>
			where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
		{
			internal static System.Threading.ThreadLocal<BTypeValuesXmlSerializer<T>> sXmlSerializer =
				new System.Threading.ThreadLocal<BTypeValuesXmlSerializer<T>>(BTypeValuesXmlSerializer<T>.kNewFactory);
		};
		internal static System.Threading.ThreadLocal<BTypeValuesInt32XmlSerializer> sBTypeValuesInt32XmlSerializer =
			new System.Threading.ThreadLocal<BTypeValuesInt32XmlSerializer>(BTypeValuesInt32XmlSerializer.kNewFactory);
		internal static System.Threading.ThreadLocal<BTypeValuesSingleXmlSerializer> sBTypeValuesSingleXmlSerializer =
			new System.Threading.ThreadLocal<BTypeValuesSingleXmlSerializer>(BTypeValuesSingleXmlSerializer.kNewFactory);
		internal static System.Threading.ThreadLocal<BTypeValuesStringXmlSerializer> sBTypeValuesStringXmlSerializer =
			new System.Threading.ThreadLocal<BTypeValuesStringXmlSerializer>(BTypeValuesStringXmlSerializer.kNewFactory);

		internal static System.Threading.ThreadLocal<BTypeValuesSingleAttrHackXmlSerializer> sBTypeValuesSingleAttrHackXmlSerializer =
			new System.Threading.ThreadLocal<BTypeValuesSingleAttrHackXmlSerializer>(BTypeValuesSingleAttrHackXmlSerializer.kNewFactory);

		internal static System.Threading.ThreadLocal<BCostTypeValuesSingleAttrHackXmlSerializer> sBCostTypeValuesSingleAttrHackXmlSerializer =
			new System.Threading.ThreadLocal<BCostTypeValuesSingleAttrHackXmlSerializer>(BCostTypeValuesSingleAttrHackXmlSerializer.kNewFactory);
#endif
	};

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
			using(var xs = 
#if NO_TLS_STREAMING
				new BTypeNamesXmlSerializer(@params, list)
#else
				BDatabaseXmlSerializerBase.sBTypeNamesXmlSerializer.Value.Reset(@params, list)
#endif
			)
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

			using(var xs = 
#if NO_TLS_STREAMING
				new BTypeValuesXmlSerializer<T>(@params, list)
#else
				BDatabaseXmlSerializerBase._BTypeValues<T>.sXmlSerializer.Value.Reset(@params, list)
#endif
			)
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

			using(var xs = 
#if NO_TLS_STREAMING
				new BTypeValuesInt32XmlSerializer(@params, list)
#else
				BDatabaseXmlSerializerBase.sBTypeValuesInt32XmlSerializer.Value.Reset(@params, list)
#endif
			)
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
#if NO_TLS_STREAMING
			if (attr_name == null)	xs = new BTypeValuesSingleXmlSerializer(@params, list);
			else					xs = new BTypeValuesSingleAttrHackXmlSerializer(@params, list, attr_name);
#else
			if (attr_name == null)	xs = BDatabaseXmlSerializerBase.sBTypeValuesSingleXmlSerializer.Value.Reset(@params, list);
			else					xs = BDatabaseXmlSerializerBase.sBTypeValuesSingleAttrHackXmlSerializer.Value.Reset(@params, list, attr_name);
#endif
			using(xs)
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

			using(var xs = 
#if NO_TLS_STREAMING
				new BTypeValuesStringXmlSerializer(@params, list)
#else
				BDatabaseXmlSerializerBase.sBTypeValuesStringXmlSerializer.Value.Reset(@params, list)
#endif
			)
			{
				xs.StreamXml(s, mode, db);
			}
		}

		public static void SerializeCostHack(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase db,
			Collections.BTypeValuesSingle list)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);

			using(var xs = 
#if NO_TLS_STREAMING
				new BCostTypeValuesSingleAttrHackXmlSerializer(list)
#else
				BDatabaseXmlSerializerBase.sBCostTypeValuesSingleAttrHackXmlSerializer.Value.Reset(list)
#endif
			)
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

#if NO_TLS_STREAMING
		public BTypeNamesXmlSerializer(BListXmlParams @params, Collections.BTypeNames list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeNamesXmlSerializer> kNewFactory = () => new BTypeNamesXmlSerializer();
		BTypeNamesXmlSerializer() { }

		public BTypeNamesXmlSerializer Reset(BListXmlParams @params, Collections.BTypeNames list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;

			return this;
		}

		protected override void FinishTlsStreaming()
		{
			mParams = null;
			mList = null;
		}
#endif
		#endregion

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

#if NO_TLS_STREAMING
		public BTypeValuesXmlSerializerBase(BTypeValuesXmlParams<T> @params, Collections.BTypeValuesBase<T> list) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		protected BTypeValuesXmlSerializerBase() { }

		protected void Reset(BTypeValuesXmlParams<T> @params, Collections.BTypeValuesBase<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params);
			mList = list;
		}

		protected override void FinishTlsStreaming()
		{
			base.FinishTlsStreaming();
			mList = null;
		}
#endif
		#endregion

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
#if NO_TLS_STREAMING
		public BTypeValuesXmlSerializer(BTypeValuesXmlParams<T> @params, Collections.BTypeValues<T> list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeValuesXmlSerializer<T>> kNewFactory = () => new BTypeValuesXmlSerializer<T>();
		BTypeValuesXmlSerializer() { }

		public BTypeValuesXmlSerializer<T> Reset(BTypeValuesXmlParams<T> @params, Collections.BTypeValues<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params, list);

			return this;
		}
#endif
		#endregion

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
#if NO_TLS_STREAMING
		public BTypeValuesInt32XmlSerializer(BTypeValuesXmlParams<int> @params, Collections.BTypeValuesInt32 list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeValuesInt32XmlSerializer> kNewFactory = () => new BTypeValuesInt32XmlSerializer();
		BTypeValuesInt32XmlSerializer() { }

		public BTypeValuesInt32XmlSerializer Reset(BTypeValuesXmlParams<int> @params, Collections.BTypeValuesInt32 list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params, list);

			return this;
		}
#endif
		#endregion

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
#if NO_TLS_STREAMING
		public BTypeValuesSingleXmlSerializer(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeValuesSingleXmlSerializer> kNewFactory = () => new BTypeValuesSingleXmlSerializer();
		BTypeValuesSingleXmlSerializer() { }

		public BTypeValuesSingleXmlSerializer Reset(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params, list);

			return this;
		}
#endif
		#endregion

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
#if NO_TLS_STREAMING
		public BTypeValuesStringXmlSerializer(BTypeValuesXmlParams<string> @params, Collections.BTypeValuesString list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeValuesStringXmlSerializer> kNewFactory = () => new BTypeValuesStringXmlSerializer();
		BTypeValuesStringXmlSerializer() { }

		public BTypeValuesStringXmlSerializer Reset(BTypeValuesXmlParams<string> @params, Collections.BTypeValuesString list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params, list);

			return this;
		}
#endif
		#endregion

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
		/*readonly*/ string kAttrName;

#if NO_TLS_STREAMING
		public BTypeValuesSingleAttrHackXmlSerializer(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list, string attr_name)
			: base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
			Contract.Requires<ArgumentNullException>(attr_name != null);

			kAttrName = attr_name;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BTypeValuesSingleAttrHackXmlSerializer> kNewFactory = () => new BTypeValuesSingleAttrHackXmlSerializer();
		BTypeValuesSingleAttrHackXmlSerializer() { }

		public BTypeValuesSingleAttrHackXmlSerializer Reset(BTypeValuesXmlParams<float> @params, Collections.BTypeValuesSingle list, string attr_name)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
			Contract.Requires<ArgumentNullException>(attr_name != null);

			base.Reset(@params, list);
			kAttrName = attr_name;

			return this;
		}
#endif
		#endregion

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

	internal class BCostTypeValuesSingleAttrHackXmlSerializer : XML.BListExplicitIndexXmlSerializerBase<float>
	{
		// Just an alias for less typing and code
		static readonly XML.BTypeValuesXmlParams<float> kParams = Engine.BResource.kBListTypeValuesXmlParams_Cost;

		Collections.BTypeValuesSingle mList;

		public override Collections.BListExplicitIndexBase<float> ListExplicitIndex { get { return mList; } }

#if NO_TLS_STREAMING
		public BCostTypeValuesSingleAttrHackXmlSerializer(Collections.BTypeValuesSingle list) : base(kParams)
		{
			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BCostTypeValuesSingleAttrHackXmlSerializer> kNewFactory = () => new BCostTypeValuesSingleAttrHackXmlSerializer();
		BCostTypeValuesSingleAttrHackXmlSerializer() { }

		public BCostTypeValuesSingleAttrHackXmlSerializer Reset(Collections.BTypeValuesSingle list)
		{
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(kParams);
			mList = list;

			return this;
		}

		protected override void FinishTlsStreaming()
		{
			base.FinishTlsStreaming();
			mList = null;
		}
#endif
		#endregion

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration) { throw new NotImplementedException(); }
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, float data) { throw new NotImplementedException(); }
		protected override int ReadExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs) { throw new NotImplementedException(); }
		protected override void WriteExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int index) { throw new NotImplementedException(); }

		protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			var penum = mList.TypeValuesParams.kGetProtoEnumFromDB(xs.Database);

			foreach (XmlAttribute attr in s.Cursor.Attributes)
			{
				// The only attributes in this are actual member names so we don't waste time calling
				// penum.IsValidMemberName only to call GetMemberId when we can just compare id to -1
				int index = penum.GetMemberId(attr.Name);
				if (index == PhxLib.Util.kInvalidInt32) continue;

				mList.InitializeItem(index);
				float value = PhxLib.Util.kInvalidSingle;
				s.ReadAttribute(attr.Name, ref value);
				mList[index] = value;
			}
		}
		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			var tvp = mList.TypeValuesParams;

			var penum = tvp.kGetProtoEnumFromDB(xs.Database);
			float k_invalid = tvp.kTypeGetInvalid();

			for (int x = 0; x < mList.Count; x++)
			{
				float data = mList[x];

				if (tvp.kComparer.Compare(data, k_invalid) != 0)
				{
					string name = penum.GetMemberName(x);
					s.WriteAttribute(name, data);
				}
			}
		}
		#endregion
	};
}