using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace RuleKnit
{
	public interface IGeneratedMethod
	{
		CodeMemberMethod ToCode(RuleElementList rules);
	}

	public class GeneratedMethod : IGeneratedMethod
	{
		private MethodInfo methodInfo;

		public GeneratedMethod(MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
		}

		public GeneratedAttribute GeneratedAttribute
		{
			get
			{
				return (GeneratedAttribute) methodInfo
                    .GetCustomAttributes(typeof(GeneratedAttribute), false).FirstOrDefault();
			}
		}

		public CodeMemberMethod ToCode(RuleElementList rules)
		{
			CodeMemberMethod memberMethod = CreateMethodSignature();

			List<Signature> declaredVariables = new List<Signature>();
			foreach (ParameterInfo pi in methodInfo.GetParameters())
			{
				memberMethod.Parameters.Add(
					new CodeParameterDeclarationExpression(pi.ParameterType, pi.Name));
				Signature paramSignature = new Signature(pi.Name, pi.ParameterType);
				declaredVariables.Add(paramSignature);
				rules = rules.With(PropertyValue.CreateFromType(paramSignature, pi.ParameterType));
			}
			ReadOnlyCollection<Signature> parameterVariables = new ReadOnlyCollection<Signature>(declaredVariables.ToArray());
			
			IRuleElement returnValueRule = rules.Find(GeneratedAttribute.ReturnValueRule);
			if (returnValueRule == null)
			{
				throw new RuleKnitException("Could not find return value rule " + GeneratedAttribute.ReturnValueRule);
			}
			
			Skeleton skeleton = new Skeleton();

			IRuleElement iterationExitRule = null;
			if (GeneratedAttribute.IterationExitRule != null)
			{
				iterationExitRule = rules.Find(GeneratedAttribute.IterationExitRule);
			}

			if (!parameterVariables.Contains(returnValueRule.Signature))
			{
				skeleton.AddInvariant(new CodeVariableDeclarationStatement(returnValueRule.Signature.Type,
					returnValueRule.Signature.NameAsVariable));
				declaredVariables.Add(returnValueRule.Signature);
			}
			if (iterationExitRule != null && !parameterVariables.Contains(iterationExitRule.Signature))
			{
				skeleton.AddInvariant(new CodeVariableDeclarationStatement(iterationExitRule.Signature.Type,
					iterationExitRule.Signature.NameAsVariable));
				declaredVariables.Add(iterationExitRule.Signature);
			}

			RuleElementList evaluatingElements;
			if (GeneratedAttribute.IterationExitRule != null)
			{
				evaluatingElements = rules.GetOrderedDependencies(returnValueRule, iterationExitRule);
			}
			else
			{
				evaluatingElements = rules.GetOrderedDependencies(returnValueRule);
			}

			foreach(IRuleElement element in evaluatingElements)
			{
				CodeStatement ruleCode = CreateRuleCode(element, declaredVariables.Contains(element.Signature));
				if (element.EvaluationMode == RuleEvaluation.Once)
				{
					skeleton.AddInvariant(ruleCode);
				}
				else if (iterationExitRule != null && parameterVariables.Contains(element.Signature))
				{
					skeleton.AddPostResult(ruleCode);
				}
				else 
				{
					skeleton.AddVariant(ruleCode);
				}
			}

			memberMethod.Statements.AddRange(skeleton.ToCode(CreateIterationExit(iterationExitRule),
				CreateReturnStatement(returnValueRule.Signature.NameAsVariable)));

			return memberMethod;
		}

		protected CodeVariableReferenceExpression CreateIterationExit(IRuleElement iterationExitRule)
		{
			if (iterationExitRule == null)
			{
				return null;
			}
			return new CodeVariableReferenceExpression(iterationExitRule.Signature.NameAsVariable);
		}

		protected internal class Skeleton
		{
			private List<CodeStatement> _invariantStatements = new List<CodeStatement>();
			private List<CodeStatement> _variantStatements = new List<CodeStatement>();
			private List<CodeStatement> _postResultStatements = new List<CodeStatement>();

			public void AddInvariant(CodeStatement statement)
			{
				_invariantStatements.Add(statement);
			}

			public void AddVariant(CodeStatement statement)
			{
				_variantStatements.Add(statement);
			}

			public void AddPostResult(CodeStatement statement)
			{
				_postResultStatements.Add(statement);
			}

			public CodeStatement[] ToCode(CodeVariableReferenceExpression iterationExit, CodeStatement returnStatement)
			{
				List<CodeStatement> statements = new List<CodeStatement>();
				statements.AddRange(_invariantStatements);
				if (iterationExit != null)
				{
					statements.Add(new CodeSnippetStatement("\tdo {"));
				}
				statements.AddRange(_variantStatements);
				if (iterationExit != null)
				{
					statements.AddRange(_postResultStatements);
					statements.Add(new CodeSnippetStatement("\t}"));
					statements.Add(new CodeSnippetStatement(string.Format("\twhile(!{0});", iterationExit.VariableName)));
				}
				statements.Add(returnStatement);
				return statements.ToArray();
			}
		}

		protected internal static CodeStatement CreateReturnStatement(string returnVariable)
		{
			return new CodeMethodReturnStatement(new CodeVariableReferenceExpression(returnVariable));
		}

		protected internal CodeMemberMethod CreateMethodSignature()
		{
			CodeMemberMethod memberMethod = new CodeMemberMethod();
			memberMethod.Name = methodInfo.Name;
			memberMethod.ReturnType = new CodeTypeReference(methodInfo.ReturnType);
			MemberAttributes visibility = GetVisiblityAttribute(methodInfo.Attributes);
			memberMethod.Attributes = visibility | MemberAttributes.Override;
			return memberMethod;
		}

		protected internal static MemberAttributes GetVisiblityAttribute(MethodAttributes methodAttributes)
		{
			if ((methodAttributes & MethodAttributes.Public) == MethodAttributes.Public)
			{
				return MemberAttributes.Public;
			}
			if ((methodAttributes & MethodAttributes.Family) == MethodAttributes.Family)
			{
				return MemberAttributes.Family;
			}
			if ((methodAttributes & MethodAttributes.Private) == MethodAttributes.Private)
			{
				return MemberAttributes.Private;
			}
			throw new InvalidOperationException("No visibility attribute present");
		}

		protected internal static CodeStatement CreateRuleCode(IRuleElement ruleValueProvider, bool declared)
		{
			CodeExpression methodInvokeExpression = ruleValueProvider.ToCode();
			if (declared)
			{
				return new CodeAssignStatement(
					new CodeVariableReferenceExpression(ruleValueProvider.Signature.NameAsVariable), methodInvokeExpression);
			}
			return new CodeVariableDeclarationStatement(
				ruleValueProvider.Signature.Type, ruleValueProvider.Signature.NameAsVariable, methodInvokeExpression);
		}
	}
}
