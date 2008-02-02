using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuleKnit
{
	public class RuleMethod : IRuleElement
	{
		private MethodInfo methodInfo;

		public RuleMethod(MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
		}
		
		public Signature Signature
		{
			get { return new Signature(methodInfo.Name, methodInfo.ReturnType); }
		}
		
		public Signature[] Dependencies
		{
			get {
				return Array.ConvertAll<ParameterInfo, Signature>(methodInfo.GetParameters(),
					delegate(ParameterInfo pi) { return new Signature(pi.Name, pi.ParameterType); });
			}
		}

		public RuleEvaluation EvaluationMode
		{
			get {
				EvaluationAttribute evaluationAttribute = (EvaluationAttribute)methodInfo
                    .GetCustomAttributes(typeof(EvaluationAttribute), false).FirstOrDefault();
				if (evaluationAttribute != null)
				{
					return evaluationAttribute.EvaluationMode;
				}
				return RuleEvaluation.Default;
			}
		}

		public CodeExpression ToCode()
		{
			return new CodeMethodInvokeExpression(
				null, methodInfo.Name,
				Array.ConvertAll<ParameterInfo, CodeExpression>(
					methodInfo.GetParameters(), CreateVariableReference));
		}

		public static IRuleElement[] CreateFromType(Type type)
		{
			List<IRuleElement> elements = new List<IRuleElement>();
			foreach (MethodInfo methodInfo in type.GetMethods(
				BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				elements.Add(new RuleMethod(methodInfo));
			}
			return elements.ToArray();
		}

		private static CodeExpression CreateVariableReference(ParameterInfo parameterInfo)
		{
			return new CodeVariableReferenceExpression(parameterInfo.Name);
		}
	}
}
