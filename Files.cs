// File-handling shortcuts
public class Files
{
	// Return the contents of a file as a list of strings
	public static List<string> GetContentAsList(string path)
	{
		List<string> lines = new List<string>();

		try
		{
			StreamReader sr = File.OpenText(path);
			string? line;

			while ((line = sr.ReadLine()) != null)
			{
				lines.Add(line);
			}
			return lines;
		}
		catch (Exception e)
		{
			Console.WriteLine($"Failed to read file: {e}");
			throw;
		}
	}
}
