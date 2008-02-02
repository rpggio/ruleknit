using System;

namespace RuleKnit
{
	public class RuleKnitException : Exception
	{
		public RuleKnitException(string message, params object[] args) 
			: base(string.Format(message, args))
		{
		}
	}
}