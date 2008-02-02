using System.CodeDom;

namespace RuleKnit
{
	public interface IRuleElement
	{
		Signature Signature { get; }
		Signature[] Dependencies { get; }
		RuleEvaluation EvaluationMode { get; }
		CodeExpression ToCode();
	}

	public enum RuleEvaluation : short 
	{
		Default,
		Once,
	}
}
