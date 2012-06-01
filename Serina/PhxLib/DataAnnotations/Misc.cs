using System;
using System.Collections.Generic;
using System.Reflection;


namespace PhxLib.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DebugDataAttribute : Attribute
	{
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class EditorDataAttribute : DebugDataAttribute
	{
	};
}