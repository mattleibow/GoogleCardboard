using System;
using Android.Util;
using Android.Opengl;

namespace Google.Cardboard
{
	public class HeadTransform
	{
		private const float GIMBAL_LOCK_EPSILON = 0.01F;
		private readonly float[] headView;

		public HeadTransform()
		{
			headView = new float[16];
			Matrix.SetIdentityM(headView, 0);
		}

		internal virtual float[] HeadView
		{
			get { return headView; }
		}

		public virtual void GetHeadView(float[] headView, int offset)
		{
			if (offset + 16 > headView.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			Array.Copy(this.headView, 0, headView, offset, 16);
		}

		public virtual void GetTranslation(float[] translation, int offset)
		{
			if (offset + 3 > translation.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			for (int i = 0; i < 3; i++)
			{
				translation[(i + offset)] = headView[(12 + i)];
			}
		}

		public virtual void GetForwardVector(float[] forward, int offset)
		{
			if (offset + 3 > forward.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			for (int i = 0; i < 3; i++)
			{
				forward[(i + offset)] = (-headView[(8 + i)]);
			}
		}

		public virtual void GetUpVector(float[] up, int offset)
		{
			if (offset + 3 > up.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			for (int i = 0; i < 3; i++)
			{
				up[(i + offset)] = headView[(4 + i)];
			}
		}

		public virtual void GetRightVector(float[] right, int offset)
		{
			if (offset + 3 > right.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			for (int i = 0; i < 3; i++)
			{
				right[(i + offset)] = headView[i];
			}
		}

		public virtual void GetQuaternion(float[] quaternion, int offset)
		{
			if (offset + 4 > quaternion.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			float[] m = headView;
			float t = m[0] + m[5] + m[10];
			float z;
			float x;
			float y;
			float w;
			if (t >= 0.0F)
			{
				float s = FloatMath.Sqrt(t + 1.0F);
				w = 0.5F*s;
				s = 0.5F/s;
				x = (m[9] - m[6])*s;
				y = (m[2] - m[8])*s;
				z = (m[4] - m[1])*s;
			}
			else
			{
				if ((m[0] > m[5]) && (m[0] > m[10]))
				{
					float s = FloatMath.Sqrt(1.0F + m[0] - m[5] - m[10]);
					x = s*0.5F;
					s = 0.5F/s;
					y = (m[4] + m[1])*s;
					z = (m[2] + m[8])*s;
					w = (m[9] - m[6])*s;
				}
				else
				{
					if (m[5] > m[10])
					{
						float s = FloatMath.Sqrt(1.0F + m[5] - m[0] - m[10]);
						y = s*0.5F;
						s = 0.5F/s;
						x = (m[4] + m[1])*s;
						z = (m[9] + m[6])*s;
						w = (m[2] - m[8])*s;
					}
					else
					{
						float s = FloatMath.Sqrt(1.0F + m[10] - m[0] - m[5]);
						z = s*0.5F;
						s = 0.5F/s;
						x = (m[2] + m[8])*s;
						y = (m[9] + m[6])*s;
						w = (m[4] - m[1])*s;
					}
				}
			}
			quaternion[(offset + 0)] = x;
			quaternion[(offset + 1)] = y;
			quaternion[(offset + 2)] = z;
			quaternion[(offset + 3)] = w;
		}

		public virtual void GetEulerAngles(float[] eulerAngles, int offset)
		{
			if (offset + 3 > eulerAngles.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			float pitch = (float) Math.Asin(headView[6]);
			float roll;
			float yaw;
			if (FloatMath.Sqrt(1.0F - headView[6]*headView[6]) >= 0.01F)
			{
				yaw = (float) Math.Atan2(-headView[2], headView[10]);
				roll = (float) Math.Atan2(-headView[4], headView[5]);
			}
			else
			{
				yaw = 0.0F;
				roll = (float) Math.Atan2(headView[1], headView[0]);
			}
			eulerAngles[(offset + 0)] = (-pitch);
			eulerAngles[(offset + 1)] = (-yaw);
			eulerAngles[(offset + 2)] = (-roll);
		}
	}
}