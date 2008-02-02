using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuleKnit
{
	public class RuleElementList : IEnumerable
	{
		private IRuleElement[] _rules;
		
		public RuleElementList(params IRuleElement[] rules)
		{
			_rules = rules;
		}

		public RuleElementList With(params IRuleElement[] rules)
		{
			List<IRuleElement> list = new List<IRuleElement>(_rules);
			list.AddRange(rules);
			return new RuleElementList(list.ToArray());
		}

		public RuleElementList Without(IRuleElement rule)
		{
			List<IRuleElement> list = new List<IRuleElement>((IRuleElement[])_rules.Clone());
			list.Remove(rule);
			return new RuleElementList(list.ToArray());
		}

		public IRuleElement Find(string name)
		{
			return Array.Find(_rules,
				delegate(IRuleElement r) { return name.Equals(r.Signature.Name); });
		}

		public IRuleElement Find(Signature signature)
		{
			return Array.Find(_rules,
				delegate(IRuleElement r) { return signature.Equals(r.Signature); });
		}

		public IRuleElement[] FindAll(RuleEvaluation evaluationMode)
		{
			return Array.FindAll(_rules,
				delegate(IRuleElement r) { return r.EvaluationMode == evaluationMode; });
		}

		public IRuleElement FindSatisfying(Signature signature)
		{
			IRuleElement[] found = Array.FindAll(_rules,
				delegate(IRuleElement r) { return r.Signature.Satisfies(signature); });
			if (found.Length > 1)
			{
				throw new RuleKnitException("Found more than one rule satisfying " + signature);
			}
			return found.FirstOrDefault();
		}

		public RuleElementList GetOrderedDependencies(params IRuleElement[] requiredRules)
		{
			List<IRuleElement> methodList = new List<IRuleElement>();
			Array.ForEach(requiredRules,
				delegate(IRuleElement r) { methodList.AddRange(BuildDependencyList(r, this)); });
			return new RuleElementList(methodList.Distinct().ToArray());
		}

		public IRuleElement[] ToArray()
		{
			return _rules;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rules.GetEnumerator();
		}

		protected static internal List<IRuleElement> BuildDependencyList(IRuleElement element, RuleElementList availableRules)
		{
			List<IRuleElement> list = new List<IRuleElement>();
			foreach(Signature arg in element.Dependencies)
			{
				IRuleElement parameter = availableRules.FindSatisfying(arg);
				if (parameter != null && parameter != element)
				{
					list.AddRange(BuildDependencyList(parameter, availableRules.Without(element)));
				}
			}
			list.Add(element);
			return list;
		}
	}
}
