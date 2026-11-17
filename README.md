# parallel_sharps

Лабораторная по параллельному программированию на C# (System.Threading.Tasks).
Три задания; сборка и запуск — в GitHub Actions (см. лог последней джобы).

## Условие

Для n = 0..N, N = 200, аргументы xn = 100·cos(n). Функция

f(x) = Σ_{k=1}^{∞} Σ_{j=1}^{∞} (x²·k) / (x² + k³ + j²) · cos((k+j)·x)

приближается частичной суммой с границей M = max(20, 20·|x|):

f(x) ≈ Σ_{k=1}^{M} Σ_{j=1}^{M} (x²·k) / (x² + k³ + j²) · cos((k+j)·x)

### I — массив Y
Заполнить Y[n] = f(xn). Сравнить `for` и `Parallel.For`.

### II — сумма S
Вычислить S = Σ_{n=0}^{N} f(xn) по массиву X. Сравнить `foreach` и `Parallel.ForEach`.

### III — контейнеры
То же, что II, но X хранится в `List`, `Stack` и `Queue`. Сравнить `foreach` и `Parallel.ForEach`.

## Структура

- `src/Task1` — задание I
- `src/Task2` — задание II
- `src/Task3` — задание III
- `.github/workflows/ci.yml` — сборка и запуск на ubuntu-latest

## Как посмотреть результаты

После `git push` откройте репозиторий → вкладка **Actions** → последний запуск →
разверните шаги **Run Task I / II / III** — там значения и замеры времени.

Локально: `dotnet run --project src/Task1 -c Release` и т.д.
