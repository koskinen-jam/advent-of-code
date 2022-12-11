using System.Text;
using System.Text.RegularExpressions;

namespace MonkeyBusiness
{
	public class Item
	{
		private static Regex itemsParser = new Regex(@"Starting items: (?<worries>[, \d]+)");
		public int Worry { get; set; }

		public Item(int worry)
		{
			Worry = worry;
		}

		public override string ToString()
		{
			return $"Item with worry level {Worry}";
		}

		public static List<Item> Parse(string s)
		{
			Match m = itemsParser.Match(s);
			if (! m.Success)
			{
				throw new Exception($"{s} is not a string with an items list");
			}

			List<Item> res = new List<Item>();

			foreach (string worryLevel in m.Groups["worries"].Value.Split(", "))
			{
				res.Add(new Item(int.Parse(worryLevel)));
			}

			return res;
		}

		public static Queue<Item> ParseToQueue(string s)
		{
			return new Queue<Item>(Parse(s));
		}
	}

	public class MonkeyOp
	{
		private static Regex opParser = new Regex(@"Operation: new = old (?<operation>.) (?<operand>.*)$");
		private string Operation { get; }
		private string Operand { get; }

		public MonkeyOp(string operation, string operand)
		{
			Operation = operation;
			Operand = operand;
		}

		public static MonkeyOp Parse(string s)
		{
			Match m = opParser.Match(s);
			if (! m.Success)
			{
				throw new Exception($"{s} is not a monkey operation");
			}
			else
			{
				return new MonkeyOp(m.Groups["operation"].Value, m.Groups["operand"].Value);
			}			
		}

		public void OperateOn(Item i)
		{
			int v = Operand == "old" ? i.Worry : int.Parse(Operand);

			if (Operation == "+")
			{
				i.Worry += v;
			}
			else if (Operation == "*")
			{
				i.Worry *= v;
			}
			else
			{
				throw new Exception($"Unknown operation {Operation}");
			}
		}

		public override string ToString()
		{
			return $"new = old {Operation} {Operand}";
		}
	}

	public class MonkeyTest
	{
		private static Regex divisorParser = new Regex(@"Test: divisible by (?<divisor>\d+)");
		private static Regex outcomeParser = new Regex(@"If (?<condition>true|false): throw to monkey (?<id>\d+)");

		private int Divisor { get; }
		private Dictionary<bool, int> Outcomes { get; }

		public string Description { get { return $"Divisible by {Divisor}"; } }

		public MonkeyTest(int divisor)
		{
			Divisor = divisor;
			Outcomes = new Dictionary<bool, int>();
		}

		public static MonkeyTest Parse(string s)
		{

			Match m = divisorParser.Match(s);
			if (! m.Success)
			{
				throw new Exception($"{s} not parseable as a MonkeyTest");
			}
			else
			{
				return new MonkeyTest(int.Parse(m.Groups["divisor"].Value));
			}			
		}

		public void SetOutcome(bool val, int targetId)
		{
			Outcomes[val] = targetId;
		}

		public void SetOutcome(string s)
		{
			Match m = outcomeParser.Match(s);
			if (! m.Success)
			{
				throw new Exception($"{s} not parseable as a MonkeyTest outcome");
			}
			else
			{
				SetOutcome(bool.Parse(m.Groups["condition"].Value), int.Parse(m.Groups["id"].Value));
			}
		}

		public int Test(Item i)
		{
			return Outcomes[Result(i)];
		}

		public bool Result(Item i)
		{
			return i.Worry % Divisor == 0;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"Test divisible by {Divisor}:");
			foreach (bool b in Outcomes.Keys)
			{
				sb.Append($" {b} => {Outcomes[b]}.");
			}

