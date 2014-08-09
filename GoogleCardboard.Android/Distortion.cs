using System;

namespace Google.Cardboard
{
	public class Distortion
	{
		private static readonly float[] DEFAULT_COEFFICIENTS = new float[]
		{
			250.0F,
			50000.0F
		};

		private float[] mCoefficients;

		public Distortion()
		{
			mCoefficients = new float[2];
			mCoefficients[0] = DEFAULT_COEFFICIENTS[0];
			mCoefficients[1] = DEFAULT_COEFFICIENTS[1];
		}

		public Distortion(Distortion other)
		{
			mCoefficients = new float[2];
			mCoefficients[0] = other.mCoefficients[0];
			mCoefficients[1] = other.mCoefficients[1];
		}

		public virtual float[] Coefficients
		{
			set
			{
				mCoefficients[0] = value[0];
				mCoefficients[1] = value[1];
			}
			get { return mCoefficients; }
		}

		public virtual float distortionFactor(float radius)
		{
			float rSq = radius*radius;
			return 1.0F + mCoefficients[0]*rSq + mCoefficients[1]*rSq*rSq;
		}

		public virtual float distort(float radius)
		{
			return radius*distortionFactor(radius);
		}

		public virtual float distortInverse(float radius)
		{
			float r0 = radius/0.9F;
			float r1 = radius*0.9F;

			float dr0 = radius - distort(r0);
			while (Math.Abs(r1 - r0) > 0.0001D)
			{
				float dr1 = radius - distort(r1);
				float r2 = r1 - dr1*((r1 - r0)/(dr1 - dr0));
				r0 = r1;
				r1 = r2;
				dr0 = dr1;
			}
			return r1;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!(other is Distortion))
			{
				return false;
			}
			Distortion o = (Distortion) other;
			return (mCoefficients[0] == o.mCoefficients[0]) && (mCoefficients[1] == o.mCoefficients[1]);
		}

		public override string ToString()
		{
			return "Distortion {" + mCoefficients[0] + ", " + mCoefficients[1] + "}";
		}
	}

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     Google.Cardboard.Distortion
	 * JD-Core Version:    0.7.0.1
	 */
}