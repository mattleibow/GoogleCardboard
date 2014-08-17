using Microsoft.Xna.Framework;

namespace VirtualReality
{
    public class VirtualRealityGame : Game
    {
	    public GraphicsDeviceManager Graphics { get; private set; }

	    public VirtualRealityGame()
		{
			Graphics = new GraphicsDeviceManager(this);
			Graphics.PreferMultiSampling = true;
			Graphics.SupportedOrientations =
				DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight |
				DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;

	    }

		public HeadTransform HeadTransform { get; private set; }

        protected override void Initialize()
        {
			HeadTransform = new HeadTransform(GraphicsDevice.Viewport);
	        HeadTransform.DisplayOrientation = Window.CurrentOrientation;
	        Window.OrientationChanged += (sender, args) => HeadTransform.DisplayOrientation = Window.CurrentOrientation;
			
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
			// for both eyes
	        HeadTransform head = HeadTransform;
	        GraphicsDevice.Viewport = head.Viewport;
			DrawFrame(gameTime);

			// left eye
	        EyeTransform left = head.LeftEyeTransform;
	        GraphicsDevice.Viewport = left.Viewport;
			DrawEye(gameTime, left);
			
			// right eye
	        EyeTransform right = head.RightEyeTransform;
	        GraphicsDevice.Viewport = right.Viewport;
			DrawEye(gameTime, right);

            base.Draw(gameTime);
        }

	    protected virtual void DrawFrame(GameTime gameTime)
	    {
	    }

	    protected virtual void DrawEye(GameTime gameTime, EyeTransform eyeTransform)
	    {
	    }
    }
}
