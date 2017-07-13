using System;
using System.Collections.Generic;
using PlaneGame2.Interfaces;
using PlaneGame2.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Blocks
{
    public class Dirt : IBlock
    {
        public BlockType type { get { return BlockType.Solid; } }
        public byte ID { get { return 2; } }
        public string BlockName { get { return "Dirt"; } }
        public Vector2 TextureAddress { get { return new Vector2(2, 0); } }

        public static void init()
        {
            GameRegistery.RegisterBlock<Dirt>(new Dirt());
        }
    }
}
