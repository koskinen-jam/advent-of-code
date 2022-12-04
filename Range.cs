// Id range for day 4 solution. Includes starting and ending ids.
public class IdRange
{
	public int first { get; }
	public int last { get; }

	// Create a new id range
	public IdRange(int first, int last)
	{
		this.first = first;
		this.last = last;
	}

	// Parse a string "M-N" into an IdRange
	public static IdRange Parse(string s)
	{
		string[] numbers = s.Split("-");
		return new IdRange(int.Parse(numbers[0]), int.Parse(numbers[1]));
	}

	// Parse a string "M-N,O-P" into an array of IdRanges
	public static IdRange[] ParsePair(string s)
	{
		string[] ranges = s.Split(",");
		return new IdRange[] {
			Parse(ranges[0]),
			Parse(ranges[1])
		};
	}

	// Stringify this IdRange
	public override string ToString()
	{
		return $"{first}-{last}";
	}

	// Return true if the other id range is fully contained in this one
	public bool Contains(IdRange other)
	{
		return first <= other.first && last >= other.last;
	}

	// Return true if this id range overlaps the other one
	public bool Overlaps(IdRange other)
	{
		return Contains(other)
			|| (first >= other.first && first <= other.last) 
			|| (last >= other.first && last <= other.last);
	}
}
