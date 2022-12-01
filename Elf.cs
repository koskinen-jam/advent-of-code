public class Elf
{
	private static int nextId = 1;
	private int id { get; }
	private Backpack backpack { get; }
	
	public Elf ()
	{
		this.id = nextId++;
		this.backpack = new Backpack();
	}

	public override string ToString()
	{
		return $"Elf {id} carrying {backpack}";
	}
}
