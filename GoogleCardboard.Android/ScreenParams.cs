using System;
using Android.Views;
using Android.Util;

namespace Google.Cardboard
{
	public class ScreenParams
	{
		public const float METERS_PER_INCH = 0.0254F;
		private const float DEFAULT_BORDER_SIZE_METERS = 0.003F;
		private int mWidth;
		private int mHeight;
		private float mXMetersPerPixel;
		private float mYMetersPerPixel;
		private float mBorderSizeMeters;

		public ScreenParams(Display display)
		{
			DisplayMetrics metrics = new DisplayMetrics();
			// TODO : FIX EXCEPTION
			//try
			//{
			//	display.GetRealMetrics(metrics);
			//}
			//catch (MissingMethodException)
			//{
				display.GetMetrics(metrics);
			//}
			mXMetersPerPixel = (0.0254F/metrics.Xdpi);
			mYMetersPerPixel = (0.0254F/metrics.Ydpi);
			mWidth = metrics.WidthPixels;
			mHeight = metrics.HeightPixels;
			mBorderSizeMeters = 0.003F;
			if (mHeight > mWidth)
			{
				int tempPx = mWidth;
				mWidth = mHeight;
				mHeight = tempPx;

				float tempMetersPerPixel = mXMetersPerPixel;
				mXMetersPerPixel = mYMetersPerPixel;
				mYMetersPerPixel = tempMetersPerPixel;
			}
		}

		public ScreenParams(ScreenParams @params)
		{
			mWidth = @params.mWidth;
			mHeight = @params.mHeight;
			mXMetersPerPixel = @params.mXMetersPerPixel;
			mYMetersPerPixel = @params.mYMetersPerPixel;
			mBorderSizeMeters = @params.mBorderSizeMeters;
		}

		public virtual int Width
		{
			set { mWidth = value; }
			get { return mWidth; }
		}

		public virtual int Height
		{
			set { mHeight = value; }
			get { return mHeight; }
		}

		public virtual float WidthMeters
		{
			get { return mWidth*mXMetersPerPixel; }
		}

		public virtual float HeightMeters
		{
			get { return mHeight*mYMetersPerPixel; }
		}

		public virtual float BorderSizeMeters
		{
			set { mBorderSizeMeters = value; }
			get { return mBorderSizeMeters; }
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
			if (!(other is ScreenParams))
			{
				return false;
			}
			ScreenParams o = (ScreenParams) other;

			return (mWidth == o.mWidth) && (mHeight == o.mHeight) && (mXMetersPerPixel == o.mXMetersPerPixel) &&
			       (mYMetersPerPixel == o.mYMetersPerPixel) && (mBorderSizeMeters == o.mBorderSizeMeters);
		}
	}
}