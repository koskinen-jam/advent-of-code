public class Primes
{
	public static List<int> Known { get; set; } = new List<int>();

	public static int LastKnown {
		get
		{
			return Known.Count > 0 ? Known[Known.Count - 1] : 0;
		}
	}

	public static bool IsPrime(int n)
	{
		if (LastKnown * LastKnown < n) {
			GenerateUpTo((int) Math.Ceiling(Math.Sqrt(n)));
		}

		int last = Known.FindLastIndex((int p) => { return p * p < n; });

		for (int i = 0; i <= last; i++)
		{
			if (n % Known[i] == 0)
			{
				return false;
			}
		}

		return true;
	}

	// Generate primes up to a maximum
	public static void GenerateUpTo(int max)
	{
		if (Known.Count > 0 && Known[Known.Count - 1] > max)
		{
			return;
		}

		int min = Known.Count == 0 ? 2 : Known[Known.Count - 1] + 1;

		Sieve(GetRange(min, max));
	}

	// Find the primes among a range of numbers and add them to known primes.
	// The argument is expected to be consequent integers.
	public static void Sieve(int?[] numbers)
	{
		Stack<int> removals = new Stack<int>();

		if (numbers.Length == 0)
		{
			return;
		}

		int first = (int) numbers[0]!;
		int last = (int) numbers[numbers.Length - 1]!;

		// If not starting from 2, remove multiples of primes discovered earlier
		numbers = RemoveKnown(numbers);

		// find first number that was not removed just now
		int i = 0;
		while (i < numbers.Length && numbers[i] == null)
		{
			i++;
		}

		if (i >= numbers.Length)
		{
			return;
		}

		// Use Eulers prime sieve. For a range of numbers, mark the first one as prime, then
		// iterate over the rest of the array, multiplying the marked prime with each of the
		// remaining numbers and marking those for removal (since they are a multiple of a prime,
		// they are not prime themselves). Remove the marked numbers and move on to the next
		// remaining number, which is now prime, since it is not a multiple of any of the previous
		// numbers. Continue until the last number in the list is smaller than the square of the
		// currently chosen prime. Once its multiples have been removed, the remaining numbers
		// in the list are coprime, i.e. none of them is a multiple of any other.
		bool done = false;
		while (! done)
		{
			// Pick the number to examine
			int p = (int) numbers[i]!;
			Known.Add(p);

			int j = i;

			// Mark multiples for removal
			while (numbers[j] * p - first < numbers.Length)
			{
				removals.Push((int) numbers[j]! * p - first);
				do
				{
					j++;
				} while (numbers[j] == null && j < numbers.Length);
			}

			// Remove marked multiples
			while (removals.Count > 0)
			{
				int rm = removals.Pop();
				
				numbers[rm] = null;
			}

			// Are we done?
			if (p * p > last)
			{
				done = true;
			}
			else
			{
				// if not, move on to the next remaining number
				do
				{
					i++;
				} while (numbers[i] == null);
			}
		};

		// Add the remaining numbers that were not added earlier to known primes
		for (i++; i < numbers.Length; i++)
		{
			if (numbers[i] != null)
			{
				Known.Add((int) numbers[i]!);
			}
		}
 	}

	// Remove multiples of known primes from a range of numbers
	private static int?[] RemoveKnown(int?[] numbers)
	{
		int last = (int) numbers[numbers.Length - 1]!;

		foreach (int p in Known)
		{
			if (p * p > last)
			{
				break;
			}

			int i = 0;
			while (i < numbers.Length && (numbers[i] == null || numbers[i] % p != 0))
			{
				i++;
			}

			do
			{
				numbers[i] = null;
				i += p;
			} while (i < numbers.Length);
		}

		return numbers;
	}

	// Create a range of numbers [min, max]
	public static int?[] GetRange(int min, int max)
	{
		int?[] ret = new int?[max - min];

		for (int i = 0; i < ret.Length; i++)
		{
			ret[i] = min + i;
		}

		return ret;
	}
}
