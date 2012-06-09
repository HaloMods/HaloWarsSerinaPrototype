using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	/// <summary>Various flags for <see cref="BCollectionParams"/></summary>
	[Flags]
	public enum BCollectionParamsFlags
	{
	};

	public abstract class BCollectionParams
	{
		public const int kDefaultCapacity = 16;

		/// <summary>For fine tuning the BDictionary initialization, to avoid reallocations</summary>
		public /*readonly*/ int InitialCapacity = kDefaultCapacity;

		#region Flags
		public /*readonly*/ BCollectionParamsFlags Flags;

		[Contracts.Pure]
		protected bool HasFlag(BCollectionParamsFlags flag) { return (Flags & flag) == flag; }
		#endregion

		protected BCollectionParams() {}
	};
}