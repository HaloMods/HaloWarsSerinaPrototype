﻿using System;
using System.Collections.Generic;
using System.Reflection;

using PhxLib.Engine;

namespace PhxLib.DataAnnotations
{
	public abstract class BDatabaseTypeNameIDAttribute : Attribute
	{
		public abstract DatabaseTypeKind GetKind();
	};

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BResourceIDAttribute : BDatabaseTypeNameIDAttribute
	{
		public override DatabaseTypeKind GetKind() { return DatabaseTypeKind.Cost; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BPopulationIDAttribute : BDatabaseTypeNameIDAttribute
	{
		public override DatabaseTypeKind GetKind() { return DatabaseTypeKind.Pop; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BRateIDAttribute : BDatabaseTypeNameIDAttribute
	{
		public override DatabaseTypeKind GetKind() { return DatabaseTypeKind.Rate; }
	};


	public abstract class BDatabaseObjectIDAttribute : Attribute
	{
		public abstract DatabaseObjectKind GetKind();
	};

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BAbilityIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Ability; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BCivIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Civ; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BLeaderIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Leader; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BProtoObjectIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Object; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BObjectTypeIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.ObjectType; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BProtoPowerIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Power; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BProtoSquadIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Squad; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BProtoTechIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Tech; }
	};
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BProtoUnitIDAttribute : BDatabaseObjectIDAttribute
	{
		public override DatabaseObjectKind GetKind() { return DatabaseObjectKind.Unit; }
	};
}