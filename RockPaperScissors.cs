namespace RockPaperScissors
{
	// Possible player moves in RPS
	public enum MoveType
	{
		Rock = 1,
		Paper = 2,
		Scissors = 3
	}

	// Single player choice for a round, comparable to another
	// players move to determine which is better.
	public class Move
	{
		private MoveType type { get; }

		// Parse a Move
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

		// Create a Move
		public Move(MoveType m)
		{
			this.type = m;
		}

		// Stringify this move
		public override string ToString()
		{
			return $"{type} ({(int)type} pts)";
		}

		// Convert Move to the value of its shape
		public static implicit operator int(Move m)
		{
			return (int)m.type;
		}

		// Compare this move to another to determine which one is greater,
		// i.e. which one wins.
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

		// Who has time to type "new Move(MoveType.Rock)"?
		public static Move Rock()
		{
			return new Move(MoveType.Rock);
		}

		// Not this guy
		public static Move Paper()
		{
			return new Move(MoveType.Paper);
		}

		// No sirree
		public static Move Scissors()
		{
			return new Move(MoveType.Scissors);
		}

		// Return the move this move will win
		// (e.g. if this is rock, return scissors)
		public Move Wins()
		{
			return this.type == MoveType.Rock
				? Scissors()
				: this.type == MoveType.Paper
					? Rock()
					: Paper();
		}

		// Return the move this move ties with
		public Move TiesWith()
		{
			return new Move(this.type);
		}

		// Return the move this move will lose to
		public Move LosesTo()
		{
			return this.type == MoveType.Rock
				? Paper()
				: this.type == MoveType.Paper
					? Scissors()
					: Rock();
		}
	}

	// A round of Rock Paper Scissors, including extra scoring rules invented
	// by elves.
	public class Round
	{
		private Move p1 { get; }
		private Move p2 { get; }

		private string outcome { get; }

		private (int p1, int p2) points { get; }

		// Create and resolve a round of Rock Paper Scissors
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

		// Convert this round into a string
		public override string ToString()
		{
			return $"{p1} vs {p2} - {outcome}! Player 1 gets {points.p1} points, player 2 gets {points.p2} points.";
		}

		// Parse a string like "B Z" into a new Round
		public static Round Parse(string input)
		{
			string[] moves = input.Split(" ");

			return new Round(new Move(moves[0]), new Move(moves[1]));
		}

		public (int p1, int p2) GetPoints()
		{
			return points;
		}
	}

	// A game of Rock Paper Scissors consisting of many rounds
	public class Game
	{
		private List<Round> rounds { get; }
		private int scoredAt = 0;

		// Backing variable for points
		private (int p1, int p2) latestPoints = (0, 0);

		// Current points as readable public property. I guess.
		public (int p1, int p2) points {
			get
			{
				if (rounds.Count == scoredAt)
				{
					return latestPoints;
				}

				UpdateScores();
				return latestPoints;
			}
		}

		// Recalculate current standings
		private void UpdateScores()
		{
				scoredAt = rounds.Count;
				latestPoints = (0, 0);

				foreach (Round r in rounds)
				{
					latestPoints.p1 += r.GetPoints().p1;
					latestPoints.p2 += r.GetPoints().p2;
				}
		}

		// Create a new game of Rock Paper Scissors
		public Game()
		{
			this.rounds = new List<Round>();
		}

		// Play a round using a strategy guide line
		public void Play(string input)
		{
			rounds.Add(Round.Parse(input));
		}

		// Play a round using moves
		public void Play(Move p1, Move p2)
		{
			rounds.Add(new Round(p1, p2));
		}

		// Return current standings
		public override string ToString()
		{
			return $"After {rounds.Count} rounds, scores are {points.p1} to {points.p2}. {State()}";
		}

		// Return description of which player is winning
		public string State()
		{
			if (points.p1 == points.p2)
			{
				return "It's a tie";
			}
			else
			{
				return $"Player {(points.p1 > points.p2 ? "1" : "2")} is winning";
			}
		}
	}

	// Parsers for elves' secret Rock Paper Scissors strategy guide
	public class Strategy
	{
		// Initial, naive understanding of strategy, where second move is
		// interpreted as an explicit move.
		public static (Move p1, Move p2) ParseNaive(string s)
		{
			string[] moves = s.Split(" ");
			return (new Move(moves[0]), new Move(moves[1]));
		}

		// Full explanation of strategy, where second value means whether
		// you should lose, tie or win the round.
		public static (Move p1, Move p2) ParseCunning(string s)
		{
			string[] moves = s.Split(" ");
			Move p1 = new Move(moves[0]);

			Move p2;
			switch (moves[1])
			{
				case "X":
					p2 = p1.Wins();
					break;
				case "Y":
					p2 = p1.TiesWith();
					break;
				case "Z":
					p2 = p1.LosesTo();
					break;
				default:
					throw new Exception($"Whatever this {moves[1]} means...");
			}

			return (p1, p2);
		}
	}
}
