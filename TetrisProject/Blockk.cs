using System.Collections.Generic;

namespace TetrisProject
{
    public abstract class Block
    {
        //-------- position & block spawn + Id for ditinguish
        protected abstract Position[][] Tiles { get;  }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        //--------- constuctor = start offset
        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        //--------return block position
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        //--------- rotate block 90 degrees CW
        public void RotateCW()
        {
            rotationState = (rotationState +1) % Tiles.Length;
        }

        //-------- rotate clockwise CCW
        public void RotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        //-------- move block
        public void Move(int Rows, int Columns)
        {
            offset.Row += Rows;
            offset.Column += Columns;
        }

        //--------- reset rotation & position
        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }

    }
}
