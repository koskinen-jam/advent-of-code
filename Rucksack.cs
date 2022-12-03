// Day 3 solution business logic classes
namespace Rucksack
{
	// An item stored in a rucksack. Represented by a single character [a-zA-Z].
	// Characters also represent priority with a-z being 1-26 and A-Z being 27-52.
	public class Item
	{
		private int a = (int)'a'; // 97
		private int A = (int)'A'; // 65

		public string kind { get; }
		public int priority { get; }

		// Create a new item from a string
		public Item(string item)
		{
			if (item.Length != 1)
			{
				throw new Exception("Must be one character exactly.");
			}

			this.kind = item;

			int charValue = (int)item[0];

			if (a <= charValue)
			{
				priority = 1 + charValue - a;
			}
			else
			{
				priority = 27 + charValue - A;
			}
		}

		// Stringify this item
		public override string ToString()
		{
			return $"Item {kind} ({priority})";
		}

		// Create a new item from a char
		public static Item FromChar(char c)
		{
			return new Item($"{c}");
		}
	}

	// A rucksack with two compartments.
	public class Sack
	{
		public Dictionary<string,List<Item>>[] compartments { get; }
		public Dictionary<string,List<Item>> itemsByKind { get; }
		public List<Item> items { get; }

		// Create a new Sack
		public Sack()
		{
			compartments = new Dictionary<string,List<Item>>[] {
				new Dictionary<string,List<Item>>(),
				new Dictionary<string,List<Item>>()
			};

			itemsByKind = new Dictionary<string,List<Item>>();

			items = new List<Item>();
		}

		// Add an item to one of the compartments of this Sack.
		public void Add(int compartment, Item item)
		{
			Dictionary<string,List<Item>> cpt = compartments[compartment];

			try
			{
				cpt[item.kind].Add(item);
			}
			catch (KeyNotFoundException)
			{
				cpt[item.kind] = new List<Item>();
				cpt[item.kind].Add(item);
			}

			try
			{
				itemsByKind[item.kind].Add(item);
			}
			catch (KeyNotFoundException)
			{
				itemsByKind[item.kind] = new List<Item>();
				itemsByKind[item.kind].Add(item);
			}

			items.Add(item);
		}

		// Stringify this sack
		public override string ToString()
		{
			string s = "";

			foreach (Item i in items)
			{
				s += i.kind;
			}

			Item? common = FindCommon();

			string commonItem = common == null ? "none" : $"{common}";

			return $"Rucksack containing {s}. Common item: {commonItem}";
		}

		// Find the Item that appears in both compartments of this Sack.
		public Item? FindCommon()
		{
			foreach (string key in compartments[0].Keys)
			{
				if (compartments[1].ContainsKey(key))
				{
					return compartments[0][key][0];
				}
			}
			return null;
		}

		// Check if this Sack has an item of the given kind.
		public bool HasKind(string kind)
		{
			return itemsByKind.ContainsKey(kind);
		}

		// Find the Item that appears in this and the other two Sacks.
		public Item? FindCommon(Sack second, Sack third)
		{
			foreach (string key in itemsByKind.Keys)
			{
				if (second.HasKind(key) && third.HasKind(key))
				{
					return itemsByKind[key][0];
				}
			}
			return null;
		}

		// Parse a string into a Sack.
		public static Sack FromString(string str)
		{
			Sack sack = new Sack();

			for (int i = 0; i < str.Length; i++)
			{
				sack.Add(
					i < str.Length / 2 ? 0 : 1,
					Item.FromChar(str[i])
				);
			}

			return sack;
		}
	}
}
