using System.Collections.Generic;
using NUnit.Framework;
using RuleKnit;

namespace RuleKnit.Test
{
	[TestFixture]
	public class CollectionUtilityTestCase
	{
		[Test]
		public void AreEqualDictionaries()
		{
			Dictionary<int, string> a = new Dictionary<int, string>();
			a[1] = "1";
			a[2] = "2";
			Dictionary<int, string> aSame = new Dictionary<int, string>();
			aSame[1] = "1";
			aSame[2] = "2";
			Dictionary<int, string> aDiff = new Dictionary<int, string>();
			aDiff[1] = "diff";
			aDiff[2] = "diff";
			Dictionary<int, string> nulls = new Dictionary<int, string>();
			nulls[1] = null;
			nulls[2] = null;
			Assert.IsTrue(CollectionUtility.AreEqual(a, aSame));
			Assert.IsFalse(CollectionUtility.AreEqual(a, new Dictionary<int, string>()));
			Assert.IsFalse(CollectionUtility.AreEqual(a, aDiff));
			Assert.IsFalse(CollectionUtility.AreEqual(a, nulls));
			Assert.IsTrue(CollectionUtility.AreEqual(nulls, nulls));
		}
	}
}