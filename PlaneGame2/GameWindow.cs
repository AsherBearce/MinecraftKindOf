using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlaneGame2.Instances;
using PlaneGame2.Misc;

//TODO: The block system needs some revision.
//      Some revision is needed to allow multiple vertex declarations.

namespace PlaneGame2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWindow : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scene mainScene;
        Camera mainCamera;
        Cube coob;
        //Chunk chunkTest;
        ChunkManager Terrain;
        Effect shader;
        Effect voxelShader;
        MouseState OriginalMouseState;
        Texture2D texture;
        Texture2D atlas;

        float camPitch = 0;
        float camYaw = 0;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1024;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Initalize blocks
            GameRegistery.init();
            //Create a new scene
            mainScene = new Scene();

            //Create a new camera to render the scene
            mainCamera = new Camera(GraphicsDevice, MathHelper.ToRadians(70f), 0.1f, 1000, (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight);
            mainCamera.Parent = mainScene;
            coob = new Cube("Cube", mainScene, GraphicsDevice);
            Terrain = new ChunkManager(20, 20);
            Terrain.Parent = mainScene;

            mainCamera.Position = new Vector3(0, 0, 0);
            coob.Position = new Vector3(16 + 0.5f, -2, 0.5f);

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            OriginalMouseState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shader = Content.Load<Effect>("BasicShader");
            voxelShader = Content.Load<Effect>("VoxelShader");
            texture = Content.Load<Texture2D>("Stone");
            atlas = Content.Load<Texture2D>("TexAtlas");

            coob.MeshData.Shader = shader;

            for (int x = 0; x < 10; x++)//Around 100 chunks being loaded, here. Not too bad.
            {
                for (int y = 0; y < 10; y++)
                {
                    Terrain.LoadChunk(x, y, atlas);
                }
            }

            Terrain.BuildWorld(GraphicsDevice, voxelShader);
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (IsActive)
            {
                MouseState CurrentMouseState = Mouse.GetState();

                if (CurrentMouseState != OriginalMouseState)
                {
                    float deltaX = CurrentMouseState.X - OriginalMouseState.X;
                    float deltaY = CurrentMouseState.Y - OriginalMouseState.Y;

                    float tanFov = 2 * (float)System.Math.Tan(mainCamera.FieldOfView * 0.00872664625d);
                    float deltaPitch = (float)System.Math.Atan(tanFov * deltaY / GraphicsDevice.Viewport.Height / 2);
                    float deltaYaw = (float)System.Math.Atan(tanFov * deltaX / GraphicsDevice.Viewport.Width / 2);
                    camPitch += 15 * deltaPitch;
                    camYaw += 15 * deltaYaw;
                    mainCamera.Rotation = Matrix.CreateRotationY(camYaw) * Matrix.CreateRotationX(camPitch);

                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.F))
                {
                    
                    for (int x = -1; x < 2; x++)//There should be a function for all this
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            for (int z = -1; z < 2; z++)
                            {
                                Terrain.SetBlockID(mainCamera.Position + new Vector3(x, y, z), (byte)0);
                            }
                        }
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            for (int z = -1; z < 2; z++)
                            {
                                Terrain.SetBlockID(mainCamera.Position + new Vector3(x, y, z), (byte)3);
                            }
                        }
                    }

                }


                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    mainCamera.Position -= 0.1f * mainCamera.UpVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    mainCamera.Position += 0.1f * mainCamera.UpVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    mainCamera.Position += 0.1f * mainCamera.RightVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    mainCamera.Position -= 0.1f * mainCamera.RightVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    mainCamera.Position += 0.1f * mainCamera.ForwardVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    mainCamera.Position -= 0.1f * mainCamera.ForwardVector;
                }


            }

            //Updates the scene
            mainScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Draws the scene with the camera
            mainScene.Draw(GraphicsDevice, mainCamera);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise);

            spriteBatch.Draw(mainCamera.Screen, new Rectangle(0, 0, 1024, 720), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
