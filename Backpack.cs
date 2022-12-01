public class Backpack
{
	private List<int> calories;

	public Backpack ()
	{
		this.calories = new List<int>();
	}

	public override string ToString()
	{
		int totalCalories = 0;

		foreach (int edible in calories)
		{
			totalCalories += edible;
		}

		return $"backpack with {calories.Count} items totaling {totalCalories} calories";
	}
}
