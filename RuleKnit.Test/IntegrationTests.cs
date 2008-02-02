using System;
using NUnit.Framework;

namespace RuleKnit.Test
{
	[TestFixture]
	public class IntegrationTests
	{
		[Test]
		public void Fibonacci()
		{
			FibonacciRuleset ruleset = new RulesetFactory().Create<FibonacciRuleset>();
			Assert.AreEqual(3, ruleset.GetNumber(1, 2, 0, 0));
			Assert.AreEqual(5, ruleset.GetNumber(1, 2, 0, 1));
			Assert.AreEqual(34, ruleset.GetNumber(1, 2, 0, 5));
		}
	}

	[Ruleset]
	public abstract class FibonacciRuleset
	{
		[Generated("Third", IterationExitRule = "Finished")]
		public abstract int GetNumber(int second, int third, int iteration, int iterations);

		protected static int Third(int first, int second)
		{
			return first + second;
		}

		protected static int Second(int third)
		{
			return third;
		}

		protected static int First(int second)
		{
			return second;
		}

		protected  static int Iteration(int iteration)
		{
			return iteration + 1;
		}

		protected static bool Finished(int iterations, int iteration)
		{
			return iteration >= iterations;
		}
	}
}
