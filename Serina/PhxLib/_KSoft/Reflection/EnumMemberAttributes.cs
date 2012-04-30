using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSoft.Reflection
{
	public class EnumMemberAttributes<TEnum>
	{
		static readonly Type kEnumType;

		static EnumMemberAttributes()
		{
			kEnumType = typeof(TEnum);

			var members = kEnumType.GetFields(BindingFlags.Static | BindingFlags.Public);
		}
	}
}