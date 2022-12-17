using System.Numerics;

public class Factorization
{
	public Dictionary<int, int> Factors { get; set; }

	// Create a Factorization from a number
	public Factorization(int n)
	{
		Factors = new Dictionary<int, int>();

		foreach (int f in Factorize(n))
		{
			AddFactor(f);
		}
	}

	// Clone a Factorization
	public Factorization(Factorization f)
	{
		Factors = new Dictionary<int, int>(f.Factors);
	}

	// Add a factor (multiply with f^power)
	public void AddFactor(int f, int power = 1)
	{
		if (f == 1 || power == 0)
		{
			return;
		}

		if (f <= 0 || ! Primes.IsPrime(f))		
		{
			throw new Exception("Can only add positive prime factors");
		}

		try
		{
			Factors[f] += power;
			if (Factors[f] == 0)
			{
				Factors.Remove(f);
			}
		}
		catch (KeyNotFoundException)
		{
			Factors[f] = power;
		}
	}

	// Inversion

	public Factorization Invert()
	{
		Factorization result = new Factorization(this);

		foreach (int factor in result.Factors.Keys)
		{
			result.Factors[factor] *= -1;
		}

		return result;
	}

	// Multiplication

	public Factorization MultiplyWith(Factorization other)
	{
		Factorization result = new Factorization(this);

		foreach (int factor in other.Factors.Keys)
		{
			result.AddFactor(factor, other.Factors[factor]);
		}

		return result;
	}

	public Factorization MultiplyWith(int n)
	{
		return MultiplyWith(new Factorization(n));
	}

	public static Factorization operator *(Factorization a, Factorization b) => a.MultiplyWith(b);
	public static Factorization operator *(Factorization a, int n) => a.MultiplyWith(n);
	public static Factorization operator *(int n, Factorization a) => a.MultiplyWith(n);

	// Division

	public Factorization DivideBy(Factorization other)
	{
		return (new Factorization(this)).MultiplyWith(other.Invert());		
	}

	public Factorization DivideBy(int n)
	{
		return (new Factorization(this)).DivideBy(new Factorization(n));
	}

	public static Factorization operator /(Factorization a, Factorization b) => a.DivideBy(b);
	public static Factorization operator /(Factorization a, int n) => a.DivideBy(n);
	public static Factorization operator /(int n, Factorization a) => (new Factorization(n)).DivideBy(a);

	// Convert to integer
	public int ToInt()
	{
		int res = 1;
		Stack<int> divisors = new Stack<int>();

		foreach (int factor in Factors.Keys)
		{
			if (Factors[factor] < 0)
			{
				divisors.Push(factor);
				continue;
			}

			for (int i = 0; i < Factors[factor]; i++)
			{
				if (int.MaxValue / factor < res)
				{
					string err = $"{this} does not fit in an integer.";
					err += $" {res} * {factor} > {int.MaxValue}";
					err += $" on step {i + 1} of {factor}^{Factors[factor]}";

					throw new Exception(err);
				}
				res *= factor;
			}
		}

		int divideBy = 1;
		while (divisors.Count > 0)
		{
			int factor = divisors.Pop();

			for (int i = Factors[factor]; i < 0; i++)
			{
				divideBy *= factor;
			}
		}

		return res / divideBy;
	}

	public static explicit operator int(Factorization f) => f.ToInt();

	// Convert to BigInteger
	public BigInteger ToBigInteger()
	{
		BigInteger res = 1;
		Stack<int> divisors = new Stack<int>();

		foreach (int factor in Factors.Keys)
		{
			if (Factors[factor] < 0)
			{
				divisors.Push(factor);
				continue;
			}

			for (int i = 0; i < Factors[factor]; i++)
			{
				res *= factor;
			}
		}

		BigInteger divideBy = 1;
		while (divisors.Count > 0)
		{
			int factor = divisors.Pop();

			for (int i = Factors[factor]; i < 0; i++)
			{
				divideBy *= factor;
			}
		}

		return res / divideBy;
	}
	
	public static explicit operator BigInteger(Factorization f) => f.ToBigInteger();

	// Divisibility
	
	public bool IsDivisibleBy(Factorization f) {
		foreach (int factor in f.Factors.Keys)
		{
			try
			{
				if (Factors[factor] < f.Factors[factor])
				{
					return false;
				}
			}
			catch (KeyNotFoundException)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsDivisibleBy(int n)
	{
		if (Primes.IsPrime(n))
		{
			try
			{
				if (Factors[n] >= 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (KeyNotFoundException)
			{
				return false;
			}
		}
		else
		{
			return IsDivisibleBy(new Factorization(n));
		}
	}

	// Return a list of primes that are at most the square root of n
	public List<int> PossiblePrimeFactors(int n)
	{
		return Primes.Known.FindAll((int p) => { return p * p <= n; });
	}

	// Return n split into prime factors of first power. If a factor was of higher power, it appears
	// that many times in the result (e.g. 12 = {2, 2, 3})
	public List<int> Factorize(int n)
	{
		List<int> factors = new List<int>();

		foreach (int p in PossiblePrimeFactors(n))
		{
			while (n % p == 0)
			{
				factors.Add(p);
				n /= p;
			}
		}

		if (n > 1)
		{
			factors.Add(n);
		}

		return factors;
	}

	// Return a string representation of this factorization (b1^p1 * b2^p2 * ... * bn^pn)
	public override string ToString()
	{
		List<string> l = new List<string>();

		foreach (int factor in Factors.Keys)
		{
			int power = Factors[factor];

			l.Add(power == 1 ? $"{factor}" : $"{factor}^{Factors[factor]}");
		}

		return string.Join(" * ", l);
	}
}
