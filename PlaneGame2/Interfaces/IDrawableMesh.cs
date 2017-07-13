using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlaneGame2.MeshTypes;
using PlaneGame2.Instances;

namespace PlaneGame2.Interfaces
{
    interface IDrawableMesh
    {
        Mesh MeshData { get; set; }

        void Draw(GraphicsDevice device, Camera activeCamera);
    }
}
