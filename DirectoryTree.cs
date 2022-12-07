using System.Text;
using System.Text.RegularExpressions;

namespace DirectoryTree
{
	public enum NodeType
	{
		File,
		Dir
	}

	// File or directory in a directory tree
	public class Node
	{
		public NodeType Type { get; }
		public string Name { get; }
		public int Size {
			get
			{
				return Type == NodeType.File ? fileSize : DirSize();
			}
		}

		// Full path of a directory
		public string Path {
			get
			{
				return GetPath();
			}
		}

		public bool IsFile {
			get
			{
				return Type == NodeType.File;
			}
		}

		private int fileSize { get; }

		public Node? Parent { get; set; }
		public List<Node> Children { get; }

		// Construct a new node
		public Node(NodeType type, string name, int size = 0, Node? parent = null)
		{
			this.Type = type;
			this.Name = name;
			this.fileSize = size;
			this.Parent = parent;

			this.Children = new List<Node>();
		}

		// Calculate the total size of a directory
		private int DirSize()
		{
			int total = 0;
			foreach (Node n in Children)
			{
				total += n.Size;
			}
			return total;
		}

		// Get the full path of a directory
		private string GetPath()
		{
			Stack<Node> p = new Stack<Node>();

			Node at = this;
			p.Push(at);
			
			while (at.Parent != null)
			{
				at = at.Parent;
				p.Push(at);
			}

			StringBuilder sb = new StringBuilder((1 + p.Count) * 10);

			while (p.Count > 0)
			{
				at = p.Pop();
				sb.Append(at.Name == "/" ? "/" : at.Name + "/");
			}

			return sb.ToString();
		}

		// Add a child to a directory
		public void Add(Node n)
		{
			if (IsFile)
			{
				throw new Exception("Cannot add a child to a file");
			}
			else
			{
				n.Parent = this;
				Children.Add(n);
			}
		}

		// Create a file node and add it as a child
		public void AddFile(string name, int size)
		{
			Add(new Node(NodeType.File, name, size));
		}

		// Create a directory node and add it as a child
		public void AddDir(string name)
		{
			Add(new Node(NodeType.Dir, name));
		}

		// Create a directory node
		public static Node Dir(string name)
		{
			return new Node(NodeType.Dir, name);
		}

		// Create a file node
		public static Node File(string name, int size)
		{
			return new Node(NodeType.File, name, size);
		}

		// Stringify node
		public override string ToString()
		{
			return $"{(IsFile ? "FILE" : "DIR ")} {Name, -15} {Size}";
		}

		// Return a string containing the path, size and contained files and directories of
		// this node
		public string List(bool recursive = false)
		{
			StringBuilder sb = new StringBuilder(50 * (1 + Children.Count));

			Queue<Node> dirs = new Queue<Node>();

			sb.Append($"{Path} ({Size})\n");

			foreach (Node n in Children)
			{
				sb.Append(n.ToString()).Append("\n");
				if (! n.IsFile)
				{
					dirs.Enqueue(n);
				}
			}

			if (recursive)
			{
				foreach (Node n in dirs)
				{
					sb.Append("\n");
					sb.Append(n.List(true));
				}
			}

			return sb.ToString();
		}

		// Check if this node has a child with the given name
		public bool Has(string name)
		{
			foreach (Node n in Children)
			{
				if (n.Name == name)
				{
					return true;
				}
			}
			return false;
		}

		// Get the child node with the given name, if it exists
		public Node? Get(string name)
		{
			foreach (Node n in Children)
			{
				if (n.Name == name)
				{
					return n;
				}
			}
			return null;
		}

		// Find the nodes matching the given predicate in the tree starting with this node
		public List<Node> Find(Func<Node, bool> match)
		{
			List<Node> result = new List<Node>();

			if (match(this))
			{
				result.Add(this);
			}
			foreach (Node n in Children)
			{
				if (n.IsFile && match(n))
				{
					result.Add(n);
				}
				else
				{
					foreach (Node m in n.Find(match))
					{
						result.Add(m);
					}
				}
			}

			return result;
		}

