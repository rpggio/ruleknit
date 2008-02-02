using System;
using System.CodeDom;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class PropertyValueTestCase : RuleKnitTestCase
	{
		[Test]
		public void ToCode()
		{
			CodeExpression code = new PropertyValue(
				new Signature("Person", typeof(object)),
				new Signature("Name", typeof(string))
				).ToCode();
			string expected = @"person.Name";
			AssertCodeIsEqual(expected, Generate(code), "");
		}

		[Test]
		public void CreateFromType()
		{
			Signature declaringSignature = new Signature("propertyTesting", typeof(PropertiesTesting));
			PropertyValue[] expected = new PropertyValue[] {
				new PropertyValue(declaringSignature, new Signature("Name", typeof(string))),
				new PropertyValue(declaringSignature, new Signature("Age", typeof(int))),
			};
			Assert.AreEqual(expected, PropertyValue.CreateFromType(declaringSignature, typeof(PropertiesTesting)));
		}

		[Test]
		public void CreateFromTypeNotExposed()
		{
			Signature declaringSignature = new Signature("propertyTesting", typeof(PropertiesTestingNotExposed));
			Assert.AreEqual(new IRuleElement[0], PropertyValue.CreateFromType(declaringSignature, typeof(PropertiesTestingNotExposed)));
		}

		[ExposeProperties]
		class PropertiesTesting
		{
			public string Name { get { throw new NotImplementedException(); } }
			public int Age { get { throw new NotImplementedException(); } }
		}

		class PropertiesTestingNotExposed
		{
			public string Name { get { throw new NotImplementedException(); } }
		}
	}
}
