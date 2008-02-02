using System;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class RuleMethodTestCase : RuleKnitTestCase
	{
		[Test]
		public void Arguments()
		{
			Signature[] expected = new Signature[]
				{
					new Signature("arg1", typeof(string)),
					new Signature("arg2", typeof(float))
				};
			Signature[] result = new RuleMethod(GetType().GetMethod("TestMethod")).Dependencies;
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void ToCode()
		{
			string expected = "TestMethod(arg1, arg2)";
			RuleMethod ruleMethod = new RuleMethod(GetType().GetMethod("TestMethod"));
			string result = Generate(ruleMethod.ToCode());
			AssertCodeIsEqual(expected, result, "");
		}

		[Test]
		public void EvaluationMode()
		{
			Assert.AreEqual(RuleEvaluation.Default, new RuleMethod(GetType().GetMethod("TestMethod")).EvaluationMode, "default");
			Assert.AreEqual(RuleEvaluation.Once, new RuleMethod(GetType().GetMethod("EvalOnceMethod")).EvaluationMode, "once");
		}

		[Test]
		public void ListCreation()
		{
			IRuleElement[] elements = RuleMethod.CreateFromType(typeof(ListCreationTesting));
			Assert.AreEqual(2, elements.Length);
			Signature[] signatures = Array.ConvertAll<IRuleElement, Signature>(elements,
				delegate(IRuleElement r) { return r.Signature; });
			Assert.Contains(new Signature("PublicStaticMethod", typeof(ListCreationTesting.Foo)), signatures, "public rule method");
			Assert.Contains(new Signature("InternalStaticMethod", typeof(void)), signatures, "protected rule method");
		}

		public static string TestMethod(string arg1, float arg2)
		{
			throw new NotImplementedException();
		}

		[Evaluation(RuleEvaluation.Once)]
		public static void EvalOnceMethod() { }

		class ListCreationTesting
		{
			public static Foo PublicStaticMethod() { return null; }

			protected internal static void InternalStaticMethod() { }
					
			public void PublicInstanceMethod() { }

			public class Foo
			{
				public int Bar { get { return 0; } }
				protected string NoBarForYou { get { return null; } }
			}
		}
	}
}
