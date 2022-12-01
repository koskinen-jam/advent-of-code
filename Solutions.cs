// Solutions for advent of code (2022) puzzles
public class Solutions
{
	// Given a list of integers representing calories in snacks carried by elves,
	// find the amount of calories carried by the one with the most, and the total
	// carried by the three with the most.
	// Elves are separated by empty rows.
	public static void day1()
	{
		string day1input = "input/day-1.txt";

		List<Elf> elves = Elf.parseCalorieStream(File.OpenText(day1input));

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
}
