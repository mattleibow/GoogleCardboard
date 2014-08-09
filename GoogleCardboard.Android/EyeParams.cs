namespace Google.Cardboard
{
	public class EyeParams
	{
		private readonly int mEye;
		private readonly Viewport mViewport;
		private readonly FieldOfView mFov;
		private readonly EyeTransform mEyeTransform;

		public EyeParams(int eye)
		{
			mEye = eye;
			mViewport = new Viewport();
			mFov = new FieldOfView();
			mEyeTransform = new EyeTransform(this);
		}

		public virtual int Eye
		{
			get { return mEye; }
		}

		public virtual Viewport Viewport
		{
			get { return mViewport; }
		}

		public virtual FieldOfView Fov
		{
			get { return mFov; }
		}

		public virtual EyeTransform Transform
		{
			get { return mEyeTransform; }
		}
	}

	public class Eye
	{
		public const int MONOCULAR = 0;
		public const int LEFT = 1;
		public const int RIGHT = 2;
	}
}