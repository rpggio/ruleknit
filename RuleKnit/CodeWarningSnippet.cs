using System.CodeDom;

namespace RuleKnit
{
	public class CodeWarningSnippet : CodeSnippetTypeMember
	{
		public CodeWarningSnippet(string text) : base(
			string.Format("#warning \"{0}\"", text)
			)
		{
		}
	}
}
