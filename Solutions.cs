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

		Console.WriteLine("\n  Top 3 calorie-carrying elves:\n");

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
	public static void day2()
	{
		string input = "input/day-2.txt";

		Console.WriteLine("\n Rock Paper Scissors tournament:\n");

		foreach (string s in new string[]{"A", "B", "C"})
		{
			rps.Move my = new rps.Move(s);
			/* Console.Write($"{s} parsed is {my}."); */

			foreach (string t in new string[]{"X", "Y", "Z"})
			{
				rps.Move their = new rps.Move(t);
				/* Console.Write($" It {outcome} against {their}."); */
				Console.WriteLine($"{new rps.Round(my, their)}");
			}
			Console.WriteLine("");
		}
	}
}
