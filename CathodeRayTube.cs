using System.Text;
using System.Text.RegularExpressions;

namespace CathodeRayTube
{

	// Result of a series of measurements made while the device executes its program
	public class Measurement
	{
		public List<int> Values { get; }

		public MeasurementSchedule Schedule { get; }

		// Total of measurement values
		public int Sum {
			get
			{
				int total = 0;
				foreach (int v in Values)
				{
					total += v;
				}
				return total;
			}
		}

		public int this[int key]
		{
			get => Values[key];
			set => Values[key] = value;
		}

		// Construct a new measurement set
		public Measurement(MeasurementSchedule? schedule = null)
		{
			schedule ??= new EvenIntervalsSchedule(20, 40, 220);

			Schedule = schedule;

			Values = new List<int>();
		}

		// Construct a new measurement set
		public Measurement(int first, int interval, int last = 0)
		{
			Schedule = new EvenIntervalsSchedule(first, interval, last);
			Values = new List<int>();
		}

		// Is it a cycle we should take a measurement in?
		public bool ShouldMeasure(int cycle)
		{
			return Schedule.ShouldMeasure(cycle);

		}

		// Add a measurement result
		public void Add(int val)
		{
			Values.Add(val);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append($"Measurement:\n\tSchedule: {Schedule}\n\tResults: ");
			sb.Append(string.Join(", ", Values));
			sb.Append($"\n\tTotal {Sum}");

			return sb.ToString();
		}
	}

	// Defines an interface for measurement schedules, that determine the cycle numbers at which
	// measurements should be taken
	public abstract class MeasurementSchedule
	{
		public abstract bool ShouldMeasure(int cycle);
	}

	// Takes measurements at given cycles
	public class SpecificCyclesSchedule : MeasurementSchedule
	{
		public int[] Cycles;
		private int Idx;

		public SpecificCyclesSchedule(int[] cycles)
		{
			Cycles = cycles;
			Array.Sort(Cycles);

			Idx = 0;
		}

		public override bool ShouldMeasure(int cycle)
		{
			if (Idx >= Cycles.Length)
			{
				return false;
			}
			else if (Cycles[Idx] == cycle)
			{
				Idx++;
				return true;
			}
			else
			{
				return false;
			}
		}

		public SpecificCyclesSchedule Clone()
		{
			return new SpecificCyclesSchedule(Cycles);
		}

		public override string ToString()
		{
			return $"At cycles {string.Join(", ", Cycles)}";
		}
	}

	// Takes the first measurement at given cycle, then every N cycles until optional last cycle has been
	// passed
	public class EvenIntervalsSchedule : MeasurementSchedule
	{
		public int First { get; }
		public int Last { get; }
		public int Interval { get; }

		public EvenIntervalsSchedule(int first, int interval, int last = 0)
		{
			First = first;
			Last = last;
			Interval = interval;
		}

		public override bool ShouldMeasure(int cycle)
		{
			if (Last > First && cycle > Last)
			{
				return false;
			}
			else
			{
				return ((cycle - First) % Interval) == 0;
			}
		}

		public EvenIntervalsSchedule Clone()
		{
			return new EvenIntervalsSchedule(First, Interval, Last);
		}

		public override string ToString()
		{
			return $"Every {Interval} cycles from cycle {First}{(Last > First ? $" to {Last}" : "")}";
		}
	}

	public enum InstructionType
	{
		NOOP,
		ADDX,
	}

	// Instructions handled by the device. Either alter the value of the devices only register ("addx N")
	// or do nothing ("noop").
	public class Instruction
	{
		public InstructionType Type { get; }
		public int Value { get; }
		public int Cycles { get; set; }

		// Has a requisite number of cycles been spent processing this?
		public bool Done {
			get
			{
				switch (Type)
				{
					case InstructionType.NOOP:
						return Cycles >= 1;
					case InstructionType.ADDX:
						return Cycles >= 2;
					default:
						throw new Exception("Unknown instruction type");
				}
			}
		}

		public Instruction(InstructionType type, int val)
		{
			Type = type;
			Value = val;
		}

		// Parse an instruction. Instructions in input are either "noop" or "addx N",
		// where N is a positive or negative integer.
		public static Instruction Parse(string s)
		{
			if (s == "noop")
			{
				return new Instruction(InstructionType.NOOP, 0);
			}
			else
			{
				int v = int.Parse(s.Split(" ")[1]);
				return new Instruction(InstructionType.ADDX, v);
			}
		}

