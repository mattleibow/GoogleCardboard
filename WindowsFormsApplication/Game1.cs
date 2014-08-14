using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace WindowsFormsApplication
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

	    public Camera camera;
	    private Floor floor;
	    private BasicEffect effect;

	    private Cube monkey;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
	        graphics.PreferMultiSampling = true;
			//graphics.PreparingDeviceSettings += (sender, e) =>
			//{
			//	PresentationParameters pp = e.GraphicsDeviceInformation.PresentationParameters;
			//	pp.MultiSampleCount = 16;

			//};
			graphics.SupportedOrientations = /*DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight | */DisplayOrientation.Portrait;// | DisplayOrientation.PortraitDown;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

			camera = new Camera(this, new Vector3(10f, 0.5f, 5f), Vector3.Zero, 5f);
			Components.Add(camera);
			floor = new Floor(GraphicsDevice, 20, 20);
			effect = new BasicEffect(GraphicsDevice);

			monkey = new Cube(GraphicsDevice);
			monkey.Position = camera.Position + new Vector3(0.0f, 0.0f, 10f);
			monkey.Rotation = new Vector3(0.0f, 0.0f, 0.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

	        floor.Texture = Content.Load<Texture2D>("floor");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

	        //monkey.Rotation += new Vector3(0.5f, 0.5f, 0.5f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

			floor.Draw(camera, effect);
	        monkey.Draw(camera, effect);

            base.Draw(gameTime);
        }
    }
}
