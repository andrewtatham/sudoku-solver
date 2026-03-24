using SudokuSolver.Library;

/*
 * SudokuSolver Console App
 *
 * Usage:
 *   SudokuSolver.Console <puzzle>
 *   SudokuSolver.Console           (reads puzzle from stdin)
 *
 * The puzzle can be in any supported format:
 *   - 81-character string (digits 1-9, '0' or '.' for empty)
 *   - Multi-line grid (9 lines of 9 chars, '.' or '0' for empty)
 *   - SDK format (.sdk files with optional '#' comments)
 *   - Pencilmark/candidate format
 *
 * Example:
 *   SudokuSolver.Console 400050000105076009060400050900000510500090006027000008050004020200360104000010005
 */

string input;

if (args.Length > 0)
{
    input = string.Join(" ", args);
}
else
{
    // Read from stdin (allows piping puzzle files)
    input = Console.In.ReadToEnd();
}

Grid puzzle;
try
{
    puzzle = PuzzleParser.Parse(input);
}
catch (FormatException ex)
{
    Console.Error.WriteLine($"Error parsing puzzle: {ex.Message}");
    Environment.Exit(1);
    return;
}

Console.WriteLine("Puzzle:");
Console.WriteLine(puzzle.ToDisplayString());

var solver = new Solver();
Grid? solution;
try
{
    solution = solver.Solve(puzzle);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Invalid puzzle: {ex.Message}");
    Environment.Exit(2);
    return;
}

if (solution == null)
{
    Console.Error.WriteLine("No solution found.");
    Environment.Exit(3);
}
else
{
    Console.WriteLine("Solution:");
    Console.WriteLine(solution.ToDisplayString());
    Console.WriteLine("Puzzle string:");
    Console.WriteLine(solution.ToPuzzleString());
}
