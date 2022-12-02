namespace RockPaperScissors
{
	public enum MoveType
	{
		Rock = 1,
		Paper = 2,
		Scissors = 3
	}

	public class Move
	{
		private MoveType type { get; }

		public Move(string m)
		{
			if (m == "A" || m == "X")
			{
				this.type = MoveType.Rock;
			}
			else if (m == "B" || m == "Y")
			{
				this.type = MoveType.Paper;
			}
			else if (m == "C" || m == "Z")
			{
				this.type = MoveType.Scissors;
			}
			else
			{
				throw new Exception($"Cannot parse \"{m}\" as a Move");
			}
		}

		public override string ToString()
		{
			return $"{type} ({(int)type} pts)";
		}

		public static implicit operator int(Move m)
		{
			return (int)m.type;
		}

		public int CompareTo(Move m)
		{
			if (this.type == m.type)
			{
				return 0;
			}
			else if (this.type == MoveType.Rock)
			{
				return m.type == MoveType.Paper ? -1 : 1;
			}
			else if (this.type == MoveType.Paper)
			{
				return m.type == MoveType.Scissors ? -1 : 1;
			}
			else // if (this.type == MoveType.Scissors)
			{
				return m.type == MoveType.Rock ? -1 : 1;
			}
		}
	}

	public class Round
	{
		private Move p1 { get; }
		private Move p2 { get; }

		private string outcome { get; }

		private (int p1, int p2) points { get; }

		public Round(Move p1, Move p2)
		{
			this.p1 = p1;
			this.p2 = p2;

			switch (p1.CompareTo(p2))
			{
				case -1:
					outcome = "Player 2 wins";
					points = ((int)p1, (int)p2 + 6);
					break;
				case 0:
					outcome = "Tie";
					points = ((int)p1 + 3, (int)p2 + 3);
					break;
				case 1:
					outcome = "Player 1 wins";
					points = ((int)p1 + 6, (int)p2);
					break;
				default:
					throw new Exception($"Bad comparison result for {p1} vs {p2}: {p1.CompareTo(p2)}");
			}		
		}

		public override string ToString()
		{
			return $"{p1} vs {p2} - {outcome}! Player 1 gets {points.p1} points, player 2 gets {points.p2} points.";
		}
	}


	public class Game
	{

	}
}
