using rps = RockPaperScissors;
using rck = Rucksack;
using sps = SupplyStacks;
using drt = DirectoryTree;

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

	// Given a drawing of the starting state of a set of stacks of crates and a list
	// of moves telling how many crates are moves one at a time from which stack
	// to which, what is the word produced by the top crates of each stack once
	// all the listed moves have been made?
	//
	// Drawing:
	//
	//     [X]
	// [A] [B]
	// [D] [N] [P]
	//  1   2   3
	//
	// Move list:
	//
	// move 1 from 2 to 3 (X goes on top of stack 3)
	// move 2 from 1 to 2 (AD goes on top of stack 2, reversed to DA)
	// move 1 from 3 to 1 (X goes on top of stack 1)
	// move 2 from 2 to 3 (DA goes on top of stack 3, reversed back to AD)
	//
	//         [A]
	//     [B] [D]
	// [X] [N] [P]
	//  1   2   3
	public static void day5()
	{
		Console.WriteLine("\n  Day 5: Supply Stacks\n");

		string input = "input/day-5.txt";

		List<string> inputLines = Files.GetContentAsList(input);

		sps.Supply supply = sps.Supply.Parse(inputLines);

		Console.WriteLine($"Initial state:\n\n{supply}\n");
		supply.Move(inputLines);

		Console.WriteLine($"Moving one crate at a time\n\n{supply}\n");

		Console.WriteLine($"They spell out: {supply.TopWord}");

		supply = sps.Supply.Parse(inputLines);
		supply.MoveMultiple = true;
		supply.Move(inputLines);

		Console.WriteLine($"Moving multiple crates at a time\n\n{supply}\n");

		Console.WriteLine($"They spell out: {supply.TopWord}");
	}

	// A start-of-packet marker is four consecutive characters that are all different from each
	// other. A start-of-message marker is the same, but fourteen characters long.
	//
	// Given a string, how many characters do you need to analyze before you have found a
	// start-of-packet marker? What about a start-of-message marker?
	public static void day6()
	{
		Console.WriteLine("\n  Day 6: Tuning Trouble");

		string input = "input/day-6.txt";

		Strings.Finder f = new Strings.Finder(Strings.Finder.AllUniqueChars, 4);

		string buffer = Files.GetContentAsList(input)[0];

		Console.WriteLine($"\nStart of packet marker found after analyzing {f.Find(buffer)} characters.");

		f.BufferLength = 14;

		Console.WriteLine($"\nStart of message marker found after analyzing {f.Find(buffer)} characters.");
	}

	// Given a list of directory-changing and listing commands and their output, find the sum of the sizes
	// of all the directories with total size less than or equal to 100000. The size of a directory is
	// the sum of the sizes of the files and directories in it. A subdirectory may be counted multiple times:
	// First by itself, and again as a part of its containing directory.
	//
	// Additionally, given a total disk size and a minimum free space requirement, find the smallest
	// directory large enough to bring the free disk space over the minimum.
	public static void day7()
	{
		Console.WriteLine("\n  Day 7: No Space Left On Device\n");

		string input = "input/day-7.txt";

		List<string> inputLines = Files.GetContentAsList(input);

		drt.Parser parser = new drt.Parser(inputLines);
		drt.Node root = parser.Parse();
		
		List<drt.Node> dirsFound = root.Find((drt.Node n) => { return ! n.IsFile && n.Size <= 100000; });

		Console.WriteLine("Directories at most 100000 units large\n");
		int total = 0;
		
		foreach (drt.Node n in dirsFound)
		{
			total += n.Size;
			Console.WriteLine($"{total, 10} {n}");
		}
		Console.WriteLine($"\n           Total {total} bytes\n");

		int totalSpace = 70000000;
		int minimumFreeRequired = 30000000;

		int free = totalSpace - root.Size;
		int needToFree = minimumFreeRequired - free;

		Console.WriteLine($"System has total {totalSpace} units of disk. Installation requires {minimumFreeRequired}.");
		Console.WriteLine($"Current free space {free} units, need to free {needToFree} more.");

		List<drt.Node> found = root.Find((drt.Node n) => { return ! n.IsFile && n.Size >= needToFree; });
		
		found.Sort(drt.Node.CompareNodesBySize);

		Console.WriteLine($"Directories larger than {needToFree}:\n");

		foreach (drt.Node n in found)
		{
			Console.WriteLine($"{n} - {n.Path}");
		}

		Console.WriteLine($"\nThe smallest one is {found[0].Path} at {found[0].Size} units.");
	}
}
