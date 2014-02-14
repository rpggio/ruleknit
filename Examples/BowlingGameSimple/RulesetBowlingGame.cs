using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// The simplicity of the methods in this class are what RuleKnit is designed to achieve.
    /// </summary>
    [Ruleset]
    public abstract class RulesetBowlingGame : IBowlingGame
    {
        public int Score(int[] rolls)
        {
            return Score(new Rolls(rolls), 0, 1);
        }

        [Generated("TotalScore", IterationExitRule = "Finished")]
        protected abstract int Score(Rolls rolls, int totalScore, int frameNumber);

        protected static int FrameNumber(int frameNumber)
        {
            return frameNumber + 1;
        }

        protected static bool Finished(int frameNumber)
        {
            return frameNumber == 10;
        }

        protected static int FrameSize(bool isStrike)
        {
            return isStrike ? 1 : 2;
        }

        protected static bool IsStrike(Rolls rolls)
        {
            return rolls[0] == 10;
        }

        protected static bool IsSpare(Rolls rolls)
        {
            return rolls[0] + rolls[1] == 10;
        }

        protected static int RollsToScore(bool isStrike, bool isSpare)
        {
            return isStrike || isSpare ? 3 : 2;
        }

        protected static Rolls Rolls(Rolls rolls, int frameSize)
        {
            return rolls.GetRange(frameSize, rolls.Count - frameSize);
        }

        protected static int FrameScore(Rolls rolls, int rollsToScore)
        {
            return rolls.GetRange(0, rollsToScore).Sum();
        }

        protected static int TotalScore(int totalScore, int frameScore)
        {
            return totalScore + frameScore;
        }
    }

    [Immutable]
    public class Rolls : List<int> 
    {
        public Rolls() : base() { }

        public Rolls(int[] values) : base(values) { }

        public new Rolls GetRange(int index, int count)
        {
            return new Rolls(base.GetRange(index, count).ToArray());
        }
    }
}
