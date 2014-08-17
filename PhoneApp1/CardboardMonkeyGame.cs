using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VirtualReality;

namespace PhoneApp1
{
    public class CardboardMonkeyGame : VirtualRealityGame
    {
	    private SpriteBatch spriteBatch;
		private BasicEffect effect;

	    private Floor floor;
	    private Cube monkey;

		public CardboardMonkeyGame()
        {
            Content.RootDirectory = "Content";

			Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
        }

        protected override void Initialize()
        {
			effect = new BasicEffect(GraphicsDevice);

			floor = new Floor(GraphicsDevice, 20, 20);
			monkey = new Cube(GraphicsDevice);

            base.Initialize();

			// now that the view is finished, move the objects to their positions
			HeadTransform.Position = new Vector3(10f, 0.5f, 5f);
			monkey.Position = HeadTransform.Position + new Vector3(0.0f, 0.0f, 10f);
			monkey.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

	        floor.Texture = Content.Load<Texture2D>("floor");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
			monkey.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * new Vector3(0.5f, 0.5f, 1.0f);

            base.Update(gameTime);
        }

	    protected override void DrawFrame(GameTime gameTime)
	    {
			GraphicsDevice.Clear(Color.CornflowerBlue);
	    }

	    protected override void DrawEye(GameTime gameTime, EyeTransform eyeTransform)
	    {
			floor.Draw(eyeTransform, effect);

			monkey.Draw(eyeTransform, effect);
	    }
    }
}
