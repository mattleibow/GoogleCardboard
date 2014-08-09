using Android.Opengl;

namespace Google.Cardboard
{
	using Matrix = Matrix;

	public class EyeTransform

	{
		private readonly EyeParams mEyeParams;
		private readonly float[] mEyeView;
		private readonly float[] mPerspective;

		public EyeTransform(EyeParams @params)

		{
			mEyeParams = @params;
			mEyeView = new float[16];
			mPerspective = new float[16];

			Matrix.SetIdentityM(mEyeView, 0);
			Matrix.SetIdentityM(mPerspective, 0);
		}

		public virtual float[] EyeView
		{
			get { return mEyeView; }
		}

		public virtual float[] Perspective
		{
			get { return mPerspective; }
		}

		public virtual EyeParams Params
		{
			get { return mEyeParams; }
		}
	}

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     Google.Cardboard.EyeTransform
	 * JD-Core Version:    0.7.0.1
	 */
}