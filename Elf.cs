// Santas little helper, carries a backpack with calorie-filled snacks.
public class Elf
{
	private static int nextId = 1;
	private int id { get; }
	private Backpack backpack { get; }
	
	// Create a new elf
	public Elf ()
	{
		this.id = nextId++;
		this.backpack = new Backpack();
	}

	// Stringify this boi
	public override string ToString()
	{
		return $"Elf {id} carrying {backpack}";
	}

	// Add a snack with some calories to this elfs backpack
	public void AddCalories(int calories)
	{
		backpack.Add(calories);
	}

	// Create a list of elves from a list of integers.
	// ```
	// 100
	// 200
	//
	// 90
	//
	// 200
	// 500
	// ```
	// Each line in the input is expected to contain either an integer or nothing.
	// Elves are separated by empty lines
	public static List<Elf> parseFile(string path)
	{
		List<Elf> elves = new List<Elf>();
		Elf e = new Elf();

		foreach (string s in Files.GetContentAsList(path))
		{
			if (s == "")
			{
				elves.Add(e);
				e = new Elf();
			}
			else
			{
				e.AddCalories(int.Parse(s));
			}
		}

		if (! elves.Contains(e))
		{
			elves.Add(e);
		}

		return elves;
	}

	// Return the number of calories this elf is carrying in their backpack
	public int CaloriesCarried() 
	{
		return backpack.calorieCount();
	}

	// Compare two elves by their calories carried. Sorts in a descending order.
	public static int CompareByCaloriesDesc(Elf a, Elf b)
	{
		if (a.CaloriesCarried() == b.CaloriesCarried())
		{
			return 0;
		}
		else
		{
			return a.CaloriesCarried() > b.CaloriesCarried() ? -1 : 1;
		}
	}

	// Find the top N elves by calories carried from the given list
	public static List<Elf> topCalories(List<Elf> elves, int count)
	{
		elves.Sort(CompareByCaloriesDesc);
		return elves.GetRange(0, count);
	}

}
