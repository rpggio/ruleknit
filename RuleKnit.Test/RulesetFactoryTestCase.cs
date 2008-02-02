using System.IO;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class RulesetFactoryTestCase : RuleKnitTestCase
	{
		[Test]
		public void WriteCode()
		{
			StringWriter writer = new StringWriter();
			new RulesetFactory().Create<TestClass>(writer, false);
			AssertCodeContains("public override int Foo(string value){", writer.ToString(), "");
		}

		[Test]
		public void Create()
		{
			RulesetFactory factory = new RulesetFactory();
			TestClass testClass = factory.Create<TestClass>();
			Assert.IsNotNull(testClass, "object creation");
			TestClass testClass2 = factory.Create<TestClass>();
			Assert.AreEqual(testClass.GetType(), testClass2.GetType(), "type caching");
		}

		[Test]
		public void GetReferences()
		{
			string[] references = RulesetFactory.GetReferences(GetType());
			Assert.Contains(GetType().Assembly.Location, references);
			Assert.Contains("System.dll", references);
			foreach (string reference in references)
			{
				if (!reference.StartsWith("System") && !File.Exists(reference))
				{
					Assert.Fail("referenced file does not exist:" + reference);
				}
			}
		}

		[Ruleset]
		public abstract class TestClass
		{
			[Generated("FooRule")]
			public abstract int Foo(string value);

			public static int FooRule() { return 0; }
		}
	}
}
