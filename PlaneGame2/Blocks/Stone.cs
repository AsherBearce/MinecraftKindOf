using System;
using System.Collections.Generic;
using PlaneGame2.Interfaces;
using PlaneGame2.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Blocks
{
    public class Stone : IBlock
    {
        public BlockType type { get { return BlockType.Solid; } }
        public byte ID { get { return 1; } }
        public string BlockName { get { return "Stone"; } }
        public Vector2 TextureAddress { get { return new Vector2(1, 0); } }

        public static void init()
        {
            GameRegistery.RegisterBlock<Stone>(new Stone());
        }
    }
}
