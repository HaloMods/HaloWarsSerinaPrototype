using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BListExplicitIndexParams<T> : BListParams
	{
		public readonly IComparer<T> kComparer = Comparer<T>.Default;

		/// <summary>The index base offset as it appears in the XML</summary>
		/// <example>If this is 1, then the XML values are 1, 2, 3, etc.</example>
		/// <remarks>In-memory, everything is always at base-0</remarks>
		public int IndexBase = 1;

		/// <summary>Get the 'invalid' value for a value</summary>
		public Func<T> kTypeGetInvalid = () => default(T);

		public BListExplicitIndexParams() { }
		/// <summary>Sets ElementName and sets DataName (defaults to attribute usage)</summary>
		/// <param name="element_name"></param>
		/// <param name="index_name"></param>
		/// <param name="initial_capacity"></param>
		public BListExplicitIndexParams(string element_name, string index_name, int initial_capacity = -1) : base(element_name)
		{
			RootName = null;
			DataName = index_name;
			Flags = 0;
			if (initial_capacity > 0) base.InitialCapacity = initial_capacity;
		}

		public void StreamExplicitIndex(KSoft.IO.XmlElementStream s, FA mode, ref int index)
		{
			// 'rebase' the index to how the XML defs expect it
			if (mode == FA.Write) index += IndexBase;

			BCollectionParams.StreamValue(s, mode, DataName, ref index, 
				UseInnerTextForData, UseElementForData);

			// Undo any rebasing
			/*if (mode == FA.Read)*/ index -= IndexBase;
		}
	};

	public abstract class BListExplicitIndexBase<T> : BListBase<T>
	{
		BListExplicitIndexParams<T> ExplicitIndexParams { get { return Params as BListExplicitIndexParams<T>; } }

		protected BListExplicitIndexBase(BListExplicitIndexParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		protected void InitializeItem(int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(index.ToString(), "index");

			var eip = ExplicitIndexParams;

			if (index >= Count)
			{
				// expand the list up-to the requested index
				for (int x = Count; x <= index; x++)
					base.Add(eip.kTypeGetInvalid());
			}
			else
				base[index] = eip.kTypeGetInvalid();
		}

		#region IXmlElementStreamable Members
		protected virtual int ReadExplicitIndex(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			int index = -1;
			ExplicitIndexParams.StreamExplicitIndex(s, FA.Read, ref index);

			return index;
		}
		protected virtual void WriteExplicitIndex(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int index)
		{
			ExplicitIndexParams.StreamExplicitIndex(s, FA.Write, ref index);
		}

		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			var eip = ExplicitIndexParams;
			T k_invalid = ExplicitIndexParams.kTypeGetInvalid();

			int index = 0;
			foreach (T data in this)
			{
				if (eip.kComparer.Compare(data, k_invalid) != 0)
				{
					using (s.EnterCursorBookmark(Params.ElementName))
					{
						WriteExplicitIndex(s, db, index);
						WriteXml(s, db, data);
					}
				}

				index++;
			}
		}
		#endregion
	};

	public class BListExplicitIndex<T> : BListExplicitIndexBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		public BListExplicitIndex(BListExplicitIndexParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			T data = new T();
			data.StreamXml(s, FA.Read, db);
			base[index] = data;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data)
		{
			data.StreamXml(s, FA.Write, db);
		}
		#endregion
	};
}