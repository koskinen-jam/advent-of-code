using rps = RockPaperScissors;
using rck = Rucksack;

// Solutions for advent of code (2022) puzzles
public class Solutions
{
	// Given a list of integers representing calories in snacks carried by elves,
	// find the amount of calories carried by the one with the most, and the total
	// carried by the three with the most.
	// Elves are separated by empty rows.
	public static void day1()
	{
		Console.WriteLine("\n  Day 1: Top calorie-carrying elves:\n");

		string input= "input/day-1.txt";

		List<Elf> elves = Elf.parseFile(input);

		int top3Calories = 0;
		List<Elf> top3Elves = Elf.topCalories(elves, 3);

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
		Console.WriteLine("\n  Day 2: Rock Paper Scissors tournament:\n");
		Console.WriteLine("\tWe are player 2.\n");

		string input = "input/day-2.txt";

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

	// Given a list of rucksack contents as strings where each item is represented
	// by a lower- or uppercase letter, and the first and last halves of the
	// string representing the two compartments of a rucksack, first find which
	// item is present in both compartments of each rucksack. Items have priorities
	// according to their letter, a-z being 1-26 and A-Z being 27-52. Now find
	// the sum of the priorities of each item that is common to both compartments
	// to its rucksack.
	//
	// And the twist: Each consequent group of three rucksacks contains a key item
	// that is a type of item that appears in each rucksack (compartment does not
	// matter). Find the sum of the priorities of those key items.
	public static void day3()
	{
		Console.WriteLine("\n  Day 3: Rucksack Reorganization:\n");

		string input = "input/day-3.txt";

		int totalPrio = 0;
		List<rck.Sack> sacks = new List<rck.Sack>();

		foreach (string s in Files.GetContentAsList(input))
		{
			rck.Sack sack = rck.Sack.FromString(s);
			rck.Item? common = sack.FindCommon();

			if (common != null)
			{
				totalPrio += common.priority;
			}

			sacks.Add(sack);
		}

		Console.WriteLine($"\tTotal priority of items found in both compartments of their sacks: {totalPrio}\n");

		int totalKeyPrio = 0;

		for (int i = 0; i < sacks.Count; i += 3)
		{
			rck.Item? common = sacks[i].FindCommon(sacks[i + 1], sacks[i + 2]);
			if (common != null)
			{
				totalKeyPrio += common.priority;
			}
		}
		
		Console.WriteLine($"\tTotal priority of group key items: {totalKeyPrio}");	
	}

	// Given a list of pairs of section ID ranges, find the number of pairs where
	// one range fully contains the other (e.g. in "4-5,7-10", neither does, and
	// in "3-5,2-8", the second contains the first.)
	public static void day4()
	{
		Console.WriteLine("\n  Day 4: Camp Cleanup:\n");

		string input = "input/day-4.txt";

		int pairsWithFullContainment = 0;
		int pairsWithOverlap = 0;

		foreach (string s in Files.GetContentAsList(input))
		{
			IdRange[] ranges = IdRange.ParsePair(s);
			if (ranges[0].Contains(ranges[1]) || ranges[1].Contains(ranges[0]))
			{
				pairsWithFullContainment++;
			}

			if (ranges[0].Overlaps(ranges[1]))
			{
				pairsWithOverlap++;
			}
		}

		Console.WriteLine($"\tNumber of pairs where one range fully contains the other one: {pairsWithFullContainment}\n");

		Console.WriteLine($"\tNumber of pairs with overlap: {pairsWithOverlap}");
	}
}
