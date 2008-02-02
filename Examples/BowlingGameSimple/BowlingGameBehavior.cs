using NUnit.Framework;

namespace Examples
{
	public interface IBowlingGame
	{
		int Score(int[] rolls);
	}

	public class BowlingGameBehavior
	{
		internal static void AssertScoring(IBowlingGame game)
		{
            // Tests from http://web.archive.org/web/20090304201612/http://xprogramming.com/xpmag/acsBowlingProcedural.htm
			Assert.AreEqual(0, game.Score(CreateRolls(20, 0)), "gutter balls");
			Assert.AreEqual(60, game.Score(CreateRolls(20, 3)), "threes");
			Assert.AreEqual(23, game.Score(CreateRolls(20, 4, 6, 5, 3, 0)), "spare");
			Assert.AreEqual(29, game.Score(CreateRolls(19, 10, 5, 3, 2, 1, 0)), "strike");
			Assert.AreEqual(300, game.Score(CreateRolls(12, 10)), "perfect");
			Assert.AreEqual(200, game.Score(CreateRolls(16, 10, 4, 6, 10, 4, 6, 10, 4, 6, 10, 4, 6, 10, 4, 6, 10)), "alternating");

			// Tests from a more recent article http://www.xprogramming.com/xpmag/dbcRecurringDrama.htm
			Assert.AreEqual(22, game.Score(CreateRolls(20, 6, 4, 5, 2, 0)), "spare2");
			Assert.AreEqual(200, game.Score(new int[] { 10,5,5, 10,5,5, 10,5,5, 10,5,5, 10,5,5, 10 }), "alternating strike spare");
			Assert.AreEqual(200, game.Score(new int[] { 5,5, 10,5,5, 10,5,5, 10,5,5, 10,5,5, 10,5,5 }), "alternating spare strike");
			Assert.AreEqual(20, game.Score(new int[] { 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 10,5,5 }), "trailing spare");
			Assert.AreEqual(15, game.Score(new int[] { 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 10,2,3 }), "pit strike final frame");
			Assert.AreEqual(20, game.Score(new int[] { 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 0,0, 10, 2,3 }), "pit strike ninth frame");
		}

		private static int[] CreateRolls(int count, params int[] scores)
		{
			int[] rolls = new int[count];
			scores.CopyTo(rolls, 0);
			int lastScore = scores[scores.Length - 1];
			for (int i = scores.Length ; i < count; i++)
			{
				rolls[i] = lastScore;
			}
			return rolls;
		}
	}
}