using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class SignatureTestCase
	{
		[Test]
		public void Equals()
		{
			Signature a = new Signature("a", typeof(int));
			Assert.IsTrue(a.Equals(a), "identity");
			Assert.IsFalse(a.Equals(new Signature("a", typeof(float))), "type different");
			Assert.IsFalse(a.Equals(new Signature("b", typeof(int))), "name different");
			Assert.IsTrue(a.Equals(new Signature("A", typeof(int))), "case insensitive");
		}

		[Test]
		public void NameAsVariable()
		{
			Assert.AreEqual("foo", new Signature("Foo", typeof(void)).NameAsVariable);
		}

		[Test]
		public void Satisfies()
		{
			Assert.IsTrue(new Signature("ANumber", typeof(int)).Satisfies(new Signature("anumber", typeof(int))), "case insensitive");
			Assert.IsTrue(new Signature("ANumber", typeof(int)).Satisfies(new Signature("ANumber", typeof(object))), "assignable");
			Assert.IsFalse(new Signature("ANumber", typeof(object)).Satisfies(new Signature("ANumber", typeof(int))), "not assignable");
			Assert.IsFalse(new Signature("ANumber", typeof(int)).Satisfies(new Signature("OtherNumber", typeof(int))), "name mismatch");
		}

		[Test]
		public void IsImmutable()
		{
			Assert.IsTrue(new Signature(null, typeof(int)).IsImmutable, "int");
			Assert.IsTrue(new Signature(null, typeof(ReadOnlyCollectionBase)).IsImmutable, "ReadOnlyCollectionBase");
			Assert.IsTrue(new Signature(null, typeof(ReadOnlyCollection<object>)).IsImmutable, "ReadOnlyCollection<>");
			Assert.IsTrue(new Signature(null, typeof(ImmutableClass)).IsImmutable, "Custom class");
		}

		[Immutable]
		class ImmutableClass { }
	}
}
