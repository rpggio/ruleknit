
using System;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class RulesetTypeDefinitionTestCase : RuleKnitTestCase
	{

		[Test]
		public void WarningOnMutableType()
		{
			string result = Generate(new RulesetTypeDefinition(typeof(UsesMutableRuleset)).ToCode());
			AssertCodeContains("#warning", result, "");
			AssertCodeContains(new Signature("Calc", typeof(NoAttributesClass)) + "is not immutable", result, "");
		}

		[Test]
		public void ToCode()
		{
			RulesetTypeDefinition typeDef = new RulesetTypeDefinition(typeof(Nominal));
			AssertCodeContains("public class GeneratedNominal : RuleKnit.Test.RulesetTypeDefinitionTestCase.Nominal {", Generate(typeDef.ToCode()), "class def");
			AssertCodeContains("public override string TestActivatorMethod", Generate(typeDef.ToCode()), "method def");
		}

		[Test]
		[ExpectedException(typeof(RuleKnitException))]
		public void RulesetAttributeChecked()
		{
			new RulesetTypeDefinition(typeof(NoAttributesClass));
		}

		[Ruleset]
		abstract class Nominal
		{
			[Generated("TestValue")]
			public abstract string TestActivatorMethod(int arg1, string arg2);

			public static string TestValue() { return "foo"; }
		}

		[Ruleset]
		abstract class UsesMutableRuleset
		{
			[Generated("Calc")]
			public abstract NoAttributesClass Run(int value);

			protected static NoAttributesClass Calc(int value)
			{
				throw new NotImplementedException();
			}
		}

		class NoAttributesClass { }
	}
}
