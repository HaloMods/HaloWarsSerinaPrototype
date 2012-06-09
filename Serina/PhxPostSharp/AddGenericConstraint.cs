using System;
using PostSharp.Extensibility;

namespace PhxPostSharp
{
	[AttributeUsage(AttributeTargets.GenericParameter)]
	[RequirePostSharp("PhxPostSharp", "AddGenericConstraint")]
	public sealed class AddGenericConstraintAttribute : Attribute
	{
		public AddGenericConstraintAttribute(Type type)
		{
		}
	};

	[AttributeUsage(AttributeTargets.GenericParameter)]
	[RequirePostSharp("PhxPostSharp", "EnumConstraint")]
	public sealed class EnumConstraintAttribute : Attribute
	{
	};
}