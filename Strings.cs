namespace Strings
{
	// Steps through a string picking sub-buffers of set length and checking if they fulfill
	// a given rule.
	public class Finder
	{
		// Rule to analyze
		private Func<char[], int, bool> analyzer;

		// Length of buffer passed to analyzer function
		public int BufferLength { get; set; }

		// Current position
		public int Position { get; private set; } = 0;

		// Construct a new finder with given analyzer function and buffer length
		public Finder(Func<char[], int, bool> analyzer, int length)
		{
			this.BufferLength = length;
			this.analyzer = analyzer;
		}

		// Return the number of strings that have been analyzed when the buffer matches the rule,
		// or -1 if no match is found.
		public int Find(string s)
		{
			for (Position = 0; Position < s.Length; Position++)
			{
				if (analyzer(sliceUpTo(s, Position), BufferLength))
				{
					return Position + 1;
				}	
			}

			return -1;
		}

		// Get a substring that ends at position end and is at most the length of the
		// current buffer.
		private char[] sliceUpTo(string s, int end)
		{
			int start = end + 1 < BufferLength ? 0 : end + 1 - BufferLength;
			int actualLength = end + 1 < BufferLength ? end + 1 : BufferLength;

			char[] ret = new char[actualLength];
			s.CopyTo(start, ret, 0, actualLength);

			return ret;
		}

		// Returns true if given character array is of expected length, and all its characters
		// are distinct.
		public static Func<char[], int, bool> AllUniqueChars = (char[] chars, int length) => {
			if (chars.Length < length)
			{
				return false;
			}

			for (int i = 0; i < chars.Length - 1; i++)
			{
				for (int j = i + 1; j < chars.Length; j++)
				{
					if (chars[i] == chars[j])
					{
						return false;
					}
				}
			}

			return true;
		};
	}
}
