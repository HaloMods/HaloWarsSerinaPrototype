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
	};

	public abstract class BListBase<T> : List<T>, IEqualityComparer<BListBase<T>>
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

		protected BListBase(BListParams @params) : base(@params != null ? @params.InitialCapacity : BCollectionParams.kDefaultCapacity)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			Params = @params;
		}
		protected BListBase() { }

		public bool IsEmpty { get { return Count == 0; } }
		internal void OptimizeStorage()
		{
			//if (Count == 0)
			//	mList = null;
			this.TrimExcess();
		}

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
	};
}