using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Instances
{
    /// <summary> Acts as a root node for the hierarchy </summary>
    public class Scene : Instance
    {
        #region PUBLIC FUNCTIONS
        /// <summary> Draws the scene with the given camera </summary>
        /// <param name="graphicsDevice"> The graphics device to draw to </param>
        /// <param name="activeCamera"> The camera that is drawing the scene </param>
        public override void Draw(GraphicsDevice graphicsDevice, Camera activeCamera)
        {
            //Set the render target to the given camera
            graphicsDevice.SetRenderTarget(activeCamera.Screen);

            //Activates the depth buffer
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            //Clears the render target
            graphicsDevice.Clear(Color.CornflowerBlue);

            //Draws the scene's children
            base.Draw(graphicsDevice, activeCamera);

            //Sets the render target back to the screen
            graphicsDevice.SetRenderTarget(null);
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a blank Scene </summary>
        public Scene()
            : base ("Scene")
        {

        }

        /// <summary> Creates a Scene with a given name </summary>
        /// <param name="name"> The name of this Scene </param>
        public Scene(string name)
            : base(name)
        {

        }
        #endregion
    }
}
