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

// Fill the three containers with the same set of values x[n] = 100*cos(n).
var list = new List<double>(N + 1);
for (int n = 0; n <= N; n++)
    list.Add(100.0 * Math.Cos(n));

var stack = new Stack<double>(N + 1);
for (int n = 0; n <= N; n++)
    stack.Push(100.0 * Math.Cos(n));

var queue = new Queue<double>(N + 1);
for (int n = 0; n <= N; n++)
    queue.Enqueue(100.0 * Math.Cos(n));

Console.WriteLine($"Task III: S = sum(n=0..N) f(x[n]) over List / Stack / Queue, N = {N}");
Console.WriteLine($"Logical processors available: {Environment.ProcessorCount}");
Console.WriteLine();

// Warm-up.
_ = F(1.0);
Parallel.ForEach(list.Take(4), v => F(1.0));

foreach (var (name, container) in new (string Name, IEnumerable<double> Values)[]
{
    ("List", list),
    ("Stack", stack),
    ("Queue", queue),
})
{
    var sw = new Stopwatch();

    double sFor = 0.0;
    sw.Restart();
    foreach (var xi in container)
        sFor += F(xi);
    sw.Stop();
    var tFor = sw.Elapsed;

    double sPar = 0.0;
    object gate = new();
    sw.Restart();
    Parallel.ForEach(
        container,
        () => 0.0,
        (xi, _, local) => local + F(xi),
        local => { lock (gate) { sPar += local; } });
    sw.Stop();
    var tPar = sw.Elapsed;

    Console.WriteLine($"{name}:");
    Console.WriteLine($"  first elements : {string.Join(", ", container.Take(5).Select(v => v.ToString("F2")))}");
    Console.WriteLine($"  S (foreach)           : {sFor:R}");
    Console.WriteLine($"  S (Parallel.ForEach)   : {sPar:R}");
    Console.WriteLine($"  difference            : {Math.Abs(sFor - sPar):E2}");
    Console.WriteLine($"  foreach          : {tFor.TotalMilliseconds,10:F2} ms");
    Console.WriteLine($"  Parallel.ForEach : {tPar.TotalMilliseconds,10:F2} ms");
    Console.WriteLine($"  Speedup          : {(tFor.TotalMilliseconds / Math.Max(1.0, tPar.TotalMilliseconds)),10:F2}x");
    Console.WriteLine();
}
