// Thematic wrapper for a list of integers
public class Backpack
{
	private List<int> calories;

	// Create a new Backpack
	public Backpack ()
	{
		this.calories = new List<int>();
	}

	// Stringify this object
	public override string ToString()
	{
		return $"backpack with {calories.Count} items totaling {calorieCount()} calories";
	}

	// Add a snack represented by its calories to this backpack
	public void Add(int snack)
	{
		calories.Add(snack);
	}

	// Return the total calories contained in this backpack
	public int calorieCount()
	{
		int totalCalories = 0;

		foreach (int edible in calories)
		{
			totalCalories += edible;
		}

		return totalCalories;
	}
}
