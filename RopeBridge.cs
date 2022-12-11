using TreeHeightMap;
using System.Text;
using System.Text.RegularExpressions;

namespace RopeBridge
{
	// One node of a simulated rope
	public class RopePoint : Point
	{
		// List containing locations of this point after every move
		public List<Point> Trace { get; set; }

		// Current location of this rope point
		public Point Location { get { return new Point(X, Y); } }

		// Create a new RopePoint
		public RopePoint(int x, int y, List<Point>? trace = null) : base(x, y) {
			Trace = (trace != null) ? trace : new List<Point>();
			Trace.Add(Location);
		} 

		// RopePoint + Offset => RopePoint; Does not create a new instance, returns self after
		// updating location
		public static RopePoint operator +(RopePoint a, Offset o) => a.MoveByOffset(o);

		// Move this point by an offset
		public RopePoint MoveByOffset(Offset o)
		{
			X += o.dX;
			Y += o.dY;
			Trace.Add(Location);
			return this;
		}
	}

	// A simulated rope of a certain length
	public class Rope
	{
		public static Regex parseMove = new Regex(@"(?<dir>[UDLR]) (?<count>\d+)");

		// Characters used to draw the rope body in maps
		public static char[] BodyChars = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

		// Nodes of the rope
		public List<RopePoint> Points { get; }

		// Special nodes we're interested in
		public RopePoint Head { get { return Points[0]; } set { Points[0] = value; } }
		public RopePoint Tail { get { return Points[Points.Count - 1]; } }

		// Map dimensions for visualization
		public int XMax { get; set; } = 0;
		public int XMin { get; set; } = 0;
		public int YMax { get; set; } = 0;
		public int YMin { get; set; } = 0;

		// 
		private int W { get { return 1 + XMax - XMin; } }
		private int H { get { return 1 + YMax - YMin; } }

		// Count the number of unique locations the tail has been at
		public int UniqueTailLocations {
			get
			{
				HashSet<Point> tp = new HashSet<Point>(Tail.Trace, new PointsEqual());

				return tp.Count;
			}
		}

		// Create a new rope of a given length
		public Rope(int length = 2)
		{
			if (length < 2)
			{
				throw new Exception("Cannot make ropes shorter than 2");
			}

			Points = new List<RopePoint>();

			for (int i = 0; i < length; i++)
			{
				Points.Add(new RopePoint(0, 0));
			}
		}

		// Move the rope head by an offset and simulate the movement of the rest of the points
		public void Move(Offset o)
		{
			Head += o;
			for (int i = 1; i < Points.Count; i++)
			{
				MoveFollower(Points[i - 1], Points[i]);
			}

			if (Head.X < XMin)
			{
				XMin = Head.X;
			}
			else if (Head.X > XMax)
			{
				XMax = Head.X;
			}

			if (Head.Y < YMin)
			{
				YMin = Head.Y;
			}
			else if (Head.Y > YMax)
			{
				YMax = Head.Y;
			}
		}

		// Parse the direction and number of moves from a string like "U 5" for 5 moves up, and
		// execute those moves
		public void Move(string s)
		{
			Match m = parseMove.Match(s);

			if (! m.Success)
			{
				throw new Exception($"Cannot parse move \"{s}\"");
			}

			Offset o = Offset.Parse(m.Groups["dir"].Value);
			int count = int.Parse(m.Groups["count"].Value);

			for (int i = 0; i < count; i++)
			{
				Move(o);
			}
		}

		// Move a RopePoint towards another one
		private void MoveFollower(RopePoint head, RopePoint tail)
		{
			int dX = head.X - tail.X;
			int dY = head.Y - tail.Y;

			// tail and head overlap or are horizontally or vertically adjacent,
			// or they are diagonally adjacent (offset by (+-1, +-1)).
			if (Math.Abs(dX) + Math.Abs(dY) <= 1 || (Math.Abs(dX) == 1 && Math.Abs(dY) == 1))
			{
				tail += new Offset(0, 0);
				return;
			}

			if (dY == 0)
			{
				if (dX < 0)
				{
					tail += new Offset(-1, 0);
				}
				else
				{
					tail += new Offset(1, 0);
				}
				return;
			}
			else if (dX == 0)
			{
				if (dY < 0)
				{
					tail += new Offset(0, -1);
				}
				else
				{
					tail += new Offset(0, 1);
				}
				return;
			}
			else
			{
				Offset o = new Offset(dX < 0 ? -1 : 1, dY < 0 ? -1 : 1);
				tail += o;
			}
		}

		// Parse and execute all the moves in a list of strings
		public void Move(List<string> l)
		{
			foreach (string s in l)
			{
				Move(s);
			}
		}

		// Draw a map tracing the paths of the points at given indices (0 is head) and show it
		public void Show(int[]? indices = null)
		{
			if (indices == null)
			{
				indices = new int[] {0, Points.Count - 1};
			}

			char[,] map = getMap();

			Offset o = new Offset(XMin * -1, YMin * -1);
			foreach (int i in indices)
			{
				foreach (Point p in Points[i].Trace)
				{
					Point P = p + o;
					map[P.X, P.Y] = GetBodyChar(i);
				}
			}

			Console.WriteLine($"  Map:\n{stringifyMap(map)}\n Tail positions: {Tail.Trace.Count} / {UniqueTailLocations} unique\n");
		}

		// Draw and display a map that show the position of the rope at given step (defaults to
		// after latest move)
		public void Snapshot(int? step = null)
		{
			int i = step != null ? (int) step : Head.Trace.Count - 1;

			char[,] map = getMap();
			Offset o = new Offset(XMin * -1, YMin * -1);

			int charIdx = 0;
			foreach (RopePoint p in Points)
			{
				Point P = p.Trace[i] + o;
				if (map[P.X, P.Y] == '.')
				{
					map[P.X, P.Y] = GetBodyChar(charIdx);
				}
				charIdx++;
			}

			Console.WriteLine($"\n  Snapshot at step {i}:\n{stringifyMap(map)}");
		}

		// Return the character representing the i'th point in a rope. Head is always 'H'.
		private char GetBodyChar(int i)
		{
			if (i == 0)
			{
				return 'H';
			}
			else
			{
				return BodyChars[i % BodyChars.Length];
			}
		}

		// Prepare a character array for drawing a map
		private char[,] getMap()
		{
			char[,] ret = new char[W, H];

			for (int x = 0; x < W; x++)
			{
				for (int y = 0; y < H; y++)
				{
					ret[x, y] = '.';
				}
			}

			return ret;
		}

		// Turn a character array into a string
		private string stringifyMap(char[,] map)
		{
			StringBuilder sb = new StringBuilder((W + 1) * H);

			for (int y = 0; y < H; y++)
			{
				for (int x = 0; x < W; x++)
				{
					sb.Append(map[x, y]);
				}
				sb.Append("\n");
			}

			return sb.ToString();
		}

	}

}
