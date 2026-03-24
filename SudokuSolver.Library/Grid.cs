namespace SudokuSolver.Library;

/// <summary>
/// Represents a 9x9 Sudoku grid.
/// </summary>
public class Grid
{
    private readonly int[] _cells;

    public const int Size = 9;
    public const int BoxSize = 3;
    public const int TotalCells = Size * Size;

    /// <summary>
    /// Initializes an empty grid (all cells = 0).
    /// </summary>
    public Grid()
    {
        _cells = new int[TotalCells];
    }

    /// <summary>
    /// Initializes a grid from a cell array (values 0-9, where 0 = empty).
    /// </summary>
    public Grid(int[] cells)
    {
        if (cells.Length != TotalCells)
            throw new ArgumentException($"Expected {TotalCells} cells, got {cells.Length}.");
        _cells = (int[])cells.Clone();
    }

    /// <summary>
    /// Gets or sets the value of a cell at row r (0-8) and column c (0-8).
    /// Value 0 represents an empty cell; 1-9 are the digits.
    /// </summary>
    public int this[int row, int col]
    {
        get => _cells[row * Size + col];
        set
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value), "Cell value must be 0-9.");
            _cells[row * Size + col] = value;
        }
    }

    /// <summary>
    /// Gets the flat cell value by index (0-80).
    /// </summary>
    public int this[int index]
    {
        get => _cells[index];
        set
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value), "Cell value must be 0-9.");
            _cells[index] = value;
        }
    }

    /// <summary>
    /// Returns true if the grid contains no empty cells.
    /// </summary>
    public bool IsComplete => Array.IndexOf(_cells, 0) == -1;

    /// <summary>
    /// Returns true if the grid is valid and complete (i.e. solved).
    /// </summary>
    public bool IsSolved => IsComplete && IsValid();

    /// <summary>
    /// Returns true if the current state of the grid does not violate any Sudoku constraints.
    /// </summary>
    public bool IsValid()
    {
        for (int i = 0; i < Size; i++)
        {
            if (!IsGroupValid(GetRow(i))) return false;
            if (!IsGroupValid(GetColumn(i))) return false;
            if (!IsGroupValid(GetBox(i))) return false;
        }
        return true;
    }

    private static bool IsGroupValid(int[] group)
    {
        bool[] seen = new bool[Size + 1];
        foreach (int v in group)
        {
            if (v == 0) continue;
            if (seen[v]) return false;
            seen[v] = true;
        }
        return true;
    }

    /// <summary>
    /// Returns the 9 values in the given row (0-8).
    /// </summary>
    public int[] GetRow(int row)
    {
        int[] result = new int[Size];
        for (int c = 0; c < Size; c++)
            result[c] = _cells[row * Size + c];
        return result;
    }

    /// <summary>
    /// Returns the 9 values in the given column (0-8).
    /// </summary>
    public int[] GetColumn(int col)
    {
        int[] result = new int[Size];
        for (int r = 0; r < Size; r++)
            result[r] = _cells[r * Size + col];
        return result;
    }

    /// <summary>
    /// Returns the 9 values in the 3x3 box identified by index 0-8 (row-major order of boxes).
    /// </summary>
    public int[] GetBox(int boxIndex)
    {
        int startRow = (boxIndex / BoxSize) * BoxSize;
        int startCol = (boxIndex % BoxSize) * BoxSize;
        int[] result = new int[Size];
        int idx = 0;
        for (int r = startRow; r < startRow + BoxSize; r++)
            for (int c = startCol; c < startCol + BoxSize; c++)
                result[idx++] = _cells[r * Size + c];
        return result;
    }

    /// <summary>
    /// Returns the 3x3 box index (0-8) for the given row and column.
    /// </summary>
    public static int GetBoxIndex(int row, int col) => (row / BoxSize) * BoxSize + col / BoxSize;

    /// <summary>
    /// Returns a deep copy of this grid.
    /// </summary>
    public Grid Clone() => new Grid(_cells);

    /// <summary>
    /// Returns the puzzle as an 81-character string (digits 1-9, '0' for empty).
    /// </summary>
    public string ToPuzzleString() => new string(_cells.Select(v => (char)('0' + v)).ToArray());

    /// <summary>
    /// Returns a human-readable multi-line grid representation with box separators.
    /// </summary>
    public string ToDisplayString()
    {
        var sb = new System.Text.StringBuilder();
        for (int r = 0; r < Size; r++)
        {
            if (r > 0 && r % BoxSize == 0)
                sb.AppendLine("------+-------+------");
            for (int c = 0; c < Size; c++)
            {
                if (c > 0 && c % BoxSize == 0)
                    sb.Append(" | ");
                else if (c > 0)
                    sb.Append(' ');
                int v = _cells[r * Size + c];
                sb.Append(v == 0 ? '.' : (char)('0' + v));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    /// Returns the puzzle as a 9-line grid string (digits 1-9, '.' for empty).
    /// </summary>
    public string ToGridString()
    {
        var sb = new System.Text.StringBuilder();
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                int v = _cells[r * Size + c];
                sb.Append(v == 0 ? '.' : (char)('0' + v));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
