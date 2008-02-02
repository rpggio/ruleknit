using System;

namespace RuleKnit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class RulesetAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class ExposePropertiesAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ImmutableAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class EvaluationAttribute : Attribute
	{
		private RuleEvaluation _evaluationMode;

		public EvaluationAttribute(RuleEvaluation evaluationMode)
		{
			_evaluationMode = evaluationMode;
		}

		public RuleEvaluation EvaluationMode
		{
			get { return _evaluationMode; }
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class GeneratedAttribute : Attribute
	{
		private string _returnValueRule;
		private string _iterationExitRule;

		public GeneratedAttribute(string returnValueRule)
		{
			_returnValueRule = returnValueRule;
		}

		public string ReturnValueRule
		{
			get { return _returnValueRule; }
		}

		public string IterationExitRule
		{
			get { return _iterationExitRule; }
			set { _iterationExitRule = value; }
		}
	}
}