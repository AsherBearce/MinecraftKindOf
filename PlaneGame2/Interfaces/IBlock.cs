using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Interfaces
{
    public enum BlockType {Air, Solid, Glass};
    /// <summary>
    /// All Blocks must inherit from this interface.
    /// </summary>
    public interface IBlock
    {
        BlockType type { get; }
        /// <summary> The block ID that will be in the game registery </summary>
        byte ID { get; }
        /// <summary> The name of the block </summary>
        string BlockName { get; }
        /// <summary> An ID that points to a texture in the texture atlas </summary>
        Vector2 TextureAddress{ get; }
    }
}
