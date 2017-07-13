using PlaneGame2.MeshTypes;
using PlaneGame2.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using PlaneGame2.Misc;
using PlaneGame2.Blocks;

namespace PlaneGame2.Instances
{
    public class ChunkManager : Instance
    {
        internal Chunk[,] Chunks;

        public int Width => Chunks.GetLength(0);

        public int Height => Chunks.GetLength(1);

        private int Mod(float x, float y)//May move this function into some other class
        {
            float sgnX = System.Math.Sign(x);
            return (int)sgnX * (int)System.Math.Floor((sgnX * x) / y);
        }
        public void AddChunk(Chunk chunk, int x, int y)
        {
            Chunks[x, y] = chunk;
        }
        public void LoadChunk(int x, int y, Texture2D atlas)
        {
            Chunk newChunk = new Chunk(this);
            newChunk.Atlas = atlas;
            newChunk.Position = new Vector3(15.5f + x * 16, 0, 15.5f + y * 16);
            newChunk.Container = this;
            newChunk.PopulateChunk();
            newChunk.Visible = false;
            AddChunk(newChunk, x, y);
        }

        public void DrawChunk(int x, int y, GraphicsDevice device, Effect Shader)
        {
            Chunks[x, y].RecalculateMesh(device, Shader);
            Chunks[x, y].Visible = true;
        }
        public void UnloadChunk(int x, int y)
        {
            Chunks[x, y].Parent = null;
            Chunks[x, y] = null;
        }

        public Chunk GetChunkFromPos(Vector3 position)
        {
            int cx = Mod(position.X, 16);
            int cy = Mod(position.Z, 16);
            if (cx >= 0 && cx < Width && cy >= 0 && cy < Height)
            {
                return Chunks[cx, cy];
            }
            else
            {
                return null;
            }
        }

        public void BuildWorld(GraphicsDevice device, Effect Shader)//This function is temporary. Do not load the entire world in one go. Should show only the chunks that are needed.
        {
            for (int x = 0; x < Width; x++)//About 64 chunks being loaded here. Quite good
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Chunks[x, y] != null)
                    {
                        Chunks[x, y].Visible = true;
                        Chunks[x, y].RecalculateMesh(device, Shader);
                    }
                    
                }
            }
        }
        public void SetBlockID(Vector3 position, byte ID)
        {
            Chunk targ = GetChunkFromPos(position);
            if (targ != null)
            {
                Vector3 coords = targ.VoxelCoordinates(position);
                targ[(int)coords.X, (int)coords.Y, (int)coords.Z] = ID;//Go ahead and set the blockID
                targ.UpdateFlag = true;
                Chunk North = GetChunkFromPos(position + new Vector3(0, 0, 1));
                Chunk South = GetChunkFromPos(position + new Vector3(0, 0, -1));
                Chunk East = GetChunkFromPos(position + new Vector3(1, 0, 0));
                Chunk West = GetChunkFromPos(position + new Vector3(-1, 0, 0));

                if (North != targ)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    North.UpdateFlag = true;
                }
                if (South != targ)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    South.UpdateFlag = true;
                }
                if (West != targ)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    West.UpdateFlag = true;
                }
                if (East != targ)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    East.UpdateFlag = true;
                }
            }
            else
            {
                //Do something
            }
        }
        public byte GetChunkID(Vector3 position)//All return 0's should return the block ID output by the noise function.
        {
            int sgnX = System.Math.Sign(position.X);
            int sgnZ = System.Math.Sign(position.Z);
            int cx = Mod(position.X, 16);
            int cy = Mod(position.Z, 16);

            if (cx >= 0 && cx < Width && cy >= 0 && cy < Height)
            {
                Chunk targ = Chunks[cx, cy];
                if (targ != null)
                {
                    Vector3 Coords = targ.VoxelCoordinates(position);

                    if (targ.isInRange((int)Coords.X, (int)Coords.Y, (int)Coords.Z))//checking to make sure that the Coordinates are actually in a chunk
                    {
                        return targ[(int)Coords.X, (int)Coords.Y, (int)Coords.Z];
                    }
                    else
                    {
                        return 0;//If for whatever reason the coordinate is too high
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public ChunkManager(int width, int height) 
            : base("Terrain")
        {
            Chunks = new Chunk[width, height];
        }
    }
}
