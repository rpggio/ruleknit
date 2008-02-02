using NUnit.Framework;

namespace Examples
{
	[TestFixture]
	public class ProceduralBowlingGameTestCase
	{
		[Test]
		public void Score()
		{
			BowlingGameBehavior.AssertScoring(new ProceduralBowlingGame());
		}
	}

	/// <summary>
    /// From http://web.archive.org/web/20090304201612/http://xprogramming.com/xpmag/acsBowlingProcedural.htm
	/// </summary>
	public class ProceduralBowlingGame : IBowlingGame
	{
		public int Score(int[] rolls)
		{
			int rollIndex = 0;
			int total = 0;
			for (int frame = 0; frame < 10; frame++)
			{
				if (rolls[rollIndex] == 10)
				{
					total += 10 + rolls[rollIndex + 1] + rolls[rollIndex + 2];
					rollIndex++;
				}
				else if (rolls[rollIndex] + rolls[rollIndex + 1] == 10)
				{
					total += 10 + rolls[rollIndex + 2];
					rollIndex += 2;
				}
				else
				{
					total += rolls[rollIndex] + rolls[rollIndex + 1];
					rollIndex += 2;
				}
			}
			return total;
		}
	}
}
