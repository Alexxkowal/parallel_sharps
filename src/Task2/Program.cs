using System.Diagnostics;

const int N = 200;

// f(x) = sum_{k=1..Inf} sum_{j=1..Inf} (x^2 * k) / (x^2 + k^3 + j^2) * cos((k + j) * x)
// The infinite double series is truncated at M = max(20, 20 * |x|).
static double F(double x)
{
    int m = Math.Max(20, (int)Math.Floor(20.0 * Math.Abs(x)));
    double x2 = x * x;
    double sum = 0.0;

    for (int k = 1; k <= m; k++)
    {
        double kk = k;
        double num = x2 * kk;              // numerator (k-dependent, j-independent)
        double k3 = kk * kk * kk;

        for (int j = 1; j <= m; j++)
        {
            double jj = j;
            double denom = x2 + k3 + jj * jj;
            sum += (num / denom) * Math.Cos((kk + jj) * x);
        }
    }

    return sum;
}

double[] x = new double[N + 1];
for (int n = 0; n <= N; n++)
    x[n] = 100.0 * Math.Cos(n);

Console.WriteLine($"Task II: S = sum(n=0..N) f(x[n]), where x[n] = 100*cos(n), N = {N}");
Console.WriteLine($"Logical processors available: {Environment.ProcessorCount}");
Console.WriteLine();

// Warm-up.
_ = F(1.0);
Parallel.ForEach(new[] { 1.0, 2.0, 3.0, 4.0 }, v => F(1.0));

var sw = new Stopwatch();

double sFor = 0.0;
sw.Restart();
foreach (var xi in x)
    sFor += F(xi);
sw.Stop();
var tFor = sw.Elapsed;

double sPar = 0.0;
object gate = new();
sw.Restart();
Parallel.ForEach(
    x,
    () => 0.0,
    (xi, _, local) => local + F(xi),
    local => { lock (gate) { sPar += local; } });
sw.Stop();
var tPar = sw.Elapsed;

Console.WriteLine("Array X:");
Console.WriteLine($"{"n",4}  {"x[n]",14}");
for (int n = 0; n <= N; n++)
    Console.WriteLine($"{n,4}  {x[n],14:F6}");

Console.WriteLine();
Console.WriteLine($"S (foreach)           : {sFor:R}");
Console.WriteLine($"S (Parallel.ForEach)   : {sPar:R}");
Console.WriteLine($"Difference            : {Math.Abs(sFor - sPar):E2}");
Console.WriteLine();
Console.WriteLine("Timing:");
Console.WriteLine($"  foreach          : {tFor.TotalMilliseconds,10:F2} ms");
Console.WriteLine($"  Parallel.ForEach : {tPar.TotalMilliseconds,10:F2} ms");
Console.WriteLine($"  Speedup          : {(tFor.TotalMilliseconds / Math.Max(1.0, tPar.TotalMilliseconds)),10:F2}x");
