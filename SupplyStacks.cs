using System.Text.RegularExpressions;

namespace SupplyStacks
{
	// Simple container for something. Appears as "[X]" in drawing.
	public class Crate
	{
		private static Regex rgx = new Regex(@"\[(?<content>[A-Z])\]"); 

		public string content { get; }

		// Construct a Crate from a string
		public Crate(string content)
		{
			this.content = content;
		}

		// Stringify this Crate
		public override string ToString()
		{
			return $"[{content}]";
		}

		// Parse a Crate string "[M]" into a Crate with content "M"
		public static Crate Parse(string s)
		{
			Match m = rgx.Match(s);

			if (! m.Success)
			{
				throw new Exception($"Cannot parse \"{s}\" as a crate.");
			}

			return new Crate(m.Groups["content"].Value);
		}
	}

	// A supply with numbered stacks of Crates
	public class Supply
	{
		// Regex for recognizing/parsing lines with crates in drawing
		private static Regex crateLine = new Regex(@"^(?:(?<crates>   |\[[A-Z]\]) ?)+$");

		// Regex for recognizing/parsing line with stack numbers in drawing
		private static Regex stackNumbers = new Regex(@"(?: (?<num>\d)  ?)+");

		// Regex for recognizing/parsing line with movement order
		private static Regex move = new Regex(@"^move (?<count>\d+) from (?<from>\d+) to (?<to>\d+)$");

		public Dictionary<int, Stack<Crate>> stacks { get; }

		// Number of crates in tallest stack
		public int Height {
			get
			{
				int h = 0;
				foreach (Stack<Crate> s in stacks.Values)
				{
					if (s.Count > h)
					{
						h = s.Count;
					}
				}
				return h;
			}
		}

		// Number of stacks in supply
		public int Width {
			get
			{
				return stacks.Keys.Count;
			}
		}

		// The word spelled out by the contents of the top crates of the
		// stacks in this supply.
		public string TopWord {
			get
			{
				string ret = "";
				foreach (Stack<Crate> stk in stacks.Values)
				{
					ret += stk.Peek().content;
				}
				return ret;
			}
		}

		// Does the supply have an old an busted crane that has to move
		// crates one at a time, or a new, hot one, that can move a number
		// of crates in one go?
		public bool MoveMultiple { get; set; } = false;

		// Construct a new Supply
		public Supply()
		{
			stacks = new Dictionary<int, Stack<Crate>>();
		}

		// Add a Crate to the given stack, initializing it if it does
		// not exist yet.
		public void Add(int stack, Crate crate)
		{
			if (! stacks.ContainsKey(stack))
			{
				stacks.Add(stack, new Stack<Crate>());
			}
			stacks[stack].Push(crate);
		}

		// Move `count` top crates one by one from fromStack to toStack
		public void Move(int count, int fromStack, int toStack)
		{

			if (! MoveMultiple)
			{
				for (int i = 0; i < count && stacks[fromStack].Count > 0; i++)
				{
					Add(toStack, stacks[fromStack].Pop());
				}
			}
			else {
				Stack<Crate> tmp = new Stack<Crate>();

				for (int i = 0; i < count && stacks[fromStack].Count > 0; i++)
				{
					tmp.Push(stacks[fromStack].Pop());
				}

				while (tmp.Count > 0)
				{
					Add(toStack, tmp.Pop());
				}			
			}
		}

		// Apply a move order in the format "move N from X to Y", where N
		// is the number of crates to move, and X and Y are the source and
		// target stacks numbers.
		public void Move(string str)
		{
			Match m = move.Match(str);

			if (! m.Success)
			{
				return;
			}

			Move(
				int.Parse(m.Groups["count"].Value),
				int.Parse(m.Groups["from"].Value),
				int.Parse(m.Groups["to"].Value)
			);
		}

		// Apply a list of moves
		public void Move(List<string> moves)
		{
			foreach (string str in moves)
			{
				Move(str);
			}
		}
		
		// Parse a Supply from a string list produced by
		// Files.GetContentAsList. Stops analyzing input once it finds
		// the line with stack numbers. (Assumed that input begins with
		// a drawing detailing the current state of the supply)
		public static Supply Parse(List<string> strs)
		{
			Stack<Match> crateLineMatches = new Stack<Match>();
			Match? stackNumbersMatch = null;

			// Find crate stack rows and stack numbers row 
			foreach (string str in strs)
			{
				Match m = crateLine.Match(str);

				if (m.Success)
				{
					crateLineMatches.Push(m);
					continue;
				}
				
				m = stackNumbers.Match(str);

				if (m.Success)
				{
					stackNumbersMatch = m;
					break;
				}
			}

			if (stackNumbersMatch == null)
			{
				throw new Exception("Could not find stack numbers");
			}

			// Turn captures stack numbers into keys for our stack
			// dictionary.
			CaptureCollection captures = stackNumbersMatch.Groups["num"].Captures;

			int[] keys = new int[captures.Count];
			int i = 0;

			foreach (Capture c in captures)
			{
				keys[i] = int.Parse(c.Value);
				i++;
			}

			// Start working backwards through the captured lines
			// with crates, parsing them into crates and inserting them
			// into appropriate stacks.
			Supply s = new Supply();

			while (crateLineMatches.Count > 0)
			{
				Match m = crateLineMatches.Pop();

				i = 0;
				foreach (Capture c in m.Groups["crates"].Captures)
				{
					if (c.Value == "   ")
					{
						i++;
						continue;
					}

					s.Add(keys[i], Crate.Parse(c.Value));
					i++;
				}
			}

			return s;
		}

		// Produce a drawing of what the stacks in the supply currently
		// look like.
		public override string ToString()
		{
			string[,] strs = new string[Width, Height + 1];

			// Stack indices on the last line
			int i = 0;
			int h = Height;
			foreach (int k in stacks.Keys)
			{
				strs[i, h] = $" {k} ";
				i++;
			}

			// Crate stacks, drawing left to right, bottom up
			// and filling with crate-sized spaces to the top when
			// each stack runs out.
			i = 0;
			foreach (Stack<Crate> stack in stacks.Values)
			{
				int j = Height - 1;

				// Stack.ToArray() returns the items in LIFO order,
				// so we need to reverse it
				Crate[] crates = stack.ToArray();
				Array.Reverse(crates);

				foreach (Crate crate in crates)
				{
					strs[i, j] = $"{crate}";
					j--;
				}
				while (j >= 0)
				{
					strs[i, j] = "   ";
					j--;
				}
				i++;
			}

			// Compose the prepared parts into a single string
			string str = "";
			for (i = 0; i < h + 1; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					str += strs[j, i];

					if (j < Width)
					{
						str += " ";
					}
				}
				if (i < h)
				{
					str += "\n";
				}
			}

			return str;
		}
	}
}
