using rps = RockPaperScissors;

// Solutions for advent of code (2022) puzzles
public class Solutions
{
	// Given a list of integers representing calories in snacks carried by elves,
	// find the amount of calories carried by the one with the most, and the total
	// carried by the three with the most.
	// Elves are separated by empty rows.
	public static void day1()
	{
		string input= "input/day-1.txt";

		List<Elf> elves = Elf.parseFile(input);

		int top3Calories = 0;
		List<Elf> top3Elves = Elf.topCalories(elves, 3);

		Console.WriteLine("\n  Day 1: Top calorie-carrying elves:\n");

		foreach (Elf e in top3Elves)
		{
			Console.WriteLine($"\t{e}");
			top3Calories += e.CaloriesCarried();
		}

		Console.WriteLine($@"
		  The top elf carried {top3Elves[0].CaloriesCarried()} calories.

		  The top 3 carried {top3Calories} calories.");
	}

	// Given a list of Rock Paper Scissors moves and your replies as pairs "M N",
	// where rock = A or X, paper = B or Y, scissors = C or Z, moves award
	// 1, 2, or 3 points respectively, a tie awards 3 extra points and a win
	// awards 6 extra points, calculate your final score at the end of the
	// tournament if you play as indicated.
	//
	// Oh, but there was a misunderstanding! The latter letters in the strategy
	// guide actually meant whether you should lose (X), tie (Y) or win (Z) the
	// round, picking your move according to the opponents expected move.
	public static void day2()
	{
		string input = "input/day-2.txt";

		Console.WriteLine("\n  Day 2: Rock Paper Scissors tournament:\n");
		Console.WriteLine("\tWe are player 2.\n");
		rps.Game g = new rps.Game();
		List<string> strategyGuide = Files.GetContentAsList(input);

		foreach (string s in strategyGuide)
		{
			(rps.Move p1, rps.Move p2) moves = rps.Strategy.ParseNaive(s);
			g.Play(moves.p1, moves.p2);
		}
		
		Console.WriteLine($"\tPlaying naively: {g}");

		g = new rps.Game();

		foreach (string s in strategyGuide)
		{
			(rps.Move p1, rps.Move p2) moves = rps.Strategy.ParseCunning(s);
			g.Play(moves.p1, moves.p2);
		}

		Console.WriteLine($"\n\tPlaying cunningly: {g}");
	}
}
