namespace TetrisProject
{
    public class TetrisState
    {
        //--------back field for block
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                //------- Spawn blocks in visible raws
                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }
            public Grid Grid { get; }
            public Queue Queue { get; }
            public bool GameOver { get; private set; }
            public int Score { get; private set; }
            public Block HoldBlock { get; private set; }
            public bool CanHold { get; private set; }

            //-------const for grid
            public TetrisState()
        {
            Grid = new Grid(22, 10);
            Queue = new Queue();
            CurrentBlock = Queue.GetUpdate();
            CanHold = true;
        }

        //-------Check block position
        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!Grid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        //-------- rotate block CW & CCW iif posible
        public void RotateCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        //------hold method (hold current get next + swap blocks)
        public void Hold()
        {
            if(!CanHold)
            {
                return;
            }

            if (HoldBlock == null)
            {
                HoldBlock = CurrentBlock;
                CurrentBlock = Queue.GetUpdate();
            }
            else
            {
                Block swap = CurrentBlock;
                CurrentBlock = HoldBlock;
                HoldBlock = swap;
            }

            CanHold = false; //------prevenr spamming hold
        }

        //--------- move block left & right
        public void MoveLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        //---------GameOver checker
        private bool IsGameOver()
        {
            return !(Grid.IsRowEmpty(0) && Grid.IsRowEmpty(1));
        }

        //--------- if block cannot move down + score + hold
        private void BlockPlace()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                Grid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += Grid.FullClear();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = Queue.GetUpdate();
                CanHold = true;
            }
        }

        //--------- move dosn block + call BlockPlace if cant movee down
        public void MoveDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                BlockPlace();
            }
        }

        //------ method that take position and retutn num of empty cells below object
        private int TileDistance(Position p)
        {
            int drop = 0;

            while (Grid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDistance()
        {
            int drop = Grid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDistance(p));
            }

            return drop;
        }

        //------- drop method
        public void Drop()
        {
            CurrentBlock.Move(BlockDistance(), 0);
            BlockPlace();
        }
    }
}
