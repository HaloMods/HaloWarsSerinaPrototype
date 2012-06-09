using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	/// <summary>Various flags for <see cref="BCollectionXmlParams"/></summary>
	/// <remarks>
	/// * Intern flags should be set when certain values are strings and are used repeatedly within game data
	/// </remarks>
	[Flags]
	public enum BCollectionXmlParamsFlags
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

	public abstract class BCollectionXmlParams
	{
		/// <summary>Root element name in the XML</summary>
		public /*readonly*/ string RootName;
		/// <summary>Name of the elements, that appear under the root element, and host our values</summary>
		public /*readonly*/ string ElementName;

		/// <summary>Do we explicitly filter the XML tags to match <see cref="ElementName"/>?</summary>
		public bool UseElementName { get { return ElementName != null; } }

		#region Flags
		public /*readonly*/ BCollectionXmlParamsFlags Flags;

		[Contracts.Pure]
		protected bool HasFlag(BCollectionXmlParamsFlags flag) { return (Flags & flag) == flag; }

		public void SetForceNoRootElementStreaming(bool is_set)
		{
			if (is_set) Flags |= BCollectionXmlParamsFlags.ForceNoRootElementStreaming;
			else Flags &= ~BCollectionXmlParamsFlags.ForceNoRootElementStreaming;
		}
		#endregion

		public string GetOptionalRootName()
		{
			if (!HasFlag(BCollectionXmlParamsFlags.ForceNoRootElementStreaming))
				return RootName;

			return null;
		}

		protected BCollectionXmlParams() {}
		/// <summary>Sets RootName to plural of ElementName</summary>
		/// <param name="element_name"></param>
		protected BCollectionXmlParams(string element_name)
		{
			RootName = element_name + "s";
			ElementName = element_name;
		}

		#region XmlElementStream util
		protected static void StreamValue(KSoft.IO.XmlElementStream s, FA mode, string value_name, ref string value, 
			bool use_inner_text, bool use_element, bool intern_value, bool to_lower)
		{
			if (use_inner_text)			s.StreamCursor(mode, ref value);
			else if (use_element)		s.StreamElement(mode, value_name, ref value);
			else if (value_name != null)s.StreamAttribute(mode, value_name, ref value);

			if (mode == FA.Read)
			{
				if (to_lower) value = value.ToLowerInvariant();
				if (intern_value) value = string.Intern(value);
			}
		}
		protected static void StreamValue(KSoft.IO.XmlElementStream s, FA mode, string value_name, ref int value, 
			bool use_inner_text, bool use_element)
		{
			if (use_inner_text)			s.StreamCursor(mode, ref value);
			else if (use_element)		s.StreamElement(mode, value_name, ref value);
			else if (value_name != null)s.StreamAttribute(mode, value_name, ref value);
		}
		#endregion
	};
}