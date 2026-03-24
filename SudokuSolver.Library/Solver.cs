namespace SudokuSolver.Library;

/// <summary>
/// Solves Sudoku puzzles using backtracking with constraint propagation.
/// </summary>
public class Solver
{
    /// <summary>
    /// Attempts to solve the given puzzle grid.
    /// </summary>
    /// <param name="puzzle">The puzzle to solve. Must be a valid (possibly partial) grid.</param>
    /// <returns>
    /// A solved <see cref="Grid"/> if a solution exists, or <c>null</c> if no solution is found.
    /// The original puzzle grid is not modified.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the initial puzzle state is invalid.</exception>
    public Grid? Solve(Grid puzzle)
    {
        if (!puzzle.IsValid())
            throw new ArgumentException("The puzzle grid is invalid (violates Sudoku constraints).");

        var grid = puzzle.Clone();
        return Backtrack(grid);
    }

    private static Grid? Backtrack(Grid grid)
    {
        // Find the empty cell with the fewest candidates (minimum remaining values heuristic)
        int bestIndex = -1;
        int bestCount = int.MaxValue;

        for (int i = 0; i < Grid.TotalCells; i++)
        {
            if (grid[i] != 0) continue;

            int row = i / Grid.Size;
            int col = i % Grid.Size;
            var candidates = GetCandidates(grid, row, col);

            if (candidates.Count == 0) return null; // Dead end
            if (candidates.Count < bestCount)
            {
                bestCount = candidates.Count;
                bestIndex = i;
                if (bestCount == 1) break; // Can't do better
            }
        }

        if (bestIndex == -1)
            return grid; // All cells filled → solution found

        int bestRow = bestIndex / Grid.Size;
        int bestCol = bestIndex % Grid.Size;
        var bestCandidates = GetCandidates(grid, bestRow, bestCol);

        foreach (int candidate in bestCandidates)
        {
            grid[bestIndex] = candidate;
            var result = Backtrack(grid);
            if (result != null) return result;
            grid[bestIndex] = 0; // Undo
        }

        return null; // No solution found from this state
    }

    /// <summary>
    /// Returns the set of valid candidates for the cell at the given row and column.
    /// </summary>
    public static HashSet<int> GetCandidates(Grid grid, int row, int col)
    {
        var used = new bool[Grid.Size + 1];

        // Check row
        for (int c = 0; c < Grid.Size; c++)
            used[grid[row, c]] = true;

        // Check column
        for (int r = 0; r < Grid.Size; r++)
            used[grid[r, col]] = true;

        // Check 3x3 box
        int startRow = (row / Grid.BoxSize) * Grid.BoxSize;
        int startCol = (col / Grid.BoxSize) * Grid.BoxSize;
        for (int r = startRow; r < startRow + Grid.BoxSize; r++)
            for (int c = startCol; c < startCol + Grid.BoxSize; c++)
                used[grid[r, c]] = true;

        var candidates = new HashSet<int>();
        for (int v = 1; v <= Grid.Size; v++)
            if (!used[v]) candidates.Add(v);

        return candidates;
    }
}
