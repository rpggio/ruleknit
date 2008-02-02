using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace RuleKnit
{
	public class RulesetTypeDefinition
	{
		private Type rulesetType;

		public RulesetTypeDefinition(Type rulesetType)
		{
			if (rulesetType.GetCustomAttributes(typeof(RulesetAttribute), false).Length < 1)
			{
				throw new RuleKnitException("Ruleset should be marked with RulesetAttribute");
			}
			this.rulesetType = rulesetType;
		}

		protected string Name
		{
			get { return "Generated" + rulesetType.Name; }
		}
		
		public CodeTypeDeclaration ToCode()
		{
			CodeTypeDeclaration typeDeclaration = new CodeTypeDeclaration(Name);
			typeDeclaration.BaseTypes.Add(rulesetType);
			RuleElementList elements = new RuleElementList(RuleMethod.CreateFromType(rulesetType));
			foreach(IRuleElement element in elements)
			{
				if (!element.Signature.IsImmutable)
				{
					typeDeclaration.Members.Add(new CodeWarningSnippet(string.Format("Type {0} is not immutable.", element.Signature)));
				}
			}
			foreach (IGeneratedMethod activatorMethod in GetActivatorMethods(rulesetType))
			{
				typeDeclaration.Members.Add(activatorMethod.ToCode(elements));
			}
			return typeDeclaration;
		}

		protected static IEnumerable<IGeneratedMethod> GetActivatorMethods(Type type)
		{
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (methodInfo.IsAbstract)
				{
					GeneratedMethod method = new GeneratedMethod(methodInfo);
					if (method.GeneratedAttribute != null)
					{
						yield return method;
					}
				}
			}
		}
	}
}
