using System;
using System.Collections.Generic;
using PlaneGame2.Interfaces;
using PlaneGame2.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Blocks
{
    public class Air : IBlock
    {
        public BlockType type { get { return BlockType.Air; } }
        public byte ID { get { return 0; } }
        public string BlockName { get { return "Air"; } }
        public Vector2 TextureAddress { get { return new Vector2(0, 0); } }

        public static void init()
        {
            GameRegistery.RegisterBlock<Air>(new Air());
        }
    }
}