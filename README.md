# ruleknit

## About

RuleKnit provides runtime code generation for wiring together side-effect-free functions. It is intended to allow iterative algorithms to be decomposed into a series of simple functions (as static methods) that can be auto-wired together based on parameter and method naming.

## Bowling sample

The sample code below is a solution to the bowling problem posed in these articles:
* [The Bowling Game](http://web.archive.org/web/20090925095831/http://xprogramming.com/xpmag/acsbowling/)
* [Bowling Revisited](http://web.archive.org/web/20090304201612/http://xprogramming.com/xpmag/acsBowlingProcedural.htm)
* [Extending the Procedural Bowling Game](http://web.archive.org/web/20091220052733/http://xprogramming.com/xpmag/acsBowlingProceduralFrameScore)
* [Mining the Bowling Game](http://xprogramming.com/articles/miningbowling/)

### RuleKnit solution

The `RulesetBowlingGame` class (and accompanying `Rolls` class) are a complete solution to the bowling calculation problem. The rule-based approach can distill and make obvious the underlying algorithms of the problem, as demonstrated in the static methods of `RulesetBowlingGame`. Procedural solutions to the problem frequently obscure the underlying rules, as can be seen in some of the earlier solutions above.

An interesting result of using RuleKnit to solve this problem: the clarity of the solution you see below is similar to the final solution arrived at in the articles above after multiple iterations over time. However, using RuleKnit, this was the result on the first iteration (without having peeked at the existing solutions).

```
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
        public new Rolls GetRange(int index, int count)
        {
            return new Rolls(base.GetRange(index, count));
        }
    }
```

### The generated code

When the ruleset is created, the following code is generated and compiled at runtime to implement the abstract `Score` method. This method runs the rules iteratively until completion, returning the result. The static methods of the class were wired together based on the names of the methods and the parameter names.

```
public class GeneratedRulesetBowlingGame : Examples.BowlingGameSimple.RulesetBowlingGame {
    protected override int Score(Examples.BowlingGameSimple.Rolls rolls, int totalScore, int frameNumber) {
        bool finished;
        do {
            bool isStrike = IsStrike(rolls);
            int frameSize = FrameSize(isStrike);
            bool isSpare = IsSpare(rolls);
            int rollsToScore = RollsToScore(isStrike, isSpare);
            int frameScore = FrameScore(rolls, rollsToScore);
            finished = Finished(frameNumber);
            rolls = Rolls(rolls, frameSize);
            totalScore = TotalScore(totalScore, frameScore);
            frameNumber = FrameNumber(frameNumber);
        }
        while(!finished);
        return totalScore;
    }
}
```

The source code for this solution is provided in the Examples project, along with the source for an algorithmic solution for comparison.

This project was originally published in 2007.

