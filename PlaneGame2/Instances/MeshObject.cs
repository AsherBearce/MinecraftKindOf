using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlaneGame2.Interfaces;
using PlaneGame2.MeshTypes;

namespace PlaneGame2.Instances
{
    /// <summary> An Instance with positional and mesh data </summary>
    public class MeshObject : GameObject, IDrawableMesh
    {
        #region PUBLIC PROPERTIES
        /// <summary> The model of this MeshObject </summary>
        public virtual Mesh MeshData { get; set; }
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary> Draws this MeshObject's mesh </summary>
        /// <param name="graphicsDevice"> The graphics device to draw to </param>
        /// <param name="activeCamera"> The camera which is drawing this Instance </param>
        public override void Draw(GraphicsDevice graphicsDevice, Camera activeCamera)
        {
            if (Visible)
            {
                if (MeshData != null)
                {
                    //The matrices needed for drawing
                    Matrix WorldInverse = Matrix.Invert(GlobalTransform);
                    Matrix View = activeCamera.GlobalTransform;
                    Matrix Projection = activeCamera.ProjectionMatrix;

                    //Sets the matrices for the mesh
                    MeshData.Shader.Parameters["World"].SetValue(WorldInverse);
                    MeshData.Shader.Parameters["View"].SetValue(View);
                    MeshData.Shader.Parameters["Projection"].SetValue(Projection);
                    MeshData.Shader.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(WorldInverse));

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
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a new MeshObject with the given name and parent </summary>
        /// <param name="name"> The name of this MeshObject </param>
        /// <param name="parent"> The parent of this MeshObject </param>
        public MeshObject(string name, Instance parent)
            : base(name, parent)
        {

        }

        /// <summary> Creates a new MeshObject with the given mesh, rotation, position, and scale </summary>
        /// <param name="mesh"> The mesh of this MeshObject </param>
        /// <param name="rotation"> The rotation of this MeshObject </param>
        /// <param name="position"> The position of this MeshObject </param>
        /// <param name="scale"> The scale of this MeshObject </param>
        public MeshObject(Mesh mesh, Matrix rotation, Vector3 position, Vector3 scale)
            : base(rotation, position, scale)
        {
            MeshData = mesh;
        }

        /// <summary> Creates a new MeshObject with the given mesh, rotation, position, scale, and parent </summary>
        /// <param name="mesh"> The mesh of this MeshObject </param>
        /// <param name="rotation"> The rotation of this MeshObject </param>
        /// <param name="position"> The position of this MeshObject </param>
        /// <param name="scale"> The scale of this MeshObject </param>
        /// <param name="parent"> The parent of this MeshObject </param>
        public MeshObject(Mesh mesh, Matrix rotation, Vector3 position, Vector3 scale, Instance parent)
            : base(rotation, position, scale, parent)
        {
            MeshData = mesh;
        }

        /// <summary> Creates a new MeshObject with the given mesh, rotation, position, scale, name, and parent </summary>
        /// <param name="mesh"> The mesh of this MeshObject </param>
        /// <param name="rotation"> The rotation of this MeshObject </param>
        /// <param name="position"> The position of this MeshObject </param>
        /// <param name="scale"> The scale of this MeshObject </param>
        /// <param name="name"> The name of this MeshObject </param>
        /// <param name="parent"> The parent of this MeshObject </param>
        public MeshObject(Mesh mesh, Matrix rotation, Vector3 position, Vector3 scale, string name, Instance parent)
            : base(rotation, position, scale, name, parent)
        {
            MeshData = mesh;
        }
        #endregion
    }
}
