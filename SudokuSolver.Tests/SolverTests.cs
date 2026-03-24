using SudokuSolver.Library;

namespace SudokuSolver.Tests;

public class SolverTests
{
    private readonly Solver _solver = new Solver();

    // Puzzle from the problem statement
    private const string SamplePuzzle   = "400050000105076009060400050900000510500090006027000008050004020200360104000010005";
    private const string SampleSolution = "479158362135276489862439751984623517513897246627541938751984623298365174346712895";

    // "Escargot" by Arto Inkala (2006) - a notoriously hard puzzle
    private const string HardPuzzle   = "100007090030020008009600500005300900010080002600004000300000010040000007007000300";
    private const string HardSolution = "162857493534129678789643521475312986913586742628794135356478219241935867897261354";

    [Fact]
    public void Solve_SamplePuzzle_ReturnsSolution()
    {
        var puzzle = PuzzleParser.Parse(SamplePuzzle);
        var solution = _solver.Solve(puzzle);
        Assert.NotNull(solution);
        Assert.Equal(SampleSolution, solution!.ToPuzzleString());
    }

    [Fact]
    public void Solve_AlreadySolved_ReturnsSolution()
    {
        var grid = PuzzleParser.Parse(SampleSolution);
        var solution = _solver.Solve(grid);
        Assert.NotNull(solution);
        Assert.Equal(SampleSolution, solution!.ToPuzzleString());
    }

    [Fact]
    public void Solve_Solution_IsValid()
    {
        var puzzle = PuzzleParser.Parse(SamplePuzzle);
        var solution = _solver.Solve(puzzle);
        Assert.NotNull(solution);
        Assert.True(solution!.IsSolved);
    }

    [Fact]
    public void Solve_Solution_PreservesGivens()
    {
        var puzzle = PuzzleParser.Parse(SamplePuzzle);
        var solution = _solver.Solve(puzzle);
        Assert.NotNull(solution);
        for (int i = 0; i < Grid.TotalCells; i++)
        {
            if (puzzle[i] != 0)
                Assert.Equal(puzzle[i], solution![i]);
        }
    }

    [Fact]
    public void Solve_NoSolution_ReturnsNull()
    {
        // Impossible puzzle: two 5s in the first row
        const string impossible = "550000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var puzzle = PuzzleParser.Parse(impossible);
        // IsValid will be false, should throw
        Assert.Throws<ArgumentException>(() => _solver.Solve(puzzle));
    }

    [Fact]
    public void Solve_InvalidPuzzle_ThrowsArgumentException()
    {
        var grid = new Grid();
        grid[0, 0] = 1;
        grid[0, 1] = 1; // duplicate in row
        Assert.Throws<ArgumentException>(() => _solver.Solve(grid));
    }

    [Fact]
    public void Solve_DoesNotModifyInput()
    {
        var puzzle = PuzzleParser.Parse(SamplePuzzle);
        var originalString = puzzle.ToPuzzleString();
        _solver.Solve(puzzle);
        Assert.Equal(originalString, puzzle.ToPuzzleString());
    }

    [Fact]
    public void Solve_HardPuzzle_ReturnsSolution()
    {
        // "Escargot" by Arto Inkala (2006) - a notoriously hard puzzle
        var puzzle = PuzzleParser.Parse(HardPuzzle);
        var solution = _solver.Solve(puzzle);
        Assert.NotNull(solution);
        Assert.Equal(HardSolution, solution!.ToPuzzleString());
    }

    [Fact]
    public void GetCandidates_EmptyGrid_Returns1to9()
    {
        var grid = new Grid();
        var candidates = Solver.GetCandidates(grid, 0, 0);
        Assert.Equal(9, candidates.Count);
        for (int i = 1; i <= 9; i++)
            Assert.Contains(i, candidates);
    }

    [Fact]
    public void GetCandidates_CellWithConstraints_FiltersCorrectly()
    {
        var grid = new Grid();
        // Fill row 0 with 1-8, leaving 9 as the only candidate for cell [0,8]
        for (int c = 0; c < 8; c++)
            grid[0, c] = c + 1;
        var candidates = Solver.GetCandidates(grid, 0, 8);
        Assert.Equal(new HashSet<int> { 9 }, candidates);
    }
}
