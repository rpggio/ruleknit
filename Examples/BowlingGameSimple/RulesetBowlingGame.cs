using System;
using RuleKnit;
using NUnit.Framework;

namespace Examples.BowlingGameSimple
{
	[TestFixture]
	public class RulesetBowlingGameTestCase
	{
		[Test]
		public void Score()
		{
			BowlingGameBehavior.AssertScoring(new RulesetFactory().Create<RulesetBowlingGame>());
		}
	}

	[TestFixture]
	public class IntSetTestCase
	{
		[Test]
		public void Subset()
		{
			Assert.AreEqual(new int[] { 4, 6 }, new IntSet(new int[] { 2, 4, 6, 8 }).Subset(1, 2).Values);
		}

		[Test]
		public void Sum()
		{
			Assert.AreEqual(6, new IntSet(new int[] { 1, 2, 3 }).Sum);
		}
	}

	[Ruleset]
	public abstract class RulesetBowlingGame : IBowlingGame
	{
		public int Score(int[] rolls)
		{
			return Score(new IntSet(rolls), 0, 1);
		}

		[Generated("TotalScore", IterationExitRule = "Finished")]
		protected abstract int Score(IntSet rolls, int totalScore, int frameNumber);

		protected static int FrameNumber(int frameNumber)
		{
			return frameNumber + 1;
		}

		protected static bool Finished(int frameNumber)
		{
			return frameNumber == 10;
		}

		protected static int FrameSize(bool strike)
		{
			return strike ? 1 : 2;
		}

		protected static bool Strike(IntSet rolls)
		{
			return rolls[0] == 10;
		}

		protected static bool Spare(IntSet rolls)
		{
			return rolls[0] + rolls[1] == 10;
		}

		protected static int ScoreBalls(bool strike, bool spare)
		{
			return strike || spare ? 3 : 2;
		}

		protected static IntSet FrameRolls(IntSet rolls, int frameSize)
		{
			return rolls.Subset(0, frameSize);
		}

		protected static IntSet Rolls(IntSet rolls, int frameSize)
		{
			return rolls.Subset(frameSize , rolls.Count - frameSize);
		}

		protected static int FrameScore(IntSet rolls, int scoreBalls)
		{
			return rolls.Subset(0, scoreBalls).Sum;
		}

		protected static int TotalScore(int totalScore, int frameScore)
		{
			return totalScore + frameScore;
		}
	}

	[Immutable]
	public class IntSet
	{
		private readonly int[] values;

		public IntSet(int[] values)
		{
			this.values = values;
		}

		public int[] Values
		{
			get { return values; }
		}

		public int Count
		{
			get { return values.Length; }
		}

		public int this[int index]
		{
			get
			{
				if (index >= values.Length) return 0;
				return values[index];
			}
		}

		public int Sum
		{
			get
			{
				int score = 0;
				Array.ForEach(values, delegate(int r) { score += r; });
				return score;
			}
		}

		public IntSet Subset(int index, int length)
		{
			int[] array = new int[length];
			Array.Copy(values, index, array, 0, length);
			return new IntSet(array);
		}
	}
}
