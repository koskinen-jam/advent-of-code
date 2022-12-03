namespace Rucksack
{
	public class Item
	{
		private int a = (int)'a'; // 97
		private int A = (int)'A'; // 65

		public string kind { get; }
		public int priority { get; }

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

		public override string ToString()
		{
			return $"Item {kind} ({priority})";
		}

		public static Item FromChar(char c)
		{
			return new Item($"{c}");
		}
	}

	public class Sack
	{
		
	}
	
}
