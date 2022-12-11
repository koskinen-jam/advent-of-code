using System.Text;

namespace TreeHeightMap
{
	// A tree with a height value and visibility from four cardinal directions, as well as
	// general visibility determined by if it is visible from at least one cardinal direction.
	public class Tree
	{
		public Point Location { get; }
		public int Height { get; }

		public bool Visible {
			get
			{
				if (Views.Count == 0)
				{
					return false;
				}

				foreach (View v in Views.Values)
				{
					if (! v.Blocked)
					{
						return true;
					}
				}

				return false;
			}
		}

		public int ScenicScore {
			get
			{
				int score = 1;
				foreach (View v in Views.Values)
				{
					score *= v.Length;
				}
				return score;
			}
		}

		public Dictionary<Offset, View> Views { get; }

		// Construct a new Tree
		public Tree(Point location, int height)
		{
			this.Location = location;
			this.Height = height;

			this.Views = new Dictionary<Offset, View>();
		}

		// Stringify tree and its views
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Tree at {Location}. {(Visible ? "Visible" : "Not visible")}.");
			sb.Append($" Height {Height}, scenic score {ScenicScore}");

			foreach (Offset o in Views.Keys)
			{
				sb.Append($"\n\tView to {o}: {Views[o]}");
			}

			return sb.ToString();
		}
	}

	// 2D direction/offset
	public class Offset
	{
		public int dX { get; }
		public int dY { get; }

		public Offset(int dx, int dy)
		{
			dX = dx;
			dY = dy;
		}

		public override string ToString()
		{
			return $"({dX}, {dY})";
		}

		public static Offset Up()
		{
			return new Offset(0, -1);
		}

		public static Offset Down()
		{
			return new Offset(0, 1);
		}

		public static Offset Left()
		{
			return new Offset(-1, 0);
		}

		public static Offset Right()
		{
			return new Offset(1, 0);
		}

		public static Offset Parse(string s)
		{
			switch (s)
			{
				case "U":
					return Up();
				case "D":
					return Down();
				case "L":
					return Left();
				case "R":
					return Right();
				default:
					throw new Exception($"Unknown direction {s}");
			}
		}
	}

	// Shorthand names for cardinal directions
	public class Dir
	{
		public static Offset N {
			get
			{
				return Offset.Up();
			}
		}

		public static Offset E {
			get
			{
				return Offset.Right();
			}
		}

		public static Offset S {
			get
			{
				return Offset.Down();
			}
		}

		public static Offset W {
			get
			{
				return Offset.Left();
			}
		}

		public static Offset[] All {
			get
			{
				return new Offset[] {N, E, S, W};
			}
		}

	}

	// 2D point
	public class Point
	{
		public int X { get; protected set; }
		public int Y { get; protected set; }

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		// Clone this point
		public Point Clone()
		{
			return new Point(X, Y);
		}

		// Return next point after applying offset to this one
		public Point Next(Offset dir)
		{
			return new Point(X + dir.dX, Y + dir.dY);
		}

		// Return true if this point is within the bounds of a given map
		public bool IsInBounds(Map map)
		{
			return X >= 0 && X < map.Width && Y >= 0 && Y < map.Height;
		}

		// Stringify this point
		public override string ToString()
		{
			return $"[{X}, {Y}]";
		}

		// Point + Offset => Point
		public static Point operator +(Point a, Offset o) => new Point(a.X + o.dX, a.Y + o.dY);
	}

	// Comparator used by HashSets to exclude equal points
	public class PointsEqual : EqualityComparer<Point>
	{
		// Returns true if two points have the same coordinates
		public override bool Equals(Point? a, Point? b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			else if (a == null || b == null)
			{
				return false;
			}
			else
			{
				return a.X == b.X && a.Y == b.Y;
			}
		}

		// If Equals(a, b) == true, GetHashCode(a) must equal GetHashCode(b)
		public override int GetHashCode(Point p)
		{
			int hCode = p.X ^ p.Y;
			return hCode.GetHashCode();
		}
	}

	// A view from a point on a map towards a direction. Is blocked by trees that are as tall or taller
	// than the one at view origin.
	public class View
	{
		public Point Origin { get; }
		public Offset Dir { get; }
		public List<Tree> Seen { get; }
		public bool Blocked { get; }
		public int Length {
			get
			{
				return Seen.Count;
			}
		}

		// Construct a new View, looking towards dir from origin on given map
		public View(Map map, Point origin, Offset dir)
		{
			Origin = origin;
			Dir = dir;
			Seen = new List<Tree>();

			Tree firstTree = map.TreeAt(origin);
			Point location = origin;
			Point next;

			Blocked = false;

			// Cannot see original tree.
			// Sees non-blocking trees.
			// Sees blocking tree.
			// Does not see past map edge.

			while ((next = location.Next(Dir)).IsInBounds(map))
			{
				location = next;
				Tree current = map.TreeAt(location);
				Seen.Add(current);

				if (current.Height >= firstTree.Height)
				{
					Blocked = true;
					break;
				}
			}
		}

		// Stringify view
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"From {Origin} at {Dir}: ");
			foreach (Tree t in Seen)
			{
				sb.Append(t.Height);
			}

			return sb.ToString();
		}
	}

	// Map of tree heights and visibilities
	public class Map
	{
		public Tree[,] Trees { get; set; }

		public Tree TreeAt(int x, int y)
		{
			return Trees[x, y];
		}

		public Tree TreeAt(Point p)
		{
			return Trees[p.X, p.Y];
		}

		public void SetTree(int x, int y, Tree t)
		{
			Trees[x, y] = t;
		}

		public void SetTree(Point p, Tree t)
		{
			SetTree(p.X, p.Y, t);
		}

		// Map width
		public int Width {
			get
			{
				return Trees.GetUpperBound(0) + 1;
			}
		}

		// Map height
		public int Height {
			get
			{
				return Trees.GetUpperBound(1) + 1;
			}
		}

		public int VisibleTreeCount {
			get
			{
				int total = 0;
				foreach (Tree t in Trees)
				{
					total += t.Visible ? 1 : 0;
				}
				return total;
			}
		}

		public int MaxScenicScore {
			get
			{
				int max = 0;
				foreach (Tree t in Trees)
				{
					if (t.ScenicScore > max)
					{
						max = t.ScenicScore;
					}
				}
				return max;
			}
		}

		public Tree TreeWithHighestScenicScore {
			get
			{
				Tree best = Trees[0, 0];
				foreach (Tree t in Trees)
				{
					if (t.ScenicScore > best.ScenicScore)
					{
						best = t;
					}
				}
				return best;
			}
		}

		// Create a new Map from a tree matrix
		public Map(Tree[,] trees)
		{
			Trees = trees;
		}

		// Parse a list of strings into a map
		public static Map Parse(List<string> strs)
		{
			int xmax = strs[0].Length - 1;
			int ymax = strs.Count - 1;

			Tree[,] trees = new Tree[xmax + 1, ymax + 1];

			for (int y = 0; y <= ymax; y++)
			{
				for (int x = 0; x <= xmax; x++)
				{
					int height = int.Parse(strs[y].Substring(x, 1));
					trees[x, y] = new Tree(new Point(x, y), height);
				}
			}

			Map map = new Map(trees);
			map.BuildViews();

			return map;
		}

		// Build views in all the cardinal directions from every tree on the map
		public void BuildViews()
		{
			foreach (Tree t in Trees)
			{
				foreach (Offset dir in Dir.All)
				{
					t.Views[dir] = new View(this, t.Location, dir);
				}
			}
		}

		// Stringify this map
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(Trees.Length * 2);

			for (int y = 0; y < Width; y++)
			{
				for (int x = 0; x < Height; x++)
				{
					sb.Append(Trees[x, y].Height);
				}
				sb.Append("\n");
			}

			return sb.ToString();
		}

		// Show visible trees only
		public string VisibleToString()
		{
			StringBuilder sb = new StringBuilder(Trees.Length * 2);

			for (int y = 0; y < Width; y++)
			{
				for (int x = 0; x < Height; x++)
				{
					Tree t = Trees[x, y];
					sb.Append(t.Visible ? t.Height : ".");
				}
				sb.Append("\n");
			}

			return sb.ToString();
		}
	}
}
