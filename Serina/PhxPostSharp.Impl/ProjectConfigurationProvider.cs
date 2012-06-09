using System;
using System.Collections.Generic;
using PostSharp.Extensibility;

namespace PhxPostSharp.Impl
{
	public class ProjectConfigurationProvider : PostSharp.Sdk.Extensibility.IProjectConfigurationProvider
	{
		#region IProjectConfigurationProvider Members
		public PostSharp.Sdk.Extensibility.Configuration.ProjectConfiguration GetProjectConfiguration()
		{
			throw new NotImplementedException();
		}
		#endregion
	};
}