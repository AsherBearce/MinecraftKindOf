using PlaneGame2.MeshTypes;
using PlaneGame2.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using PlaneGame2.Misc;
using PlaneGame2.Blocks;

/*
 * TODO:
 * Enhance texturing abilities
 * Create voxel raycasting
 * Add loading method for VoxelObjects <- May not be needed
 */
namespace PlaneGame2.Instances
{
    /// <summary> A MeshObject with voxel data </summary>
    public class VoxelObject : GameObject, IDrawableMesh
    {
        #region PRIVATE FIELDS
        /// <summary> The internal meshData </summary>
        private Mesh meshData;

        /// <summary> The internal voxel data, use VoxelData instead </summary>
        internal byte[,,] voxelData;
        #endregion

        #region PUBLIC PROPERTIES
        //Hopefully set does nothing?
        /// <summary> The MeshData for this VoxelObject </summary>
        public Mesh MeshData { get => meshData; set { Console.WriteLine("Tried to modify voxel's MeshData"); } }

        ///<summary> The texture atlas that the voxel object current uses </summary>
        public Texture2D Atlas;

        /// <summary> The data for this VoxelObject </summary>
        /// <param name="x"> The x coord of the voxel data </param>
        /// <param name="y"> The y coord of the voxel data </param>
        /// <param name="z"> The z coord of the voxel data </param>
        /// <returns> The byte at the given coords, or 0 if the coords are out of bounds </returns>
        public virtual byte this[int x, int y, int z]//Should have the ability to be overriden. May change when developing chunks
        {
            get => isInRange(x, y, z) ? voxelData[x, y, z] : (byte)0;
            set { if (isInRange(x, y, z)) voxelData[x, y, z] = value; }
        }

        /// <summary> The length of the X axis of this VoxelObject </summary>
        public byte Width => (byte)voxelData.GetLength(0);

        /// <summary> The length of the Y axis of this VoxelObject </summary>
        public byte Height => (byte)voxelData.GetLength(1);

        /// <summary> The length of the Z axis of this VoxelObject </summary>
        public byte Depth => (byte)voxelData.GetLength(2);
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary> Finds if the given coords are in range </summary>
        /// <param name="x"> The x coord </param>
        /// <param name="y"> The y coord </param>
        /// <param name="z"> The z coord </param>
        /// <returns> If the given coords are in range </returns>
        internal bool isInRange(int x, int y, int z) => x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth;
        
        private static int mod(float x, float y)
        {
            float sgnX = System.Math.Sign(x);
            return (int)sgnX * (int)System.Math.Floor((sgnX * x) / y);
        }
        #endregion

        #region PUBLIC FUNCTIONS
        public Vector3 VoxelCoordinates(Vector3 Position)//I think this works now
        {
            Vector3 ObjSpace = Vector3.Transform(Position, Matrix.Invert(GlobalTransform));
            int sgnX = System.Math.Sign(ObjSpace.X);
            int sgnY = System.Math.Sign(ObjSpace.Y);
            int sgnZ = System.Math.Sign(ObjSpace.Z);
            Vector3 returnVector = new Vector3(mod(ObjSpace.X + 0.5f * sgnX, 1), mod(ObjSpace.Y + 0.5f * sgnY, 1), mod(ObjSpace.Z + 0.5f * sgnZ, 1));
            return returnVector;
        }// I think I need to do an inverse function of this.

        public Vector3 ToGlobalCoordinates(Vector3 Position)
        {
            return Vector3.Transform(Position, GlobalTransform);
        }

