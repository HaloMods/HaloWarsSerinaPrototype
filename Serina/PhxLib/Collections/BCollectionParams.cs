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
	/// <remarks>
	/// * Intern flags should be set when certain values are strings and are used repeatedly within game data
	/// </remarks>
	[Flags]
	public enum BCollectionParamsFlags
	{
		// Only one of these should ever be set
		UseInnerTextForData = 1<<0,

		UseElementForData = 1<<2,

		InternDataNames = 1<<4,

		ToLowerDataNames = 1<<6,
		RequiresDataNamePreloading = 1<<7,

		/// <summary>Forces the list code to not stream the root element from the xml document</summary>
		/// <remarks>Needed for when we're reading definitions from game files, but will later write to a app-specific monolithic file</remarks>
		ForceNoRootElementStreaming = 1<<8,
		SupportsUpdating = 1<<9,

		InternEverything = InternDataNames,
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