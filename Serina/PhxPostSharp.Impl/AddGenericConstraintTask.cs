using System;
using System.Collections.Generic;
using PostSharp.Extensibility;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.Extensibility.Tasks;

namespace PhxPostSharp.Impl
{
	public class AddGenericConstraintTask : MulticastAttributeTask
	{
		public override bool Execute()
		{
			// Get the type AddGenericConstraintAttribute.
			ITypeSignature attributeType = this.Project.Module.FindType(
				typeof(AddGenericConstraintAttribute),
				BindingOptions.OnlyDefinition | BindingOptions.DontThrowException);

			if (attributeType == null)
			{
				// The type is not referenced in the current module. There cannot be a custom attribute
				// of this type, so we are done.
				return true;
			}

			// Enumerate custom attributes of type AddGenericConstraintAttribute.
			var annotationRepository = AnnotationRepositoryTask.GetTask(this.Project);
			IEnumerator<IAnnotationInstance> customAttributesEnumerator =
				annotationRepository.GetAnnotationsOfType(attributeType.GetTypeDefinition(), false);
			while (customAttributesEnumerator.MoveNext())
			{
				var current = customAttributesEnumerator.Current;

				// Get the target of the custom attribute.
				GenericParameterDeclaration target = current.TargetElement as GenericParameterDeclaration;

				// Get the value of the parameter of the custom attribute constructor.
				ITypeSignature constraintType = current.Value.ConstructorArguments[0].Value.Value as ITypeSignature;

				// Add a generic constraint.
				target.Constraints.Add(constraintType);

				// Remove the custom attribute.
				(current as CustomAttributeDeclaration).Remove();
			}

			return base.Execute();
		}
	};

	public class EnumConstraintTask : MulticastAttributeTask
	{
		public override bool Execute()
		{
			// Get the type EnumConstraintAttribute.
			ITypeSignature attributeType = this.Project.Module.FindType(
				typeof(EnumConstraintAttribute),
				BindingOptions.OnlyDefinition | BindingOptions.DontThrowException);

			if (attributeType == null)
			{
				// The type is not referenced in the current module. There cannot be a custom attribute
				// of this type, so we are done.
				return true;
			}

			var enumType = this.Project.Module.FindType(
				typeof(Enum),
				BindingOptions.OnlyDefinition | BindingOptions.DontThrowException);

			// Enumerate custom attributes of type EnumConstraintAttribute.
			var annotationRepository = AnnotationRepositoryTask.GetTask(this.Project);
			IEnumerator<IAnnotationInstance> customAttributesEnumerator =
				annotationRepository.GetAnnotationsOfType(attributeType.GetTypeDefinition(), false);
			while (customAttributesEnumerator.MoveNext())
			{
				var current = customAttributesEnumerator.Current;

				// Get the target of the custom attribute.
				GenericParameterDeclaration target = current.TargetElement as GenericParameterDeclaration;

				// Add a generic constraint.
				target.Constraints.Add(enumType);

				// Remove the custom attribute.
				(current as CustomAttributeDeclaration).Remove();
			}

			return base.Execute();
		}
	};
}
