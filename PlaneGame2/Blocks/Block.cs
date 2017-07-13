namespace PlaneGame2.Blocks
{
    public enum BlockType { Air, Solid, Glass };

    /// <summary> Data concerning a block </summary>
    public struct Block
    {
        #region PUBLIC PROPERTIES
        /// <summary> This block's name </summary>
        public string Name { private set; get; }

        /// <summary> The X position within the main texture for this block </summary>
        public byte TextureOffsetX { private set; get; }

        /// <summary> The Y position within the main texture for this block </summary>
        public byte TextureOffsetY { private set; get; }

        /// <summary> The ID of this block </summary>
        public byte ID { private set; get; }

        /// <summary> What type of block this is </summary>
        public BlockType Type { private set; get; }
        #endregion

        #region INTERNAL CONSTRUCTORS
        /// <summary> Creates a new block with the given name, texture offset, ID, and type </summary>
        /// <param name="name"></param>
        /// <param name="textureOffsetX"></param>
        /// <param name="textureOffsetY"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        internal Block(string name, byte textureOffsetX, byte textureOffsetY, byte id, BlockType type)
        {
            Name = name;
            TextureOffsetX = textureOffsetX;
            TextureOffsetY = textureOffsetY;
            ID = id;
            Type = type;
        }
        #endregion
    }

    /// <summary> Manages a register of blocks </summary>
    public static class BlockManager
    {
        #region PRIVATE FIELDS
        /// <summary> A register of blocks </summary>
        private static Block[] blocks = new Block[256];

        /// <summary> The next block to add into the register </summary>
        private static byte blockCounter = 0;
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary> Gets a Block from the list of registered blocks with the given index </summary>
        /// <param name="index"> The blockID </param>
        /// <returns> The Block with the given ID </returns>
        public static Block GetBlockFromID(byte index)
        {
            return blocks[index];
        }
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary> Adds a new block to the register </summary>
        /// <param name="name"> The name of the new block </param>
        /// <param name="textureOffsetX"> The X offset of the new block's texture </param>
        /// <param name="textureOffsetY"> The Y offset of the new block's texture </param>
        /// <param name="type"> The new block's type </param>
        private static void addBlock(string name, byte textureOffsetX, byte textureOffsetY, BlockType type)
        {
            blocks[blockCounter] = new Block(name, textureOffsetX, textureOffsetY, blockCounter, type);
            blockCounter++;
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary> Initialises blocks </summary>
        static BlockManager()
        {
            addBlock("Air", 0, 0, BlockType.Air);
            addBlock("Stone", 1, 0, BlockType.Solid);
            addBlock("Dirt", 2, 0, BlockType.Solid);
            addBlock("Glass", 2, 1, BlockType.Glass);
        }
        #endregion
    }
}