namespace SudokuSolver.Library;

/// <summary>
/// Parses Sudoku puzzles from various common text formats.
/// </summary>
/// <remarks>
/// Supported formats:
/// <list type="bullet">
///   <item><b>Simple string</b> – 81 characters, digits 1-9 for givens, '0' or '.' for empty cells.</item>
///   <item><b>Grid format</b> – 9 lines of 9 characters each (with optional separator lines and whitespace).</item>
///   <item><b>SDK format</b> – SadMan/SudoCue .sdk files with optional '#' comment lines.</item>
///   <item><b>Pencilmark format</b> – grid lines where candidates are listed per cell separated by spaces.</item>
/// </list>
/// </remarks>
public static class PuzzleParser
{
    private static readonly char[] EmptyCellChars = { '0', '.', '_', 'X', 'x', '*', ' ' };

    /// <summary>
    /// Parses a Sudoku puzzle from any supported text format.
    /// Returns a <see cref="Grid"/> with the given clues, or throws <see cref="FormatException"/>
    /// if the input cannot be parsed.
    /// </summary>
    public static Grid Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Input is empty.");

        // Normalize line endings
        input = input.Replace("\r\n", "\n").Replace('\r', '\n');

        // Try SDK format (has '#' comment lines or SDK header)
        if (input.Contains('#') || IsSdkFormat(input))
            return ParseSdkFormat(input);

        // Try simple 81-char string (possibly with interleaved whitespace/pipes)
        var digits = ExtractDigits(input);
        if (digits.Length == Grid.TotalCells)
            return ParseDigitArray(digits);

        // Try multi-line grid (with optional separator lines)
        var gridDigits = ExtractGridDigits(input);
        if (gridDigits.Length == Grid.TotalCells)
            return ParseDigitArray(gridDigits);

        // Try pencilmark format
        var pmDigits = ParsePencilmarkFormat(input);
        if (pmDigits != null)
            return pmDigits;

        throw new FormatException(
            $"Unable to parse Sudoku puzzle. Expected 81 cells but found {digits.Length} digit/empty characters.");
    }

    private static bool IsSdkFormat(string input)
    {
        // SDK files often start with a line containing only dots and digits of length 9
        var lines = input.Split('\n');
        int gridLines = 0;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == Grid.Size && trimmed.All(c => char.IsDigit(c) || c == '.'))
                gridLines++;
        }
        return gridLines == Grid.Size;
    }

    private static Grid ParseSdkFormat(string input)
    {
        var lines = input.Split('\n');
        var cells = new int[Grid.TotalCells];
        int row = 0;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            // Skip comment lines and empty lines
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            // Skip separator lines (e.g. "---+---+---" or "=========")
            if (IsSeparatorLine(line))
                continue;

            if (row >= Grid.Size)
                continue;

            // Extract cells from this grid line
            var rowCells = ExtractRowCells(line);
            if (rowCells.Count == Grid.Size)
            {
                for (int c = 0; c < Grid.Size; c++)
                    cells[row * Grid.Size + c] = rowCells[c];
                row++;
            }
        }

        if (row != Grid.Size)
            throw new FormatException($"SDK format: expected {Grid.Size} grid rows, found {row}.");

        return new Grid(cells);
    }

    private static Grid? ParsePencilmarkFormat(string input)
    {
        // Pencilmark format: each row is a line, cells separated by spaces or pipes.
        // Each cell entry is either a single digit, or a string of candidate digits, or '.' / '0'.
        var lines = input.Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !IsSeparatorLine(l) && !l.StartsWith('#'))
            .ToList();

        if (lines.Count != Grid.Size)
            return null;

        var cells = new int[Grid.TotalCells];

        for (int r = 0; r < Grid.Size; r++)
        {
            // Split by spaces and/or pipes
            var parts = lines[r]
                .Split(new char[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);

            // Filter out separators like "+" that appear between box groups
            parts = parts.Where(p => p != "+" && !p.All(c => c == '-')).ToArray();

            if (parts.Length != Grid.Size)
                return null;

            for (int c = 0; c < Grid.Size; c++)
            {
                var part = parts[c];
                if (part.Length == 1 && (char.IsDigit(part[0]) || EmptyCellChars.Contains(part[0])))
                {
                    cells[r * Grid.Size + c] = IsEmptyChar(part[0]) ? 0 : part[0] - '0';
                }
                else if (part.All(char.IsDigit) && part.Length > 1)
                {
                    // Pencilmarks: multiple candidates → treat as empty (unsolved)
                    cells[r * Grid.Size + c] = 0;
                }
                else if (IsEmptyChar(part[0]) || part == "." || part == "0")
                {
                    cells[r * Grid.Size + c] = 0;
                }
                else
                {
                    return null;
                }
            }
        }

        return new Grid(cells);
    }

    /// <summary>
    /// Extracts exactly 81 digit/empty values from a simple string format.
    /// Accepts 0, '.', '_', 'X', 'x', '*' as empty cell indicators.
    /// Whitespace, pipes '|', dashes '-', plus '+', and colons ':' are ignored.
    /// </summary>
    private static int[] ExtractDigits(string input)
    {
        var result = new List<int>(Grid.TotalCells);
        foreach (char c in input)
        {
            if (char.IsDigit(c))
                result.Add(c - '0');
            else if (IsEmptyChar(c))
                result.Add(0);
            // Ignore structural characters
        }
        return result.ToArray();
    }

    private static int[] ExtractGridDigits(string input)
    {
        var result = new List<int>(Grid.TotalCells);
        var lines = input.Split('\n');

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || IsSeparatorLine(line) || line.StartsWith('#'))
                continue;

            foreach (char c in line)
            {
                if (char.IsDigit(c))
                    result.Add(c - '0');
                else if (c == '.' || c == '_')
                    result.Add(0);
                // Ignore pipes, spaces, dashes used as separators
            }
        }

        return result.ToArray();
    }

    private static List<int> ExtractRowCells(string line)
    {
        var cells = new List<int>();
        foreach (char c in line)
        {
            if (char.IsDigit(c))
                cells.Add(c - '0');
            else if (c == '.' || c == '_')
                cells.Add(0);
            // Ignore '|', '-', '+', spaces
        }
        return cells;
    }

    private static Grid ParseDigitArray(int[] digits)
    {
        if (digits.Length != Grid.TotalCells)
            throw new FormatException($"Expected {Grid.TotalCells} cells, found {digits.Length}.");
        return new Grid(digits);
    }

    private static bool IsSeparatorLine(string line)
    {
        return line.All(c => c is '-' or '+' or '=' or '|' or ':' or ' ');
    }

    private static bool IsEmptyChar(char c) => EmptyCellChars.Contains(c);
}
