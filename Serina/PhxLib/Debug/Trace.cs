using System;
using Diag = System.Diagnostics;

namespace PhxLib.Debug
{
	/// <summary>Utility class for tracing assembly logic (or lack thereof)</summary>
	internal static class Trace
	{
		static Diag.TraceSource kPhxSource,
			kIoSource,
			kUtilSource;

		static Trace()
		{
			kPhxSource = new			Diag.TraceSource("PhxLib",				Diag.SourceLevels.All);
			kIoSource = new				Diag.TraceSource("KSoft.IO",			Diag.SourceLevels.All);
			kUtilSource = new			Diag.TraceSource("PhxLib.Util",			Diag.SourceLevels.All);
		}

		/// <summary>Tracer for the <see cref="PhxLib"/> namespace</summary>
		public static Diag.TraceSource PhxLib		{ get { return kPhxSource; } }
		/// <summary>Tracer for the <see cref="KSoft.IO"/> namespace</summary>
		public static Diag.TraceSource IO			{ get { return kIoSource; } }
		/// <summary>Tracer for the <see cref="PhxLib.Util"/></summary>
		public static Diag.TraceSource Util			{ get { return kUtilSource; } }
	};
}