        public override void Draw(GraphicsDevice graphicsDevice, Camera activeCamera)
        {
            if (Visible)
            {
                if (MeshData != null)
                {
                    //The matrices needed for drawing
                    Matrix World = GlobalTransform;
                    Matrix WorldInverse = Matrix.Invert(GlobalTransform);
                    Matrix View = Matrix.Invert(activeCamera.GlobalTransform);
                    Matrix Projection = activeCamera.ProjectionMatrix;

                    //Sets the matrices for the mesh
                    MeshData.Shader.Parameters["World"].SetValue(World);
                    MeshData.Shader.Parameters["View"].SetValue(View);
                    MeshData.Shader.Parameters["Projection"].SetValue(Projection);
                    MeshData.Shader.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(WorldInverse));
                    MeshData.Shader.Parameters["Atlas"].SetValue(Atlas);

                    //Loads the mesh into the graphics device
                    graphicsDevice.SetVertexBuffer(MeshData.ObjBuffer);

                    //Draws the mesh
                    foreach (EffectPass pass in MeshData.Shader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, MeshData.Triangles.Length);
                    }
                }
            }
            //Draws any children regardless of whether or not this MeshObject has a MeshData
            base.Draw(graphicsDevice, activeCamera);
        }

        /// <summary> Creates a mesh based on the voxel data </summary>
        public virtual void RecalculateMesh(GraphicsDevice device, Effect Shader)
        {
            meshData.Shader = Shader;

            LinkedList<Vector3> Verticies = new LinkedList<Vector3>();
            LinkedList<int> Tris = new LinkedList<int>();
            LinkedList<Vector3> Norms = new LinkedList<Vector3>();
            LinkedList<Vector2> Uv = new LinkedList<Vector2>();

            int texSizeX = Atlas.Width;
            int texSizeY = Atlas.Height;
            float px = 16 / (float)texSizeX;
            float py = 16 / (float)texSizeY;

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        byte blockID = this[x, y, z];

                        if (blockID != 0)
                        {
                            Block block = BlockManager.GetBlockFromID(blockID);

                            Vector2 offset = new Vector2(block.TextureOffsetX * px, block.TextureOffsetY * py);

                            Block upType = BlockManager.GetBlockFromID(this     [x, y + 1, z]);
                            Block downType = BlockManager.GetBlockFromID(this   [x, y - 1, z]);
                            Block rightType = BlockManager.GetBlockFromID(this  [x + 1, y, z]);
                            Block leftType = BlockManager.GetBlockFromID(this   [x - 1, y, z]);
                            Block frontType = BlockManager.GetBlockFromID(this  [x, y, z - 1]);
                            Block backType = BlockManager.GetBlockFromID(this   [x, y, z + 1]);

                            Vector3 voxelPosition = new Vector3(x, y, z);

                            if (upType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));

                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);
                                Uv.AddLast(new Vector2(px, 0) + offset);

                                Tris.AddLast(3 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Norms.AddLast(new Vector3(0, 1, 0));
                                Norms.AddLast(new Vector3(0, 1, 0));
                            }

                            if (downType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));

                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);
                                Uv.AddLast(new Vector2(px, 0) + offset);

                                Tris.AddLast(2 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(3 + Verticies.Count - 4);

                                Tris.AddLast(2 + Verticies.Count - 4);
                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);

                                Norms.AddLast(new Vector3(0, -1, 0));
                                Norms.AddLast(new Vector3(0, -1, 0));
                            }

                            if (frontType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));

                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);
                                Uv.AddLast(new Vector2(px, 0) + offset);

                                Tris.AddLast(3 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Norms.AddLast(new Vector3(0, 0, -1));
                                Norms.AddLast(new Vector3(0, 0, -1));
                            }

                            if (backType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));

                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(px, 0) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);

                                Tris.AddLast(3 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(0 + Verticies.Count - 4);
                                Tris.AddLast(3 + Verticies.Count - 4);

                                Norms.AddLast(new Vector3(0, 0, -1));
                                Norms.AddLast(new Vector3(0, 0, -1));
                            }

                            if (rightType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));

                                Uv.AddLast(new Vector2(px, 0) + offset);
                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);

                                Tris.AddLast(2 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(3 + Verticies.Count - 4);

                                Tris.AddLast(2 + Verticies.Count - 4);
                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);

                                Norms.AddLast(new Vector3(1, 0, 0));
                                Norms.AddLast(new Vector3(1, 0, 0));
                            }

                            if (leftType.Type != block.Type)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));

                                Uv.AddLast(new Vector2(px, 0) + offset);
                                Uv.AddLast(new Vector2(px, py) + offset);
                                Uv.AddLast(new Vector2(0, py) + offset);
                                Uv.AddLast(new Vector2(0, 0) + offset);

                                Tris.AddLast(3 + Verticies.Count - 4);
                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Tris.AddLast(Verticies.Count - 4);
                                Tris.AddLast(1 + Verticies.Count - 4);
                                Tris.AddLast(2 + Verticies.Count - 4);

                                Norms.AddLast(new Vector3(-1, 0, 0));
                                Norms.AddLast(new Vector3(-1, 0, 0));
                            }
                        }
                    }
                }
            }

            Vector3[] verts = new Vector3[Verticies.Count];
            Vector3[] norms = new Vector3[Norms.Count];
            int[] tris = new int[Tris.Count];
            Vector2[] uv = new Vector2[Uv.Count];
            VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[Tris.Count];

            LinkedListNode<Vector3> nodeInVerticies = Verticies.First;
            LinkedListNode<Vector2> nodeInUvs = Uv.First;

            for (int v = 0; v < verts.Length; v++)
            {
                verts[v] = nodeInVerticies.Value;
                uv[v] = nodeInUvs.Value;
                if (nodeInVerticies != null)
                {
                    nodeInVerticies = nodeInVerticies.Next;
                    nodeInUvs = nodeInUvs.Next;
                }
            }

            LinkedListNode<Vector3> nodeInNormals = Norms.First;

            for (int n = 0; n < norms.Length; n++)
            {
                norms[n] = nodeInNormals.Value;
                if (nodeInNormals != null)
                {
                    nodeInNormals = nodeInNormals.Next;
                }
            }

            LinkedListNode<int> nodeInTris = Tris.First;

            for (int t = 0; t < tris.Length; t++)
            {
                tris[t] = nodeInTris.Value;
                data[t] = new VertexPositionNormalTexture(verts[tris[t]], norms[(int)System.Math.Floor((double)t / 3)], uv[tris[t]]);
                if (nodeInTris != null)
                {
                    nodeInTris = nodeInTris.Next;
                }
            }

            meshData.Verticies = verts;
            meshData.Triangles = tris;
            meshData.Normals = norms;
            meshData.UV = uv;
            meshData.VertexBufferData = data;
            meshData.UpdateBuffer(device);
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a VoxelObject with the given name and parent </summary>
        /// <param name="name"> The name of this VoxelObject </param>
        /// <param name="parent"> The parent of this VoxelObject </param>
        public VoxelObject(string name, Instance parent)
            : base(name, parent)
        {
            meshData = new Mesh();
            //Create an empty mesh since we have no voxel data
        }

        /// <summary> Creates a VoxelObject with the given name, parent, and voxel data </summary>
        /// <param name="name"> The name of this VoxelObject </param>
        /// <param name="parent"> The parent of this VoxelObject </param>
        /// <param name="voxels"> The voxel data of this VoxelObject </param>
        public VoxelObject(string name, Instance parent, byte[,,] voxels) : base(name, parent)
        {
            voxelData = voxels;//Shouldn't necessarily create the mesh upon construction. Just set the voxel data.
            meshData = new Mesh();
        }

        /// <summary>
        /// Creates a VoxelObject with empty of voxel data of a given dimension.
        /// </summary>
        /// <param name="name"> The name of this VoxelObject </param>
        /// <param name="parent">The parent of this VoxelObject </param>
        /// <param name="width">The voxel width of this VoxelObject </param>
        /// <param name="depth">The voxel depth of this VoxelObject </param>
        /// <param name="height">The voxel height of this Voxel Object </param>
        public VoxelObject(string name, Instance parent, int width, int depth, int height) : base(name, parent)
        {
            voxelData = new byte[width, depth, height];
            meshData = new Mesh();
        }
        #endregion
    }
}
