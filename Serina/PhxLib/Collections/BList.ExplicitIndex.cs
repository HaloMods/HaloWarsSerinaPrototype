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

		/// <summary>Get the 'invalid' value for a value</summary>
		public Func<T> kTypeGetInvalid = () => default(T);

		public BListExplicitIndexParams() { }
		/// <summary>Sets ElementName and sets DataName (defaults to attribute usage)</summary>
		/// <param name="element_name"></param>
		/// <param name="index_name"></param>
		/// <param name="initial_capacity"></param>
		public BListExplicitIndexParams(int initial_capacity = -1) : base()
		{
			Flags = 0;
			if (initial_capacity > 0) base.InitialCapacity = initial_capacity;
		}
	};

	public abstract class BListExplicitIndexBase<T> : BListBase<T>
	{
		internal BListExplicitIndexParams<T> ExplicitIndexParams { get { return Params as BListExplicitIndexParams<T>; } }

		protected BListExplicitIndexBase(BListExplicitIndexParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		/// <summary>
		/// If the new count is greater than <see cref="Count"/>, adds new elements up-to <paramref name="new_count"/>,
		/// using the "invalid value" defined in the list params
		/// </summary>
		/// <param name="new_count"></param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="new_count"/> is less than <see cref="Count"/></exception>
		internal void ResizeCount(int new_count)
		{
			if (new_count < Count) throw new ArgumentOutOfRangeException("new_count", new_count.ToString(),
				"For resizing to a smaller Count, use Capacity.");

			var eip = ExplicitIndexParams;

			for (int x = Count; x < new_count; x++)
				AddItem(eip.kTypeGetInvalid());
		}

		internal void InitializeItem(int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(index.ToString(), "index");

			var eip = ExplicitIndexParams;

			if (index >= Count)
			{
				// expand the list up-to the requested index
				for (int x = Count; x <= index; x++)
					AddItem(eip.kTypeGetInvalid());
			}
			else
				base[index] = eip.kTypeGetInvalid();
		}
	};

	public class BListExplicitIndex<T> : BListExplicitIndexBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		public BListExplicitIndex(BListExplicitIndexParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
}