			return sb.ToString();
		}
	}

	public class Monkey
	{
		private static Regex monkeyParser = new Regex(@"Monkey (?<id>\d+):");

		public int Id { get; }
		public Queue<Item> Items { get; }
		public MonkeyOp Operation { get; }
		public MonkeyTest Test { get; }
		public int Inspections { get; protected set; }
		
		public static Dictionary<int, Monkey> Monkeys = new Dictionary<int, Monkey>();

		public Monkey(int id, Queue<Item> items, MonkeyOp operation, MonkeyTest test)
		{
			Id = id;
			Items = items;
			Operation = operation;
			Test = test;
			Inspections = 0;

			Monkeys[Id] = this;
		}

		public static bool StartsMonkey(string s)
		{
			Match m = monkeyParser.Match(s);
			return m.Success;
		}

		public static int ParseId(string s)
		{
			Match m = monkeyParser.Match(s);
			if (! m.Success)
			{
				throw new Exception($"{s} is not parseable for monkey id");
			}
			else
			{
				return int.Parse(m.Groups["id"].Value);
			}
		}

		public static void ParseMonkeys(List<string> list)
		{
			Queue<string> q = new Queue<string>();

			int i = 0;


			while (i < list.Count)
			{
				if (StartsMonkey(list[i]))
				{
					while (i < list.Count && list[i] != "")
					{
						q.Enqueue(list[i]);
						i++;
					}
					int id = ParseId(q.Dequeue());
					Queue<Item> items = Item.ParseToQueue(q.Dequeue());
					MonkeyOp op = MonkeyOp.Parse(q.Dequeue());
					MonkeyTest test = MonkeyTest.Parse(q.Dequeue());
					while (q.Count > 0)
					{
						test.SetOutcome(q.Dequeue());
					}
					new Monkey(id, items, op, test);
				}
				i++;
			}
		}

		public static void ShowAll()
		{
			foreach (Monkey m in Monkeys.Values)
			{
				Console.WriteLine($"{m}\n");
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Monkey {Id}:\n  Items:");
			foreach (Item i in Items)
			{
				sb.Append($" {i.Worry},");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.Append($"\n  Operation: {Operation}");
			sb.Append($"\n  {Test}");

			return sb.ToString();
		}		

		public void Catch(Item i)
		{
			Items.Enqueue(i);
		}

		public void Throw(Item i, int target)
		{
			Monkeys[target].Catch(i);
		}

		public static bool Verbose = false;

		public int Examine(Item i)
		{
			string exam =	$"Monkey {Id} examines {i}.";

			Operation.OperateOn(i);
			string worryInc = $"\n  Worry increased to {i.Worry}.";

			i.Worry /= 3;
			string relax = $"\n  Relax down to {i.Worry}.";

			int target = Test.Test(i);
			string testing = $"\n  {Test.Description}: {Test.Result(i)}, throws to monkey {target}.";
			Inspections++;

			if (Verbose)
			{
				Console.WriteLine(exam + worryInc + relax + testing);
			}

			return target;
		}

		public void Turn()
		{
			while (Items.Count > 0)
			{
				Item i = Items.Dequeue();
				Throw(i, Examine(i));
			}
		}

		public static void Round()
		{
			foreach (Monkey m in Monkeys.Values)
			{
				m.Turn();
			}
		}

		public static void Rounds(int n)
		{
			while (n > 0)
			{
				Round();
				n--;
			}
		}

		public static List<Monkey> TopActive(int n = 2)
		{
			List<Monkey> list = new List<Monkey>(Monkeys.Values);
			list.Sort(new MonkeysByInspections());

			if (Verbose)
			{
				foreach (Monkey m in list)
				{
					Console.WriteLine($"Monkey {m.Id} with {m.Inspections} inspections.");
				}
			}

			return list.GetRange(list.Count - 2, 2);
		}

		public static int BusinessScore()
		{
			List<Monkey> top2 = TopActive(2);
			return top2[0].Inspections * top2[1].Inspections;
		}

		public static void Reset()
		{
			Monkeys = new Dictionary<int, Monkey>();
		}
 	}

	public class MonkeysByInspections : Comparer<Monkey>
	{
		public override int Compare(Monkey? a, Monkey? b)
		{
			if (a == null)
			{
				if (b == null)
				{
					return 0;
				}
				else
				{
					return -1;
				}
			}
			else if (b == null)
			{
				return 1;
			}
			else {
				return a.Inspections.CompareTo(b.Inspections);
			}
		}
	}
}
