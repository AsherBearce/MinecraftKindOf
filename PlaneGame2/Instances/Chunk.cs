using PlaneGame2.MeshTypes;
using PlaneGame2.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using PlaneGame2.Misc;
using PlaneGame2.Blocks;

/*
 * Most issues are fixed: The only problem now is updating other chunks that don't necessarily have their voxel data changed, but should be updated because voxel data near them was changed.
 * This should be an easy fix. Whenever changing a block (must be done from the ChunkManager) simply check around the block to make sure it's neighbors aren't in any other chunks. 
 */
namespace PlaneGame2.Instances
{
    public class Chunk : VoxelObject
    {
        ///<summary> The chunk manager of this chunk </summary>
        public ChunkManager Container;
        /// <summary> Tells the engine to recalculate the mesh, if necessary </summary>
        public bool UpdateFlag = false;
        public byte BlockGen(int x, int y, int z)//This method needs to be moved to it's own class
        {
            float noise = 10 * Noise.GetOctaveNoise(((double)x - Position.X) / 20f, ((double)y - Position.Y) / 20f, ((double)z - Position.Z) / 20f, 1) - 32 + y;//Temporary generator

            return (byte)((noise < 6 && noise > 3.5) ? 2 : (noise < 3.5) ? 1 : 0);
        }

        /// <summary> The data for this chunk </summary>
        /// <param name="x"> The x coord of the voxel data </param>
        /// <param name="y"> The x coord of the voxel data </param>
        /// <param name="z"> The x coord of the voxel data </param>
        /// <returns> The byte at the given coords. Also able to extend to  </returns>
        public override byte this[int x, int y, int z]
        {
            get => isInRange(x, y, z) ? voxelData[x, y, z] : Container.GetChunkID(ToGlobalCoordinates(new Vector3(-x, -y, -z)));//Ask the current chunk manager for the data, if it's out of range.
            set { if (isInRange(x, y, z)) voxelData[x, y, z] = value; }
        }

        ///<summary> Sets the chunk data produced from the terrain generator</summary>
        public void PopulateChunk()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        this[x, y, z] = BlockGen(x, y, z);
                    }
                }
            }
        }

        public override void Draw(GraphicsDevice graphicsDevice, Camera activeCamera)
        {
            if (UpdateFlag)
            {
                RecalculateMesh(graphicsDevice, MeshData.Shader);
                UpdateFlag = false;
            }
            base.Draw(graphicsDevice, activeCamera);
        }
        public Chunk(Instance parent) 
            : base("Chunk", parent, 16, 64, 16)
        {

        }
    }
}
