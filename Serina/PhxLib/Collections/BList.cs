using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BListParams : BCollectionParams
	{
		public /*readonly*/ string DataName;

		#region Flags
		[Contracts.Pure]
		public bool InternDataNames { get { return HasFlag(BCollectionParamsFlags.InternDataNames); } }
		[Contracts.Pure]
		public bool UseInnerTextForData { get { return HasFlag(BCollectionParamsFlags.UseInnerTextForData); } }
		[Contracts.Pure]
		public bool UseElementForData { get { return HasFlag(BCollectionParamsFlags.UseElementForData); } }
		[Contracts.Pure]
		public bool RequiresDataNamePreloading { get { return HasFlag(BCollectionParamsFlags.RequiresDataNamePreloading); } }
		[Contracts.Pure]
		public bool SupportsUpdating { get { return HasFlag(BCollectionParamsFlags.SupportsUpdating); } }
		#endregion

		public BListParams() { }
		/// <summary>Sets RootName to plural of ElementName and sets UseInnerTextForData</summary>
		/// <param name="element_name"></param>
		/// <param name="additional_flags"></param>
		public BListParams(string element_name, BCollectionParamsFlags additional_flags = 0) : base(element_name)
		{
			Flags = additional_flags;
			Flags |= BCollectionParamsFlags.UseInnerTextForData;
		}

		public void StreamDataName(KSoft.IO.XmlElementStream s, FA mode, ref string name)
		{
			BCollectionParams.StreamValue(s, mode, DataName, ref name, 
				UseInnerTextForData, UseElementForData, InternDataNames, HasFlag(BCollectionParamsFlags.ToLowerDataNames));
		}
		public void StreamIsUpdateAttr(KSoft.IO.XmlElementStream s, FA mode, ref bool is_update)
		{
			s.StreamAttributeOpt(mode, "update", ref is_update, Util.kNotFalsePredicate);
		}
	};

	public abstract class BListBase<T> : List<T>, IEqualityComparer<BListBase<T>>, IO.IPhxXmlStreamable
	{
		protected static readonly IEqualityComparer<T> kValueEqualityComparer = EqualityComparer<T>.Default;

		protected class _EqualityComparer : IEqualityComparer<BListBase<T>>
		{
			#region IEqualityComparer<BListBase<T>> Members
			public bool Equals(BListBase<T> x, BListBase<T> y)
			{
				bool equals = x.Count == y.Count;
				if (equals)
				{
					for (int i = 0; i < x.Count && equals; i++)
						equals &= kValueEqualityComparer.Equals(x[i], y[i]);
				}

				return equals;
			}

			public int GetHashCode(BListBase<T> obj)
			{
				int hash = 0;
				foreach (var o in obj) hash ^= kValueEqualityComparer.GetHashCode(o);

				return hash;
			}
			#endregion
		};
		protected static _EqualityComparer kEqualityComparer = new _EqualityComparer();

		/// <summary>Parameters that dictate the functionality of this list</summary>
		public BListParams Params { get; private set; }

		protected BListBase(BListParams @params) : base(@params.InitialCapacity)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			Params = @params;
		}

		#region IXmlElementStreamable Members
		protected abstract void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration);
		protected abstract void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data);

		protected virtual void ReadXmlDetermineListSize(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			int xml_node_count = s.Cursor.ChildNodes.Count;
			if(base.Capacity < xml_node_count)
				base.Capacity = xml_node_count;
		}
		protected virtual void ReadXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			ReadXmlDetermineListSize(s, db);

			int x = 0;
			foreach (XmlNode n in s.Cursor.ChildNodes)
			{
				if (n.Name != Params.ElementName) continue;

				using (s.EnterCursorBookmark(n as XmlElement))
					ReadXml(s, db, x++);
			}

			base.TrimExcess();
		}
		protected virtual void WriteXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			foreach (T data in this)
				using (s.EnterCursorBookmark(Params.ElementName))
					WriteXml(s, db, data);
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db)
		{
			using (s.EnterCursorBookmark(mode, Params.GetOptionalRootName()))
			{
					 if (mode == FA.Read)	ReadXmlNodes(s, db);
				else if (mode == FA.Write)	WriteXmlNodes(s, db);
			}
		}
		#endregion

		#region IEqualityComparer<BListBase<T>> Members
		public bool Equals(BListBase<T> x, BListBase<T> y)
		{
			return kEqualityComparer.Equals(x, y);
		}

		public int GetHashCode(BListBase<T> obj)
		{
			return kEqualityComparer.GetHashCode(obj);
		}
		#endregion
	};

	/// <summary>For an array of items which have no specific order or names</summary>
	/// <typeparam name="T"></typeparam>
	/// <see cref="Engine.BProtoTechEffect"/>
	public class BListArray<T> : BListBase<T>
		where T : IO.IPhxXmlStreamable, new()
	{
		public BListArray(BListParams @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			T item = new T();
			item.StreamXml(s, FA.Read, db);

			Add(item);
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data)
		{
			data.StreamXml(s, FA.Write, db);
		}
		#endregion
	};
}