		// Sort nodes in ascending order by size
		public static int CompareNodesBySize(Node a, Node b)
		{
			if (a.Size == b.Size)
			{
				return 0;
			}
			else
			{
				return a.Size < b.Size ? -1 : 1;
			}
		}
	}



	// Input contains commands (one per line) or their output (one to many lines).
	// Lines with commands start with a '$' and contain one of four commands:
	// - 'cd /': Move to directory tree root
	// - 'cd X': Move in, into directory X from current directory
	// - 'cd ..': Move out from the current directory, towards the root
	// - 'ls': List the files and directories contained in current directory
	// 
	// The output of 'ls' can be two types:
	// - 'dir xxx': A directory named "xxx"
	// - '20032 foo.bar': A file named "foo.bar" with size 20032.
	//
	// The directory root '/' always exists.
	public class Parser
	{
		private int Pos;
		private List<string> Lines;
		private Node root;
		private Node current;
		private static Regex dirRe = new Regex(@"dir (?<name>\w+)");
		private static Regex fileRe = new Regex(@"(?<size>\d+) (?<name>\w+)");

		// Create a new Parser
		public Parser(List<string> lines)
		{
			Lines = lines;
			root = Node.Dir("/");
			current = root;
		}

		// Parse a list of strings into a directory tree and return the root of the resulting tree
		public Node Parse(List<string>? newLines = null)
		{
			if (newLines != null)
			{
				Lines = newLines;
			}

			Pos = 0;
			root = Node.Dir("/");
			current = root;

			while (Pos < Lines.Count)
			{
				if (! IsCommand(Peek(Pos)!))
				{
					Pos++;
					continue;
				}
				Execute(Peek(Pos)!);
				Pos++;
			}

			return root;
		}

		// Return the string at the given position
		private string? Peek(int? i = null)
		{
			if (i == null)
			{
				i = Pos;
			}

			if (i < 0 || i >= Lines.Count)
			{
				return null;
			}
			else
			{
				return Lines[(int)i];
			}
		}

		// Return true if given string is a command (e.g. "$ cd ..")
		private bool IsCommand(string s)
		{
			return s[0] == '$';
		}

		// Execute a command string
		private void Execute(string s)
		{
			if (s.Substring(2, 2) == "cd")
			{
				Move(s.Substring(5));
			}
			else if (s.Substring(2,2) == "ls")
			{
				List();
			}
			else
			{
				throw new Exception($"Cannot parse \"{s}\"");
			}
		}

		// Execute a directory change command, moving to the root directory, up a level or down
		// into a named subdirectory if it exists
		private void Move(string s)
		{
			if (s == "/")
			{
				current = root;
				return;
			}
			else if (s == "..")
			{
				if (current.Parent == null)
				{
					throw new Exception($"Trying to move out from {current.Path} but its parent is null D:");
				}
				current = current.Parent;
				return;
			}
			else
			{
				Node? dir = current.Get(s);
				if (dir != null && ! dir.IsFile)
				{
					current = dir;
				}
				else
				{
					throw new Exception($"Trying to move to \"{s}\" from {current.Path} but there is no such node D:");
				}
			}
		}

		// Execute a list command, parsing the following non-command lines into files and directories
		// contained in the current node.
		private void List()
		{
			int i = Pos + 1;

			do {
				if (Peek(i) == null)
				{
					throw new Exception("Psh, out of bounds D:");
				}
				string s = Peek(i)!;
				if (s[0] == 'd')
				{
					Match m = dirRe.Match(s);
					if (! m.Success)
					{
						throw new Exception($"Could not parse obvious directory {s}");
					}

					current.Add(Node.Dir(m.Groups["name"].Value));
				}
				else
				{
					Match m = fileRe.Match(s);
					if (! m.Success)
					{
						throw new Exception($"Could not parse obvious file {s}");
					}

					current.Add(Node.File(m.Groups["name"].Value, int.Parse(m.Groups["size"].Value)));
				}
				i++;
			} while (i < Lines.Count && ! IsCommand(Peek(i)!));
		}
	}
}
