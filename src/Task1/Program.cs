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
double[] yFor = new double[N + 1];
double[] yPar = new double[N + 1];

for (int n = 0; n <= N; n++)
    x[n] = 100.0 * Math.Cos(n);

Console.WriteLine($"Task I: fill Y[n] = f(x[n]), where x[n] = 100*cos(n), N = {N}");
Console.WriteLine($"Logical processors available: {Environment.ProcessorCount}");
Console.WriteLine();

// Warm-up (JIT for F and for the parallel infrastructure).
_ = F(1.0);
Parallel.For(0, 4, _ => { _ = F(1.0); });

var sw = new Stopwatch();

sw.Restart();
for (int n = 0; n <= N; n++)
    yFor[n] = F(x[n]);
sw.Stop();
var tFor = sw.Elapsed;

sw.Restart();
Parallel.For(0, N + 1, n => yPar[n] = F(x[n]));
sw.Stop();
var tPar = sw.Elapsed;

bool equal = true;
for (int n = 0; n <= N; n++)
{
    if (Math.Abs(yFor[n] - yPar[n]) > 1e-12 * Math.Max(1.0, Math.Abs(yFor[n])))
    {
        equal = false;
        break;
    }
}

Console.WriteLine($"Y from `for` equals Y from `Parallel.For`: {equal}");
Console.WriteLine();
Console.WriteLine($"{"n",4}  {"x[n]",14}  {"f(x[n])",18}");
for (int n = 0; n <= N; n++)
    Console.WriteLine($"{n,4}  {x[n],14:F6}  {yFor[n],18:F6}");

Console.WriteLine();
Console.WriteLine("Timing:");
Console.WriteLine($"  for          : {tFor.TotalMilliseconds,10:F2} ms");
Console.WriteLine($"  Parallel.For : {tPar.TotalMilliseconds,10:F2} ms");
Console.WriteLine($"  Speedup      : {(tFor.TotalMilliseconds / Math.Max(1.0, tPar.TotalMilliseconds)),10:F2}x");
