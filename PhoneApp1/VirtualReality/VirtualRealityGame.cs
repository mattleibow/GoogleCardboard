using Microsoft.Xna.Framework;

namespace VirtualReality
{
    public class VirtualRealityGame : Game
    {
        protected override void Initialize()
        {
			HeadTransform = new HeadTransform(GraphicsDevice.Viewport);
			
            base.Initialize();
        }

		public HeadTransform HeadTransform { get; private set; }

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
