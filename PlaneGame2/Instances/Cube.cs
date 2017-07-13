using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlaneGame2.MeshTypes;

namespace PlaneGame2.Instances
{
    public class Cube : MeshObject
    {

        public Cube(string name, Instance parent, GraphicsDevice graphicsDevice) 
            : base(name, parent)
        {
            base.MeshData = new Mesh();
            base.MeshData.Verticies = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f)
            };

            base.MeshData.Triangles = new int[]
            {
                1, 0, 2,
                3, 1, 2,
                3, 2, 7,
                7, 2, 6,
                7, 1, 3,
                5, 1, 7,
                7, 6, 4,
                5, 7, 4,
                4, 0, 1,
                4, 1, 5,
                6, 2, 4,
                2, 0, 4
            };

            base.MeshData.CalculateNormals();

            base.MeshData.BuildObjectBuffer(graphicsDevice);
        }
    }
}
