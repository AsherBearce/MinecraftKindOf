using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlaneGame2.Instances
{
    /// <summary> The Camera class for rendering the Scene </summary>
    public class Camera : GameObject
    {

        #region PUBLIC PROPERTIES
        /// <summary> The screen of this Camera </summary>
        public RenderTarget2D Screen { get; set; }

        /// <summary> The projection matrix of this Camera </summary>
        public Matrix ProjectionMatrix { get { return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane); } }

        /// <summary> The field of view of this Camera </summary>
        public float FieldOfView { get; set; }

        /// <summary> The aspect ratio of this Camera </summary>
        public float AspectRatio { get; set; }

        /// <summary> The near clipping plane of this Camera </summary>
        public float NearPlane { get; set; }

        /// <summary> The far clipping plane of this Camera </summary>
        public float FarPlane { get; set; }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a new Camera </summary>
        /// <param name="graphicsDevice"> The graphics device this Camera will render to </param>
        /// <param name="fieldOfView"> The field of view of this Camera in radians </param>
        /// <param name="near"> The near clipping plane of this Camera </param>
        /// <param name="far"> The far clipping plane of this Camera </param>
        /// <param name="aspectRatio"> The aspect ratio of this Camera </param>
        public Camera(GraphicsDevice graphicsDevice, float fieldOfView, float near, float far, float aspectRatio)
        {
            FieldOfView = fieldOfView;
            NearPlane = near;
            FarPlane = far;
            AspectRatio = aspectRatio;

            Screen = new RenderTarget2D(graphicsDevice,
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
        }
        #endregion
    }
}
