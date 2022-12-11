using rps = RockPaperScissors;
using rck = Rucksack;
using sps = SupplyStacks;
using drt = DirectoryTree;
using thm = TreeHeightMap;
using rb = RopeBridge;
using crt = CathodeRayTube;

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

	// Given a map of tree heights, find the number of trees that are visible from at least one map edge.
	// A tree is visible if it is on a map edge, or all the trees between it and the map edge are shorter.
	//
	// Find the maximum scenic score among the trees. The scenic score is calculated by multiplying together
	// the visibilities in all the cardinal directions from a given tree. Visibility is blocked by a tree
	// that is at least as tall as the tree being looked from.
	public static void day8()
	{
		Console.WriteLine("\n  Day 8: Treetop Tree House\n");

		List<string> input = Files.GetContentAsList("input/day-8.txt");

		thm.Map map = thm.Map.Parse(input);

		Console.WriteLine($"\nFull map:\n{map}\n");
		Console.WriteLine($"\nVisible trees:\n{map.VisibleToString()}\n");
		Console.WriteLine($"Number of visible trees: {map.VisibleTreeCount}");

		Console.WriteLine($"Maximum scenic score among the trees is: {map.MaxScenicScore}");

		Console.WriteLine($"Tree with best scenic score:\n{map.TreeWithHighestScenicScore}");

	}

	// A rope is represented by two points, tail and end, on a 2D grid. The tail must always be adjacent
	// to the head horizontally, vertically or diagonally, or be overlapping it. If the head is two squares
	// away in a cardinal direction, the tail moves one square in that direction to catch up. If the head
	// is two squares away in one direction and one square in another, the tail moves diagonally to catch
	// up.
	//
	// The head and tail start in the same square.
	//
	// Given a list of moves the head makes, find the number of unique squares the tail visits when
	// the moves are executed.
	//
	// Then do the same with a ten-point rope!
	public static void day9()
	{
		Console.WriteLine("\n  Day 9: Rope Bridge\n");

		List<string> input = Files.GetContentAsList("input/day-9.txt");

		rb.Rope r = new rb.Rope(2);

		r.Move(input);

		Console.WriteLine("\tRope with 2 segments:\n");
		r.Show();

		r = new rb.Rope(10);

		r.Move(input);

		Console.WriteLine("\tRope with 10 segments:\n");
		r.Show();
	}

	// Measure the signal strengths of a device during the 20th cycle and during every 40th cycle after that
	// (20, 60, 100, 140, 180, 220). The signal strength is the cycle number multiplied by the content
	// of the devices register, X. The device takes two commands:
	//   - "addx N" adds N to the value of the register and takes two cycles to complete.
	//     During processing the value of the register is not changed. N can be negative.
	//   - "noop" does nothing and takes one cycle to complete.
	//
	// First, find the sum of the interesting signal strengths.
	public static void day10()
	{
		Console.WriteLine("\n  Day 10: Cathode-Ray Tube:\n");

		List<string> input = Files.GetContentAsList("input/day-10.txt");

		List<string> testInput = Files.GetContentAsList("input/day-10-test.txt");

		crt.Device device = new crt.Device(testInput);

		crt.Measurement m = device.run();

		int[] want = new int[] { 420, 1140, 1800, 2940, 2880, 3960 };

		bool ok = true;
		int i = 0;
		foreach (int w in want)
		{
			Console.WriteLine($"Reading {i}: {m[i]}");
			if (m[i] != w)
			{
				ok = false;
			}
		}
		Console.WriteLine($"Test measurement: {m}, test {(m.Sum == 13140 && ok ? "OK" : "NOT OK")}");
	}
}
