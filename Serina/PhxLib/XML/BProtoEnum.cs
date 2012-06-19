using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	internal static class ProtoEnumUndefinedMembers
	{
		public static void Write(KSoft.IO.XmlElementStream s, BListXmlParams p, 
			Collections.IProtoEnumWithUndefined undefined)
		{
			if (undefined.MemberUndefinedCount == 0) return;

			string element_name = "Undefined" + p.ElementName;

			foreach (string str in undefined.UndefinedMembers)
				using (s.EnterCursorBookmark(element_name))
					s.WriteAttribute(p.DataName, str);
		}
	};
}