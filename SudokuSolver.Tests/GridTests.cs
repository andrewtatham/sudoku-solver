using SudokuSolver.Library;

namespace SudokuSolver.Tests;

public class GridTests
{
    [Fact]
    public void EmptyGrid_AllCellsAreZero()
    {
        var grid = new Grid();
        for (int i = 0; i < Grid.TotalCells; i++)
            Assert.Equal(0, grid[i]);
    }

    [Fact]
    public void Grid_SetAndGetCell_RowCol()
    {
        var grid = new Grid();
        grid[0, 0] = 5;
        grid[8, 8] = 9;
        Assert.Equal(5, grid[0, 0]);
        Assert.Equal(9, grid[8, 8]);
    }

    [Fact]
    public void Grid_SetAndGetCell_Index()
    {
        var grid = new Grid();
        grid[0] = 3;
        grid[80] = 7;
        Assert.Equal(3, grid[0]);
        Assert.Equal(7, grid[80]);
    }

    [Fact]
    public void Grid_InvalidValue_ThrowsArgumentOutOfRangeException()
    {
        var grid = new Grid();
        Assert.Throws<ArgumentOutOfRangeException>(() => grid[0, 0] = 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => grid[0, 0] = -1);
    }

    [Fact]
    public void Grid_IsComplete_FalseWhenEmpty()
    {
        Assert.False(new Grid().IsComplete);
    }

    [Fact]
    public void Grid_IsComplete_TrueWhenAllFilled()
    {
        var cells = Enumerable.Repeat(1, Grid.TotalCells).ToArray();
        Assert.True(new Grid(cells).IsComplete);
    }

    [Fact]
    public void Grid_IsValid_EmptyGridIsValid()
    {
        Assert.True(new Grid().IsValid());
    }

    [Fact]
    public void Grid_IsValid_DetectsDuplicateInRow()
    {
        var grid = new Grid();
        grid[0, 0] = 5;
        grid[0, 1] = 5;
        Assert.False(grid.IsValid());
    }

    [Fact]
    public void Grid_IsValid_DetectsDuplicateInColumn()
    {
        var grid = new Grid();
        grid[0, 0] = 3;
        grid[1, 0] = 3;
        Assert.False(grid.IsValid());
    }

    [Fact]
    public void Grid_IsValid_DetectsDuplicateInBox()
    {
        var grid = new Grid();
        grid[0, 0] = 7;
        grid[1, 1] = 7;
        Assert.False(grid.IsValid());
    }

    [Fact]
    public void Grid_Clone_IsIndependent()
    {
        var grid = new Grid();
        grid[0, 0] = 5;
        var clone = grid.Clone();
        clone[0, 0] = 9;
        Assert.Equal(5, grid[0, 0]);
        Assert.Equal(9, clone[0, 0]);
    }

    [Fact]
    public void Grid_ToPuzzleString_ReturnsCorrectLength()
    {
        var grid = new Grid();
        var s = grid.ToPuzzleString();
        Assert.Equal(81, s.Length);
        Assert.All(s, c => Assert.Equal('0', c));
    }

    [Fact]
    public void Grid_GetBoxIndex_CorrectForCorners()
    {
        Assert.Equal(0, Grid.GetBoxIndex(0, 0));
        Assert.Equal(2, Grid.GetBoxIndex(0, 8));
        Assert.Equal(6, Grid.GetBoxIndex(8, 0));
        Assert.Equal(8, Grid.GetBoxIndex(8, 8));
        Assert.Equal(4, Grid.GetBoxIndex(4, 4));
    }
}
