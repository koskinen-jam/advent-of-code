using System.Numerics;

/* Solutions.day1(); */
/* Solutions.day2(); */
/* Solutions.day3(); */
/* Solutions.day4(); */
/* Solutions.day5(); */
/* Solutions.day6(); */
/* Solutions.day7(); */
/* Solutions.day8(); */
/* Solutions.day9(); */
/* Solutions.day10(); */

/* Solutions.day11(); */

/* Primes.GenerateUpTo(50); */
/* Console.WriteLine($"{string.Join(", ", Primes.Known)}"); */
/* Primes.GenerateUpTo(10000); */
/* Console.WriteLine($"{string.Join(", ", Primes.Known)}"); */

foreach (int i in new int[] { 50, 80, 201, 79, 43, 49, 99, 7, 11, 13, 15, 1000 })
{
	Console.WriteLine($"{i} is prime: { Primes.IsPrime(i) } ({new Factorization(i)})");
}

Console.WriteLine($"180 * 181 * 182 = ({new Factorization(180)}) * ({new Factorization(181)}) * ({new Factorization(182)}) = {(new Factorization(180)) * (new Factorization(181)) * 182}");

Factorization f = new Factorization(1024);
f.AddFactor(7, -2);
f.AddFactor(2, -5);
f.AddFactor(3, 4);

Console.WriteLine($"{f} = {(int) f}");

f.AddFactor(2);
f.AddFactor(7);
f.AddFactor(3, -4);

Console.WriteLine($"{f} = {(int) f}");

f /= 4;

Console.WriteLine($"{f} = {(int) f}");

f *= 12;

Console.WriteLine($"{f} = {(int) f}");

f /= 12;

Console.WriteLine($"{f} = {(int) f}");

f *= 4;

Console.WriteLine($"{f} = {(int) f}");

f *= 1024;

Console.WriteLine($"{f} = {(int) f}");
f *= 1024;

Console.WriteLine($"{f} = {(int) f}");
f *= 1024;

Console.WriteLine($"{f} = {(BigInteger) f}");
f *= 9 * 9 * 11 * 13;

Console.WriteLine($"{f} = {(BigInteger) f}");

foreach (int i in new int[] {1, 2, 4, 8, 1024, 3, 9, 27, 18, 11, 99, 17, 29, 7, 11*11})
{
	Console.WriteLine($"{(BigInteger) f} {(f.IsDivisibleBy(i) ? "is" : "is not")} divisible by {i}");
}

