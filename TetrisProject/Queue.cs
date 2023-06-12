using System;
using System.Security.Cryptography.X509Certificates;

namespace TetrisProject
{
    public class Queue
    {
        //-------- recycle 7 blocks + Random
        private readonly Block[] blocks = new Block[]
        {
            new Block_I(),
            new Block_L(),
            new Block_J(),
            new Block_Z(),
            new Block_O(),
            new Block_S(),
            new Block_T(),
        };

        private readonly Random random = new Random();

        //------- prop for next block
        public Block NextBlock {  get; private set; }

        //const for next random block
        public Queue()
        {
            NextBlock = RandomBlock();
        }

        //------- return random block
        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        //------- return next block + update prop
        public Block GetUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
