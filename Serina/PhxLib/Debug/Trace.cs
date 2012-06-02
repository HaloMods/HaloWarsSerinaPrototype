using System;
using Diag = System.Diagnostics;

namespace PhxLib.Debug
{
	/// <summary>Utility class for tracing assembly logic (or lack thereof)</summary>
	internal static class Trace
	{
		static Diag.TraceSource kPhxSource,
			kEngineSource,
			kIoSource,
			kUtilSource,
			kXMLSource;

		static Trace()
		{
			kPhxSource = new			Diag.TraceSource("PhxLib",				Diag.SourceLevels.All);
			kEngineSource = new			Diag.TraceSource("PhxLib.Engine",		Diag.SourceLevels.All);
			kIoSource = new				Diag.TraceSource("KSoft.IO",			Diag.SourceLevels.All);
			kUtilSource = new			Diag.TraceSource("PhxLib.Util",			Diag.SourceLevels.All);
			kXMLSource = new			Diag.TraceSource("PhxLib.XML",			Diag.SourceLevels.All);
		}

		/// <summary>Tracer for the <see cref="PhxLib"/> namespace</summary>
		public static Diag.TraceSource PhxLib		{ get { return kPhxSource; } }
		/// <summary>Tracer for the <see cref="PhxLib.Util"/></summary>
		public static Diag.TraceSource Engine		{ get { return kEngineSource; } }
		/// <summary>Tracer for the <see cref="KSoft.IO"/> namespace</summary>
		public static Diag.TraceSource IO			{ get { return kIoSource; } }
		/// <summary>Tracer for the <see cref="PhxLib.Util"/></summary>
		public static Diag.TraceSource Util			{ get { return kUtilSource; } }
		/// <summary>Tracer for the <see cref="PhxLib.XML"/></summary>
		public static Diag.TraceSource XML			{ get { return kXMLSource; } }
	};
}