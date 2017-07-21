using PlaneGame2.MeshTypes;
using PlaneGame2.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using PlaneGame2.Misc;
using PlaneGame2.Blocks;

//Messy code. Needs a bit of clean up later on.
namespace PlaneGame2.Instances
{
    struct LoadQuery
    {
        public int x;
        public int y;
        public byte Command;//0 loads a chunk, 1 unloads
    }

    public class ChunkManager : Instance
    {
        internal Chunk[,] Chunks;
        internal LinkedList<LoadQuery> ChunkQuery = new LinkedList<LoadQuery>();
        public Texture2D Atlas;
        public Effect Shader;
        public int QueriesPerThread = 1;
        public bool ProcessingQueries { get; private set; }

        public Chunk this[int x, int y]
        {
            get => (x >= 0 && x < Width && y >= 0 && y < Height) ? Chunks[x, y] : null;
        }

        public int Width => Chunks.GetLength(0);

        public int Height => Chunks.GetLength(1);

        public void ProcessQueries()
        {
            if (ChunkQuery.First != null)
            {
                LinkedListNode<LoadQuery> NodeInList = ChunkQuery.First;
                LinkedListNode<LoadQuery> Next;

                for (int i = 0; i < QueriesPerThread; i++)
                {
                    if (NodeInList.Value.Command == 1)
                    {
                        Next = NodeInList.Next;
                        this.LoadChunk(NodeInList.Value.x, NodeInList.Value.y, Atlas);
                        this.DrawChunk(NodeInList.Value.x, NodeInList.Value.y);
                        ChunkQuery.Remove(NodeInList);
                        NodeInList = Next;
                    }
                    else
                    {
                        Next = NodeInList.Next;
                        this.UnloadChunk(NodeInList.Value.x, NodeInList.Value.y);
                        ChunkQuery.Remove(NodeInList);
                        NodeInList = Next;
                    }
                    if (NodeInList == null || NodeInList.Next == null)
                    {
                        break;
                    }
                }
            }
        }

        private int Mod(float x, float y)//May move this function into some other class
        {
            float sgnX = System.Math.Sign(x);
            return (int)sgnX * (int)System.Math.Floor((sgnX * x) / y);
        }
        public void AddChunk(Chunk chunk, int x, int y)
        {
            Chunks[x, y] = chunk;
        }

        public void QueryChunkLoad(int x, int y)
        {
            LoadQuery query = new LoadQuery();
            query.Command = 1;
            query.x = x;
            query.y = y;
            ChunkQuery.AddLast(query);
            Chunks[x, y] = new Chunk(null);//Set the parent null for now, until the processor can actually get to this.
        }

        public void QueryChunkDeload(int x, int y)
        {
            if (Chunks[x, y] != null)
            {
                LoadQuery query = new LoadQuery();
                query.Command = 0;
                query.x = x;
                query.y = y;
                ChunkQuery.AddLast(query);
                Chunks[x, y].DeloadScheduled = true;
            }

        }

        public void LoadChunk(int x, int y, Texture2D atlas)//Eventually will look for a file to load the chunk from.
        {
            Chunk newChunk = Chunks[x, y];
            newChunk.Parent = this.Parent;
            newChunk.Atlas = atlas;
            newChunk.Position = new Vector3(x * 16, 0, y * 16);
            newChunk.Container = this;
            newChunk.PopulateChunk();
            newChunk.Visible = false;
            //AddChunk(newChunk, x, y);
        }

        public void DrawChunk(int x, int y)
        {
            Chunks[x, y].MeshData.Shader = Shader;
            Chunks[x, y].UpdateFlag = true;
            Chunks[x, y].Visible = true;
        }

        public void UnloadChunk(int x, int y)//Eventually will write to a file.
        {
            if (Chunks[x, y] != null)
            {
                Chunks[x, y].Dispose();
                Chunks[x, y].Parent = null;
                Chunks[x, y] = null;
            }

        }

        public Vector2 GetChunkCoords(Vector3 position)
        {
            int cx = Mod(position.X, 16);
            int cy = Mod(position.Z, 16);
            return new Vector2(cx, cy);
        }

        public bool PositionInRange(Vector3 position)
        {
            Vector2 chunkPos = GetChunkCoords(position);
            return (chunkPos.X >= 0 && chunkPos.X < Width && chunkPos.Y >= 0 && chunkPos.Y < Height);
        }

        public bool PositionInRange(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
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

                if (North != targ && North != null)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    North.UpdateFlag = true;
                }
                if (South != targ && South != null)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    South.UpdateFlag = true;
                }
                if (West != targ && West != null)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    West.UpdateFlag = true;
                }
                if (East != targ && East != null)//Next, check each of the four directions(north south east west) to see if any other chunks need updating
                {
                    East.UpdateFlag = true;
                }
            }
            else
            {
                Console.WriteLine("Out of range");
            }
        }
        public byte GetChunkID(Vector3 position)//All return 0's should return the block ID output by the noise function.
        {
            int sgnX = System.Math.Sign(position.X);
            int sgnY = System.Math.Sign(position.Y);
            int sgnZ = System.Math.Sign(position.Z);
            int cx = Mod(position.X, 16);
            int cy = Mod(position.Z, 16);

            if (cx >= 0 && cx < Width && cy >= 0 && cy < Height)
            {
                Chunk targ = Chunks[cx, cy];
                if (targ != null)
                {
                    Vector3 Coords = targ.VoxelCoordinates(position);
                    Vector3 GlobalCoords = targ.ToGlobalCoordinates(new Vector3(Coords.X, -Coords.Y, Coords.Z));

                    if (targ.isInRange((int)Coords.X, -(int)Coords.Y, (int)Coords.Z))//checking to make sure that the Coordinates are actually in a chunk
                    {
                        return targ[(int)Coords.X, -(int)Coords.Y, (int)Coords.Z];
                    }
                    else
                    {
                        return TerrainGenerator.BlockGen(GlobalCoords.X, GlobalCoords.Y, GlobalCoords.Z);//If for whatever reason the coordinate is too high
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

        public override void Update(GameTime gameTime)
        {
            //ProcessQueries();
            base.Update(gameTime);
        }

        public ChunkManager(int width, int height) 
            : base("Terrain")
        {
            Chunks = new Chunk[width, height];
        }
    }
}
