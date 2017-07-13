using System;
using System.Collections.Generic;
using PlaneGame2.Interfaces;
using PlaneGame2.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Blocks
{
    public class Glass : IBlock
    {
        public BlockType type { get { return BlockType.Glass; } }
        public byte ID { get { return 3; } }
        public string BlockName { get { return "Glass"; } }
        public Vector2 TextureAddress { get { return new Vector2(2, 1); } }

        public static void init()
        {
            GameRegistery.RegisterBlock<Glass>(new Glass());
        }
    }
}