using System;
using Android.Opengl;

namespace Google.Cardboard
{
	using GLES20 = GLES20;

	public class Viewport
	{
		public int x;
		public int y;
		public int width;
		public int height;

		public virtual void SetViewport(int x, int y, int width, int height)
		{
			this.x = x;

			this.y = y;

			this.width = width;
			this.height = height;
		}

		public virtual void SetGlViewport()
		{
			GLES20.GlViewport(x, y, width, height);
		}

		public virtual void SetGlScissor()
		{
			GLES20.GlScissor(x, y, width, height);
		}

		public virtual void GetAsArray(int[] array, int offset)
		{
			if (offset + 4 > array.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			array[offset] = x;

			array[(offset + 1)] = y;

			array[(offset + 2)] = width;

			array[(offset + 3)] = height;
		}

		public override string ToString()
		{
			return "Viewport {x:" + x + " y:" + y + " width:" + width + " height:" + height + "}";
		}
	}
}