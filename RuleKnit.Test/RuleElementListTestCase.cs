using System;
using System.CodeDom;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class RuleElementListTestCase
	{
		[Test]
		public void FindByName()
		{
			RuleElementList list = new RuleElementList(
				new MockRuleElement(new Signature("foo", typeof(int))),
				new MockRuleElement(new Signature("bar", typeof(string))));
			Assert.AreEqual(typeof(string), list.Find("bar").Signature.Type);
		}

		[Test]
		public void FindBySignature()
		{
			Signature bar = new Signature("bar", typeof(string));
			RuleElementList list = new RuleElementList(
				new MockRuleElement(new Signature("foo", typeof(int))),
				new MockRuleElement(bar));
			Assert.AreEqual(bar, list.Find(bar).Signature);
		}

		[Test]
		public void FindSatisfying()
		{
			Signature foo = new Signature("foo", typeof(int));
			MockRuleElement fooElement = new MockRuleElement(foo);
			RuleElementList list = new RuleElementList(
				fooElement,
				new MockRuleElement(new Signature("bar", typeof(int))),
				new MockRuleElement(new Signature("foo", typeof(string))));
			Assert.AreEqual(fooElement, list.FindSatisfying(foo));
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void FindSatisfyingExceptionOnDuplicate()
		{
			Signature foo = new Signature("foo", typeof(int));
			RuleElementList list = new RuleElementList(
				new MockRuleElement(foo),
				new MockRuleElement(foo));
			list.FindSatisfying(foo);
		}

		[Test]
		public void FindAllByRuleEvaluation()
		{
			Signature foo = new Signature("foo", typeof(int));
			Signature bar = new Signature("bar", typeof(int));
			MockRuleElement barElement = new MockRuleElement(RuleEvaluation.Once, bar);
			RuleElementList list = new RuleElementList(
				new MockRuleElement(RuleEvaluation.Default, foo), barElement);
			Assert.AreEqual(new IRuleElement[] { barElement }, list.FindAll(RuleEvaluation.Once));
		}

		[Test]
		public void GetOrderedDependencies()
		{
			Signature a = new Signature("a", typeof(int));
			Signature b = new Signature("b", typeof(string));
			Signature c = new Signature("c", typeof(char));
			Signature d = new Signature("d", typeof(decimal));
			Signature e = new Signature("e", typeof(Array));
			MockRuleElement elementA = new MockRuleElement(a, b, c);
			MockRuleElement elementB = new MockRuleElement(b, c);
			MockRuleElement elementC = new MockRuleElement(c);
			MockRuleElement elementD = new MockRuleElement(d, e);
			MockRuleElement elementE = new MockRuleElement(e, c);
			RuleElementList list = new RuleElementList(elementE, elementC, elementB, elementD, elementA);
			Assert.AreEqual(new IRuleElement[] { elementC, elementB, elementA }, list.GetOrderedDependencies(elementA).ToArray());
			Assert.AreEqual(new IRuleElement[] { elementC, elementB, elementA, elementE, elementD },
				list.GetOrderedDependencies(elementA, elementD).ToArray());
		}

		[Test]
		public void BuildDependencyList()
		{
			Signature a = new Signature("a", typeof(int));
			Signature b = new Signature("b", typeof(int));
			Signature c = new Signature("c", typeof(int));
			MockRuleElement elementA = new MockRuleElement(a, b, c);
			MockRuleElement elementB = new MockRuleElement(b, c);
			MockRuleElement elementC = new MockRuleElement(c);
			RuleElementList list = new RuleElementList(elementB, elementA, elementC);
			Assert.AreEqual(new IRuleElement[] { elementC, elementB, elementA },
				list.GetOrderedDependencies(elementA).ToArray());
		}

		[Test]
		public void BuildDependencyListRecursive()
		{
			Signature a = new Signature("a", typeof(int));
			Signature b = new Signature("b", typeof(int));
			MockRuleElement elementA = new MockRuleElement(a, b);
			MockRuleElement elementB = new MockRuleElement(b, a);
			RuleElementList list = new RuleElementList(elementB, elementA);
			Assert.AreEqual(new IRuleElement[] { elementB, elementA },
				list.GetOrderedDependencies(elementA).ToArray());
		}

		class MockRuleElement : IRuleElement
		{
			private Signature _signature;
			private Signature[] _arguments;
			private RuleEvaluation _evaluationMode;

			public MockRuleElement(Signature signature, params Signature[] arguments)
				: this(RuleEvaluation.Default, signature, arguments)
			{
			}

			public MockRuleElement(RuleEvaluation evaluationMode, Signature signature, params Signature[] arguments)
			{
				_signature = signature;
				_arguments = arguments;
				_evaluationMode = evaluationMode;
			}

			public Signature Signature
			{
				get { return _signature; }
			}

			public Signature[] Dependencies
			{
				get { return _arguments; }
			}

			public RuleEvaluation EvaluationMode
			{
				get { return _evaluationMode; }
			}

			public CodeExpression ToCode()
			{
				throw new NotImplementedException();
			}
		}

	}
}
