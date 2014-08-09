using System;
using Android.Opengl;
using Math = Java.Lang.Math;

namespace Google.Cardboard
{
	using Matrix = Matrix;

	public class FieldOfView
	{
		private float mLeft;
		private float mRight;
		private float mBottom;
		private float mTop;

		public FieldOfView()
		{
		}

		public FieldOfView(float left, float right, float bottom, float top)
		{
			mLeft = left;
			mRight = right;
			mBottom = bottom;
			mTop = top;
		}

		public FieldOfView(FieldOfView other)
		{
			mLeft = other.mLeft;
			mRight = other.mRight;
			mBottom = other.mBottom;
			mTop = other.mTop;
		}

		public virtual float Left
		{
			set { mLeft = value; }
			get { return mLeft; }
		}

		public virtual float Right
		{
			set { mRight = value; }
			get { return mRight; }
		}

		public virtual float Bottom
		{
			set { mBottom = value; }
			get { return mBottom; }
		}

		public virtual float Top
		{
			set { mTop = value; }
			get { return mTop; }
		}

		public virtual void toPerspectiveMatrix(float near, float far, float[] perspective, int offset)
		{
			if (offset + 16 > perspective.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			float l = (float) -Math.Tan(Math.ToRadians(mLeft))*near;
			float r = (float) Math.Tan(Math.ToRadians(mRight))*near;
			float b = (float) -Math.Tan(Math.ToRadians(mBottom))*near;
			float t = (float) Math.Tan(Math.ToRadians(mTop))*near;
			Matrix.FrustumM(perspective, offset, l, r, b, t, near, far);
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
			if (!(other is FieldOfView))
			{
				return false;
			}
			FieldOfView o = (FieldOfView) other;
			return (mLeft == o.mLeft) && (mRight == o.mRight) && (mBottom == o.mBottom) && (mTop == o.mTop);
		}

		public override string ToString()
		{
			return "FieldOfView {left:" + mLeft + " right:" + mRight + " bottom:" + mBottom + " top:" + mTop +
			       "}";
		}
	}

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     Google.Cardboard.FieldOfView
	 * JD-Core Version:    0.7.0.1
	 */
}