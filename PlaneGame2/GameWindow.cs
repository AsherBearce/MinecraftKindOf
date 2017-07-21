using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlaneGame2.Instances;
using PlaneGame2.Misc;

//TODO: Fix the chunk cloning glitch.
//The truth is that I've done a pretty shitty job here. 
//None of this works like it should. 
//There are too many issues to count. 
//Is it worth fixing? Or should I just redo it?

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
        ChunkManager Terrain;
        Effect shader;
        Effect voxelShader;
        MouseState OriginalMouseState;
        Texture2D texture;
        Texture2D atlas;
        Chunk CurrentChunk;
        Vector2 ChunkCoords;
        Vector2 lastChunkCoords;
        LinkedList<Vector2> LoadedChunks;

        SpriteFont FPSFont;

        float camPitch = 0;
        float camYaw = 0;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1900;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Create a new scene
            mainScene = new Scene();

            //Create a new camera to render the scene
            mainCamera = new Camera(GraphicsDevice, MathHelper.ToRadians(70f), 0.1f, 1000, (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight);
            mainCamera.Parent = mainScene;
            coob = new Cube("Cube", mainScene, GraphicsDevice);
            Terrain = new ChunkManager(256, 256);
            Terrain.Parent = mainScene;

            mainCamera.Position = new Vector3(800, -50, 800);
            coob.Position = new Vector3(800, -50, 800);

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            OriginalMouseState = Mouse.GetState();
            CurrentChunk = Terrain.GetChunkFromPos(mainCamera.Position);
            ChunkCoords = Terrain.GetChunkCoords(mainCamera.Position);
            LoadedChunks = new LinkedList<Vector2>();

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
            Terrain.Shader = voxelShader;
            Terrain.Atlas = atlas;

            FPSFont = Content.Load<SpriteFont>("FPS");

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
                    float deltaPitch = -(float)System.Math.Atan(tanFov * deltaY / GraphicsDevice.Viewport.Height / 2);
                    float deltaYaw = -(float)System.Math.Atan(tanFov * deltaX / GraphicsDevice.Viewport.Width / 2);
                    camPitch += 15 * deltaPitch;
                    camYaw += 15 * deltaYaw;
                    mainCamera.Rotation = Matrix.CreateRotationX(camPitch) * Matrix.CreateRotationY(camYaw);

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
                    mainCamera.Position += 0.5f * mainCamera.UpVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    mainCamera.Position -= 0.5f * mainCamera.UpVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    mainCamera.Position -= 0.5f * mainCamera.RightVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    mainCamera.Position += 0.5f * mainCamera.RightVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    mainCamera.Position += 0.5f * mainCamera.ForwardVector;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    mainCamera.Position -= 0.5f * mainCamera.ForwardVector;
                }


            }

            ChunkCoords = Terrain.GetChunkCoords(mainCamera.Position);//It may be possible to use a linked list of loaded chunks, then traverse the list and unload chunks that are not needed

            if (ChunkCoords != lastChunkCoords)
            {
                for (int x = -10; x <= 10; x++)
                {
                    for (int y = -10; y <= 10; y++)
                    {
                        CurrentChunk = Terrain[(int)ChunkCoords.X + x, (int)ChunkCoords.Y + y];
                        if (CurrentChunk == null && Terrain.PositionInRange((int)ChunkCoords.X + x, (int)ChunkCoords.Y + y))
                        {
                            Terrain.QueryChunkLoad((int)ChunkCoords.X + x, (int)ChunkCoords.Y + y);//I bet this is getting called multiple times. 
                        }
                    }
                }

                LinkedListNode<Instance> Child = Terrain.Children.First;

                if (Child != null)
                {
                    while (Child.Next != null)
                    {
                        Console.WriteLine("There are childs");
                        if (Child.Value is Chunk)
                        {
                            Chunk ValAsChunk = (Chunk)Child.Value;
                            Vector3 ChunkPos = ValAsChunk.Position;
                            Vector2 Coords = new Vector2((ChunkPos.X) / 16f, (ChunkPos.Z) / 16f);
                            if (Coords.X - ChunkCoords.X < -10 || Coords.X - ChunkCoords.X > 10 || Coords.Y - ChunkCoords.Y < -10 || Coords.Y - ChunkCoords.Y > 10)
                            {
                                if (!ValAsChunk.DeloadScheduled)
                                {
                                    Terrain.QueryChunkDeload((int)Coords.X, (int)Coords.Y);
                                    Terrain[(int)Coords.X, (int)Coords.Y].DeloadScheduled = true;
                                }
                            }
                        }
                        if (Child.Next == null)
                        {
                            break;
                        }
                        Child = Child.Next;
                    }
                }
            }
            lastChunkCoords = ChunkCoords;
            Terrain.ProcessQueries();
            
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

            spriteBatch.Draw(mainCamera.Screen, new Rectangle(0, 0, 1900 , 1000), Color.White);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            spriteBatch.DrawString(FPSFont, ((int)(1 / gameTime.ElapsedGameTime.TotalSeconds)).ToString(), new Vector2(10, 10), Color.White);
            GraphicsDevice.BlendState = BlendState.Opaque;
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
