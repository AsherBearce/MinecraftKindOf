using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.MeshTypes
{
    /// <summary>
    /// Handles geometry data, and vertex buffers
    /// </summary>
    public class Mesh
    {
        public Vector3[] Verticies;
        public Vector3[] Normals;
        public int[] Triangles;
        public Vector2[] UV;
        public VertexPositionNormalTexture[] VertexBufferData;
        public Effect Shader;
        public VertexBuffer ObjBuffer;

        public void CalculateNormals()
        {
            Normals = new Vector3[Triangles.Length / 3];

            for (int i = 0; i < Triangles.Length / 3; i += 3)
            {
                Vector3 a = this.Verticies[this.Triangles[i]] - this.Verticies[this.Triangles[i + 1]];
                Vector3 b = this.Verticies[this.Triangles[i]] - this.Verticies[this.Triangles[i + 2]];

                Normals[i / 3] = Vector3.Normalize(Vector3.Cross(a, b));
            }
        }

        public void BuildObjectBuffer(GraphicsDevice graphicsDevice)
        {
            VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[Triangles.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new VertexPositionNormalTexture(this.Verticies[this.Triangles[i]], Normals[(int)System.Math.Floor((double)i / 3)], UV == null ? new Vector2(0, 0) : UV[i]);//The mesh doesn't HAVE to include UV coordinates
            }

            VertexBufferData = data;

            ObjBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), Triangles.Length, BufferUsage.WriteOnly);
            ObjBuffer.SetData<VertexPositionNormalTexture>(data);
        }

        public void UpdateBuffer(GraphicsDevice graphicsDevice)
        {
            if (ObjBuffer != null)
            {
                ObjBuffer.Dispose();//<- This must be called in order to avoid a memory leak
            }

            ObjBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), Triangles.Length, BufferUsage.WriteOnly);
            ObjBuffer.SetData<VertexPositionNormalTexture>(VertexBufferData);
        }
    }
}