		// Parse a list of strings into an instruction queue.
		public static Queue<Instruction> ToQueue(List<string> program)
		{
			Queue<Instruction> q = new Queue<Instruction>();

			foreach (string s in program)
			{
				q.Enqueue(Parse(s));
			}

			return q;
		}

		// Mark a cycle spent working on this instruction
		public void Tick()
		{
			Cycles++;
		}

		// Apply this instruction to given register value, returning the result.
		public int Apply(int x)
		{
			return Type == InstructionType.NOOP ? x : x + Value;
		}

		public override string ToString()
		{
			return (Done ? "+" : "-") + (Type == InstructionType.NOOP ? "noop" : $"addx {Value}");
		}
	}

	// Monochrome display for the device. Draws lit pixels when a three-pixel-wide sprite coincides with
	// the position being drawn. The devices register value indicates the position of the middle pixel of
	// the sprite on a line from 0 to Width - 1. The pixels of the screen are drawn left to right, starting
	// with the top line. Y value of the pixel being drawn does not matter when deciding if the current
	// position and sprite overlap.
	public class Display
	{
		public int W { get; }
		public int H { get; }
		public char[,] Buffer { get; }
		public char Lit { get; set; } = '#';
		public char Unlit { get; set; } = '.';

		public Display(int w, int h, char init = '.')
		{
			W = w;
			H = h;
			Buffer = new char[W, H];
			SetAll(init);
		}

		// Convert cycle number to a screen location.
		public (int X, int Y) ToScreenCoords(int cycle)
		{
			int X = (cycle - 1) % W;
			int Y = (cycle - 1) / W;

			return (X, Y);
		}

		// Draw the pixel for given cycle and register value
		public void DrawCycle(int cycle, int reg)
		{
			(int X, int Y) pos = ToScreenCoords(cycle);

			if (pos.X == reg - 1 || pos.X == reg || pos.X == reg + 1)
			{
				Buffer[pos.X, pos.Y] = Lit;
			}
			else
			{
				Buffer[pos.X, pos.Y] = Unlit;
			}
		}

		// Reset screen to be all given character
		public void SetAll(char c)
		{
			for (int j = 0; j < H; j++)
			{
				for (int i = 0; i < W; i++)
				{
					Buffer[i, j] = c;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			char c;
			for (int i = 0; i < W; i++)
			{
				if (i % 5 == 0 && i % 10 != 0)
				{
					c = '+';
				}
				else if (i % 10 == 0)
				{
					c = $"{i / 10}"[0];
				}
				else
				{
					c = '-';
				}
				sb.Append(c);
			}
			sb.Append("\n");

			for (int j = 0; j < H; j++)
			{
				for (int i = 0; i < W; i++)
				{
					sb.Append(Buffer[i, j]);						
				}
				sb.Append("\n");
			}

			return sb.ToString();
		}
	}

	// Cathode-Ray Tube device that executes a program and draws a picture on its display
	public class Device
	{
		public List<string> Program { get; set; }
		private Queue<Instruction> InstructionQueue { get; set; }
		public Display Screen { get; }

		// Signal strength at current cycle
		public int SignalStrength { get { return X * Cycle; } }
		public int X { get; private set; }
		public int Cycle { get; private set; }

		// Construct a new device
		public Device(List<string> program)
		{
			Program = program;
			X = 1;
			Cycle = 0;
			InstructionQueue = Instruction.ToQueue(Program);
			Screen = new Display(40, 6);
		}

		// Run the program, returning a default set of measurements or taking custom measurements, if
		// given a measurement as an argument
		public Measurement Run(Measurement? m = null)
		{
			X = 1;
			Cycle = 0;
			InstructionQueue = Instruction.ToQueue(Program);

			m ??= new Measurement(20, 40);

			while (InstructionQueue.Count > 0)
			{
				Cycle++;

				Screen.DrawCycle(Cycle, X);

				if (m.ShouldMeasure(Cycle))
				{
					m.Add(SignalStrength);
				}

				Instruction itc = InstructionQueue.Peek();

				itc.Tick();

				if (itc.Done)
				{
					X = itc.Apply(X);
					InstructionQueue.Dequeue();
				}
			}

			return m;
		}
	}	
}
