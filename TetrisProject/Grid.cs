namespace TetrisProject
{
    public class Grid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        //------------ rows & columns inside the grid
        public bool IsInside(int r, int c) 
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        //----------- cell empty/not
        public bool IsEmpty(int r, int c) 
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }

        //------------ row is full
        public bool IsFull(int r) 
        {
            for(int c = 0; c < Columns; c++) 
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        //------------ row if empty
        public bool IsRowEmpty(int r) 
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        //----------- Clear Row
        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r, c] = 0;
            }
        }

        //----------- move rows down
        private void MoveDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }

        //----------- читска і вниз
        public int FullClear()
        {
            int cleared = 0;

            for (int r = Rows-1; r >= 0; r--)
            {
                if (IsFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveDown(r, cleared);
                }
            }

            return cleared;
        }
    }
 }
